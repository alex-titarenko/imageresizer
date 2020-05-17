using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            this.logger.LogInformation("---------------Start---------------------");

            var url = req.Query["url"];

            this.logger.LogInformation($"size:{size}");
            this.logger.LogInformation($"url:{url}");

            this.logger.LogInformation("---------------End---------------------");

            var imageStream = await this.imageResizerService.ResizeAsync(size, url);
            return new FileStreamResult(imageStream, "image");
        }
    }
}
