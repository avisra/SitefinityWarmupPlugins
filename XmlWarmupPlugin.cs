using Louw.SitemapParser;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Warmup;

namespace Avisra.WarmupPlugins
{
    public class XmlWarmupPlugin : IWarmupPlugin
    {
        public virtual string Name { get; private set; }

        public virtual IEnumerable<WarmupUrl> GetUrls()
        {
            return this.pageUrls.Select(u => new WarmupUrl(u) { Priority = this.priority });
        }

        public virtual void GetLinksFromSitemap(Sitemap sitemap)
        {
            if (sitemap.SitemapType == SitemapType.Index)
                Log.Write($"Parsing Sitemap Index {sitemap.SitemapLocation.AbsoluteUri} for warmup. Contains {sitemap.Sitemaps.Count()} sitemaps", TraceEventType.Information);
            else if (sitemap.SitemapType == SitemapType.Items)
                Log.Write($"Parsing Sitemap {sitemap.SitemapLocation.AbsoluteUri} for warmup. Contains {sitemap.Items.Count()} items", TraceEventType.Information);

            if (sitemap.Items != null && sitemap.Items.Count() > 0)
            {
                this.pageUrls = this.pageUrls.Union(sitemap.Items.Where(i => Uri.IsWellFormedUriString(i.Location.AbsoluteUri, UriKind.Absolute)).Select(i => i.Location.AbsoluteUri));
            }

            if(sitemap.Sitemaps != null && sitemap.Sitemaps.Count() > 0)
            {
                foreach(var childSitemap in sitemap.Sitemaps)
                {
                    this.GetLinksFromSitemap(childSitemap);
                }
            }
        }

        public virtual void Initialize(string name, NameValueCollection parameters)
        {
            this.Name = name;
            var priorityKey = parameters[XmlWarmupPlugin.PriorityKey]?.ToLower();
            if (!priorityKey.IsNullOrEmpty())
            {
                switch (priorityKey)
                {
                    case "high":
                        this.priority = WarmupPriority.High;
                        break;
                    case "low":
                        this.priority = WarmupPriority.Low;
                        break;
                    default:
                        this.priority = WarmupPriority.Normal;
                        break;
                }
            }

            var sitemapUri = parameters[XmlWarmupPlugin.SitemapUriKey];
            if (!sitemapUri.IsNullOrEmpty())
            {
                var sitemapLink = new Sitemap(new Uri(sitemapUri));
                var loadedSitemap = Task.Run<Sitemap>(async () => await sitemapLink.LoadAsync()).Result;
                GetLinksFromSitemap(loadedSitemap);
            }
        }

        private IEnumerable<string> pageUrls = Enumerable.Empty<string>();
        private WarmupPriority priority = WarmupPriority.Normal;
        private const string SitemapUriKey = "sitemapUri";
        private const string PriorityKey = "priority";
    }
}
