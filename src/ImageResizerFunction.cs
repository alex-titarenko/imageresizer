using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using Microsoft.Extensions.Options;
using TAlex.ImageResizer.Service.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Headers;

namespace TAlex.ImageResizer.Service
{
    public class ImageProxyFunction
    {
        private readonly IImageResizerService imageResizerService;
        private readonly IOptions<ClientCacheOptions> clientCacheOptions;

        public ImageProxyFunction(IImageResizerService imageProxyService, IOptions<ClientCacheOptions> clientCacheOptions)
        {
            this.imageResizerService = imageProxyService;
            this.clientCacheOptions = clientCacheOptions;
        }

        [FunctionName("ResizeImage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            if (req.HttpContext.Request.GetTypedHeaders().IfModifiedSince.HasValue)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotModified);
            }

            this.SetCacheHeaders(req.HttpContext.Response.GetTypedHeaders());
            var url = req.Query["url"];
            var size = req.Query["size"];
            var imageStream = await this.imageResizerService.ResizeAsync(url, size);

            return new FileStreamResult(imageStream, "image/png");
        }

        private void SetCacheHeaders(ResponseHeaders responseHeaders)
        {
            responseHeaders.CacheControl = new CacheControlHeaderValue { Public = true };
            responseHeaders.LastModified = new DateTimeOffset(new DateTime(1900, 1, 1));
            responseHeaders.Expires = new DateTimeOffset((DateTime.Now + this.clientCacheOptions.Value.MaxAge).ToUniversalTime());
        }
    }
}
