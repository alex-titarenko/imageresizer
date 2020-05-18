using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using Microsoft.Extensions.Options;
using TAlex.ImageProxy.Options;
using Microsoft.Net.Http.Headers;

namespace TAlex.ImageProxy
{
    public class ImageProxyFunction
    {
        private readonly IImageResizerService imageResizerService;
        private readonly IOptions<ClientCacheOptions> clientCacheOptions;
        private readonly ILogger logger;

        public ImageProxyFunction(IImageResizerService imageProxyService, IOptions<ClientCacheOptions> clientCacheOptions, ILogger<ImageProxyFunction> logger)
        {
            this.imageResizerService = imageProxyService;
            this.clientCacheOptions = clientCacheOptions;
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

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }

        private bool SetCacheHeaders(HttpRequest request)
        {
            if (request.HttpContext.Request.GetTypedHeaders().IfModifiedSince.HasValue)
            {
                return false;
            }

            var responseHeaders = request.HttpContext.Response.GetTypedHeaders();
            responseHeaders.CacheControl = new CacheControlHeaderValue { Public = true };
            responseHeaders.LastModified = new DateTimeOffset(new DateTime(1900, 1, 1));
            responseHeaders.Expires = new DateTimeOffset((DateTime.Now + this.clientCacheOptions.Value.MaxAge).ToUniversalTime());
            return true;
        }
    }
}
