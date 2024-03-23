using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace RazorPages.Pages
{
/*    public class Banner
    {
        public string Title { get; set; }
        public string MainPage { get; set; }
        public string ThisPage { get; set; }
    }

    public class Header
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ProjectCard
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImagePath { get; set; }
    }

    public class PortfolioContent
    {
        public Banner Banner { get; set; }
        public Header Header { get; set; }
        public List<string> Filters { get; set; }
        public List<ProjectCard> ProjectCards { get; set; }
    }*/

    public interface IContentService
    {
        public void Load(string path);
        public string? Get(string tokenPath);
    }

    public class ContentService : IContentService
    {
        private JObject content;

        public void Load(string path)
        {
            content = JObject.Parse(File.ReadAllText(path));
        }

        public string? Get(string tokenPath)
        {
            return (string?)content.SelectToken(tokenPath);
        }
    }

    public class PortfolioModel : PageModel
    {
        public readonly IContentService contentService;
        private readonly string pathToContent = "wwwroot/PortfolioContent.json";

        public PortfolioModel(IContentService _contentService)
        {
            contentService = _contentService;
            contentService.Load(pathToContent);
        }

        public void OnGet()
        {
        }
    }
}
