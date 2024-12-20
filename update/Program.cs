﻿using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;

var template = @"# ✌ Olá, boas vindas!

Me chamo **Lucas**, sou formado em Engenharia da Computação, tenho MBAs em Engenharia de Software e em CyberSecurity e Ethical Hacking.
Com 14 anos de experiência em TI, trabalho atualmente como Líder Técnico.

Meu foco é em DevSec e desenvolvimento backend com .NET, mas sempre busco novos conhecimentos nos mais diversos assuntos e tecnologias, e nos meus repositórios você vai encontrar todos tipos de projetos.
</br><p align=""center"">
<a href=""https://bsky.app/profile/lfrigodesouza.net"" rel=""me""><img src=""https://img.shields.io/badge/-BlueSky-161e27?style=flat-square&logo=bluesky&logoColor=0285FF&link=https://bsky.app/profile/lfrigodesouza.net""></a>
<a href=""https://techhub.social/@lfrigodesouza"" rel=""me""><img src=""https://img.shields.io/badge/-Mastodon-191b22?style=flat-square&logo=mastodon&logoColor=6162fe&link=https://techhub.social/@lfrigodesouza""></a>
<a href=""https://www.threads.net/@lfrigodesouza"" rel=""me""><img src=""https://img.shields.io/badge/-Threads-161e27?style=flat-square&logo=threads&logoColor=FFFFFF&link=https://www.threads.net/@lfrigodesouza""></a>
<a href=""https://www.linkedin.com/in/lfrigodesouza/""><img src=""https://img.shields.io/badge/-LinkedIn-0077B5?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/lfrigodesouza/""></a>
<a href=""https://LFrigoDeSouza.NET/""><img src=""https://img.shields.io/badge/-LFS.NET-9e9e9e?style=flat-square&logo=firefox-browser&logoColor=white&link=https://LFrigoDeSouza.NET/""></a>
</p>

## ✒️Artigos Recentes
<ul>
#ARTICLES_PLACEHOLDER#
<li style=""list-style-type: none;""><a href=""https://blog.lfrigodesouza.net"" target=""_blank"">Veja mais...</a></li>
</ul>

## 👨‍💻 Meu GitHub
<p align=""center"">
<img src=""https://github-readme-stats.vercel.app/api/top-langs/?username=lfrigodesouza&layout=compact&theme=dark""/>
<img src=""https://github-readme-stats.vercel.app/api?username=lfrigodesouza&show_icons=true&theme=dark"">
</p>
";
Console.WriteLine("Iniciando processamento do README.md");
var url = "https://lfrigodesouza-functions.azurewebsites.net/api/blog-latests-posts";
var client = new HttpClient();
var response = await client.GetAsync(url);
if (!response.IsSuccessStatusCode)
    throw new Exception("Não foi possível obter os dados");
var responseContent = await response.Content.ReadAsStringAsync();
if (string.IsNullOrWhiteSpace(responseContent))
    throw new Exception("Não foi possível obter os dados");
var posts = JsonDocument.Parse(responseContent).RootElement;
var postsLinks = new StringBuilder();
for (var i = 0; i < 5; i++)
{
    var (title, postDate, permaLink) = GetPostDetails(posts[i]);

    postsLinks.AppendLine(
        $"<li style=\"list-style-type: none;\"><a href=\"{permaLink}\" target=\"_blank\">{title}</a><i> &nbsp;({GetPublishDateTime(postDate)})</i></li>");
    // postsLinks.AppendLine($"* [{title}]({permaLink}) _({GetPublishDate(postDate)})_ ");
}

var readmeContent = template.Replace("#ARTICLES_PLACEHOLDER#", postsLinks.ToString());
File.WriteAllText("../README.md", readmeContent);

string GetPublishDateTime(DateTime postDate){
    var publishedDateTime = System.TimeZoneInfo.ConvertTimeFromUtc(
    postDate.ToUniversalTime(), 
    TimeZoneInfo.FindSystemTimeZoneById("Brazil/East"));
    return publishedDateTime.ToString("dd/MM/yy HH:mm:ss");
}

string GetPublishDate(DateTime postDate)
{
    var daysFromPost = (DateTime.Now.Date - postDate.Date).TotalDays;
    return daysFromPost switch
    {
        < 1 => "hoje",
        < 2 => "1 dia atrás",
        _ => $"{daysFromPost.ToString("#")} dias atrás"
    };
}

(string title, DateTime postDate, string permaLink) GetPostDetails(JsonElement post)
{
    var title = post.GetProperty("title").ToString();
    var postDate = post.GetProperty("date").GetDateTime();
    var permaLink = post.GetProperty("permalink").ToString();
    return (title, postDate, permaLink);
}
