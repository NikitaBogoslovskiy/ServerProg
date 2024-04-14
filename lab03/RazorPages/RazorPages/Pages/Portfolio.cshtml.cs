using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace RazorPages.Pages
{
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
