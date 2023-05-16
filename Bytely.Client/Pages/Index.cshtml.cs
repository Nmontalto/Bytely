using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Bytely.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        
    public async Task<IActionResult> OnGetAsync(string redKey)
        {
            if (redKey != null)
            {
                string redUrl;
                UrlModel urlModel;
                using (var httpClient = new HttpClient())
                {
                    //the endpoint we hit here would normally be determined based on environment we are running in and not be hardcoded.
                    using (var response = await httpClient.GetAsync($"http://localhost:5202/BytelyKey/GetBytelyRedirectForKey/{redKey}"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        urlModel = JsonConvert.DeserializeObject<UrlModel>(apiResponse);
                    }
                }

                if (urlModel != null && urlModel.BytelyKey.Length > 0)
                    return Redirect(urlModel.RedirectUrl);
                else
                    return Redirect("/GenerateURL");
            }
            else
                return Redirect("/GenerateURL");
        }
    }
}