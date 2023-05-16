using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Bytely.Pages
{
    public class URLsModel : PageModel
    {
        private readonly string URLSKEY = "URLS";
        private readonly IMemoryCache _cache;
        public List<UrlModel> URLs { get; set; }

        public URLsModel(IMemoryCache cache)
        {
            _cache = cache;
            URLs = new List<UrlModel>();
        }

        public async void OnGet()
        {
            List<UrlModel> urls = null;
            if(!_cache.TryGetValue(URLSKEY, out urls))
            {
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
                    URLs = new List<UrlModel>();
                    entry.Value = urls;
                    entry.SetSlidingExpiration(TimeSpan.FromMinutes(60));
                }
            }

            this.URLs = urls;
        }
    }

    public class UrlModel
    {
        public string RedirectURL { get; set; }
        public string BytelyKey { get; set; }
        public string BytelyUrl { get; set; }
    }
}
