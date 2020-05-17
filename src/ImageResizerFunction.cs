using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;


namespace TAlex.ImageProxy
{
    public class ImageProxyFunction
    {
        private readonly IImageResizerService imageResizerService;

        private readonly ILogger logger;

        public ImageProxyFunction(IImageResizerService imageProxyService, ILogger<ImageProxyFunction> logger)
        {
            this.imageResizerService = imageProxyService;
            this.logger = logger;
        }

        [FunctionName("ResizeImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "resizeimage/{size}")] HttpRequest req,
            string size)
        {
            if (SetCacheHeaders(req))
            {
                var url = req.Query["url"];
                var imageStream = await this.imageResizerService.ResizeAsync(size, url);
                return new FileStreamResult(imageStream, "image/png");
            }

            return null;
        }

        private bool SetCacheHeaders(HttpRequest request)
        {
            if (request.HttpContext.Request.GetTypedHeaders().IfModifiedSince.HasValue)
            {
                request.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotModified;
                return false;
            }

            var responseHeaders = request.HttpContext.Response.GetTypedHeaders();
            responseHeaders.CacheControl.Public = true;
            responseHeaders.LastModified = new DateTimeOffset(new DateTime(1900, 1, 1));
            responseHeaders.Expires = (DateTime.Now + Settings.ClientCacheMaxAge).ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
            return true;
        }
    }
}
