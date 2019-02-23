# Sitefinity XML Sitemap Warmup Plugin

I was heavily involved in the buildout of Sitefinity's out-of-the-box warmup module. The warmup module is built to warmup pages and be extensible to warmup any URLs in the application. It supports multiple priorities for warmup - so some routes can be configured to warmup during the startup process (application isn't available until these high priority pages complete) and others will warmup with a background task after the application has started.

By default, the warmup module comes with a single "plugin" for warming up sitemap pages. This accounts for some of the more "important" routes to warmup, but it does not account for any of the detail routes for modular content (news, events, blog posts, dynamic content, etc). 

This plugin will read from an XML sitemap (like the one that is generated from Sitefinity's XML Sitemap Generator). It is using a 3rd party tool for the sitemap parsing: https://github.com/louislouw/Louw.SitemapParser. This tool can parse sitemap indexes, sitemap files, and handles gzip compressed sitemaps.

Keep in mind that Sitefinity's Sitemap Generator module uses the content location service behind the scenes. If you aren't careful with how you develop/configure widgets, you could easily have unintended routes (or no routes!) in the generated sitemap for each content type. Read more about how the content location service works here: https://www.progress.com/documentation/sitefinity-cms/for-developers-locations-of-content-items
