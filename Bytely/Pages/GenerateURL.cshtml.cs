using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Bytely.Pages
{
    public class AddURLModel : PageModel
    {
        public string Url { get; set; }
        public string BytelyKey { get; set; }
        public string BytelyUrl { get; set; }

        private readonly IMemoryCache _cache;
        private readonly string urlsKey = "URLS";


        public AddURLModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnGet()
        {
            UrlModel model;
            if (_cache.TryGetValue("CurrentUrlModel", out model))
            {
                Url = model.RedirectURL;
                BytelyKey = model.BytelyKey;
                BytelyUrl = model.BytelyUrl;
            }
        }

        public async Task<IActionResult> OnPost([FromForm]string url)
        {
            if (url == null || string.IsNullOrWhiteSpace(url))
                RedirectToPage("/GenerateURL");

            var urlModel = new UrlModel();

            using (var httpClient = new HttpClient())
            {
                //the endpoint we hit here would normally be determined based on environment we are running in.
                using (var response = await httpClient.GetAsync($"http://localhost:5202/BytelyKey/GetBytelyRedirectForURL/{Uri.EscapeDataString(url)}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    urlModel = JsonConvert.DeserializeObject<UrlModel>(apiResponse);
                }
            }

            List<UrlModel> urls;
            if(!_cache.TryGetValue(urlsKey, out urls) || urls == null)
            {
                using (var entry = _cache.CreateEntry(urlsKey))
                {
                    urls = new List<UrlModel>();

                    entry.Value = urls;

                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(60));
                }
            }

            urls.Add(urlModel);

            using(var item = _cache.CreateEntry("CurrentUrlModel"))
            {
                item.Value = urlModel;
            }

            return RedirectToPage("/GenerateURL");
        }
    }
}
