using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace TAlex.ImageProxy
{
    public class ImageProxyFunction
    {
        private readonly IImageProxyService imageProxyService;
        private readonly ILogger logger;

        public ImageProxyFunction(IImageProxyService imageProxyService, ILogger<ImageProxyFunction> logger)
        {
            this.imageProxyService = imageProxyService;
            this.logger = logger;
        }

        [FunctionName("GetImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getimage/{size}")] HttpRequest req,
            string size)
        {
            this.logger.LogInformation("---------------Start---------------------");

            var url = req.Query["url"];

            this.logger.LogInformation($"size:{size}");
            this.logger.LogInformation($"url:{url}");

            this.logger.LogInformation("---------------End---------------------");

            return new OkResult();

            //var imageStream = await this.imageProxyService.GetImageAsync(size, url);
            //return new FileStreamResult(imageStream, "image");
        }
    }
}
