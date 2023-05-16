using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Bytely.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BytelyKeyController : Controller
    {
        private readonly string URLSKEY = "URLS";
        private IMemoryCache _cache { get; set; }

        public BytelyKeyController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public List<BytelyRedirect> GetBytelyRedirects()
        {
            List<BytelyRedirect> result;
            _cache.TryGetValue(URLSKEY, out result);

            return result;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{url}")]
        public BytelyRedirect GetBytelyRedirectForURL([FromRoute]string url)
        {
            url = Uri.UnescapeDataString(url);

            //In real world, there would be calls to a data layer to retrieve a redirect or generate one and return it
            //For this example, we will generate a key and then see if we already have it.
            var key = Math.Abs(url.GetHashCode()).ToString();

            List<BytelyRedirect> redirects;
            BytelyRedirect redirect = null;

            if (!_cache.TryGetValue(URLSKEY, out redirects))
            {
                //Create the list of redirects in memory  and add this new one to it
                using (var cacheItem = _cache.CreateEntry(URLSKEY))
                {
                    //set long for demonstration purposes
                    cacheItem.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddYears(1));
                    redirects = new List<BytelyRedirect>();
                    cacheItem.Value = redirects;
                }
            }

            //we have an ongoing list in memory, so lets add to it or fetch from it.
            redirect = redirects.FirstOrDefault(r => r.BytelyKey.Equals(key));
            if(redirect == null)
            {
                redirect = new BytelyRedirect()
                {
                    BytelyKey = key,
                    RedirectURL = url,
                    //BytelyURL = $"Bytely/red/{key}"
                    BytelyURL = $"https://localhost:7076/{key}"
                };

                redirects.Add(redirect);
            }
            

            return redirect;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{key}")]
        public BytelyRedirect GetBytelyRedirectForKey(string key)
        {
            //It would be assumed that the Key would exist, so simply return the redirect URL
            //Or if an invalid Key was provided, return no redirect URL.
            //Either way, we wouldn't try and create a key or redirect object from this end point.
            List<BytelyRedirect> redirects;
            if (_cache.TryGetValue(URLSKEY, out redirects))
                return redirects.FirstOrDefault(r => r.BytelyKey.Equals(key));
            else
                return new BytelyRedirect();
        }

        [HttpDelete]
        [Route("/[controller]/[action]/{key}")]
        public void DeleteUrlByKey(string key)
        {
            List<BytelyRedirect> redirects;
            if (_cache.TryGetValue(URLSKEY, out redirects))
            {
                redirects.RemoveAll(r => r.BytelyKey.Equals(key));
            }
        }
    }
}
