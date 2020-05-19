using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TAlex.ImageResizer.Service.Options;


namespace TAlex.ImageResizer.Service
{
    public class ImageResizerService : IImageResizerService
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<ImageResizerOptions> settings;
        private readonly ILogger logger;

        public ImageResizerService(HttpClient httpClient, IOptions<ImageResizerOptions> settings, ILogger<ImageResizerService> logger)
        {
            this.httpClient = httpClient;
            this.settings = settings;
            this.logger = logger;
        }

        public async Task<Stream> ResizeAsync(string url, string size)
        {
            return await this.GetResultStreamAsync(NormalizeUrl(url), StringToImageSize(size));
        }

        private async Task<Stream> GetResultStreamAsync(Uri uri, ImageSize imageSize)
        {
            this.httpClient.DefaultRequestHeaders.UserAgent.Clear();
            this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(this.settings.Value.UserAgent);

            var imageStream = new MemoryStream();
            using (var responseStream = await this.httpClient.GetStreamAsync(uri))
            {
                responseStream.CopyTo(imageStream);
                imageStream.Position = 0;
            }

            return (imageSize.Name == ImageSize.OriginalImageSize) ?
                imageStream :
                this.GetResizedImage(imageStream, imageSize);
        }

        private Stream GetResizedImage(Stream originalStream, ImageSize size)
        {
            var resultStream = new MemoryStream();
            using (var image = Image.Load(originalStream))
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(size.Width, size.Height),
                    Mode = ResizeMode.Max
                };
                image.Mutate(x => x.Resize(resizeOptions));
                image.SaveAsPng(resultStream);
                resultStream.Position = 0;
            }

            return resultStream;
        }

        private ImageSize StringToImageSize(string value)
        {
            ImageSize imageSize;
            if (this.settings.Value.PredefinedImageSizes.TryGetValue(value.ToLowerInvariant(), out imageSize))
            {
                return imageSize;
            }
            return ImageSize.Parse(value);
        }

        private static Uri NormalizeUrl(string url)
        {
            var targetUrl = url;
            if (url.StartsWith("base64:", StringComparison.InvariantCultureIgnoreCase))
            {
                targetUrl = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(7)));
            }

            return new Uri(targetUrl.StartsWith(Uri.UriSchemeHttp) ? targetUrl : (Uri.UriSchemeHttp + Uri.SchemeDelimiter + targetUrl));
        }
    }
}
