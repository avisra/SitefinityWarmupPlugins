using Louw.SitemapParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;

namespace Avisra.WarmupPlugins
{
    public class CustomSitemapFetcher : ISitemapFetcher
    {
        private readonly string _userAgent;

        public CustomSitemapFetcher()
        {
            _userAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";
        }

        public CustomSitemapFetcher(string userAgent)
        {
            _userAgent = userAgent;
        }

        public async Task<string> Fetch(Uri sitemapLocation)
        {
            //Automatically handle gzip compressed content
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml, */*");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Warmup-Code", GetCurrentWarmupCode());
                //client.DefaultRequestHeaders.Add("Accept-Charset", "ISO-8859-1");

                return await client.GetStringAsync(sitemapLocation);
            }
        }

        private string GetCurrentWarmupCode()
        {
            Type type = typeof(Bootstrapper); // MyClass is static class with static properties
            return type.GetField("currentWarmupCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.GetValue(null) as string;
        }
    }
}
