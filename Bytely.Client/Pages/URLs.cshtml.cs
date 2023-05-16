using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Bytely.Pages
{
    public class UrlsModel : PageModel
    {
        private readonly string URLSKEY = "URLS";
        private readonly IMemoryCache _cache;
        public List<UrlModel> Urls { get; set; }

        public UrlsModel(IMemoryCache cache)
        {
            _cache = cache;
            Urls = new List<UrlModel>();
        }

        public async void OnGet()
        {
            List<UrlModel> urls = null;
            if(!_cache.TryGetValue(URLSKEY, out urls))
            {
                await CacheUrlRedirects();
            }
            else
            {
                this.Urls = urls;
            }
        }

        public async Task<RedirectToPageResult> OnPostRemove(string key)
        {
            using (var httpClient = new HttpClient())
            {
                //Delete the Url Key from the source
                using (await httpClient.DeleteAsync($"http://localhost:5202/BytelyKey/DeleteUrlByKey/{key}")) { }
            }

            await CacheUrlRedirects();

            return RedirectToPage("Urls");
        }

        private async Task CacheUrlRedirects()
        {
            List<UrlModel> urls = null;
            using (var httpClient = new HttpClient())
            {
                //the endpoint we hit here would normally be determined based on environment we are running in.
                using (var response = await httpClient.GetAsync("http://localhost:5202/BytelyKey/GetBytelyRedirects"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    urls = JsonConvert.DeserializeObject<List<UrlModel>>(apiResponse);
                }
            }

            using (var entry = _cache.CreateEntry(URLSKEY))
            {
                Urls = new List<UrlModel>();
                entry.Value = urls;
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(60));
            }

            this.Urls = urls;
        }
    }



    public class UrlModel
    {
        public string RedirectUrl { get; set; }
        public string BytelyKey { get; set; }
        public string BytelyUrl { get; set; }
    }
}
