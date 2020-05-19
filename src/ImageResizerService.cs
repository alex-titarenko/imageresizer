using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using TAlex.ImageProxy.Options;


namespace TAlex.ImageProxy
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

        public async Task<System.IO.Stream> ResizeAsync(string size, string url)
        {
            return await this.GetResultStreamAsync(NormalizeUrl(url), StringToImageSize(size));
        }

        private async Task<Stream> GetResultStreamAsync(Uri uri, ImageSize imageSize)
        {
            if (this.settings.Value.UseCacheStorage)
            {
                throw new NotImplementedException();
            }

            return await this.GetImageStreamAsync(uri, imageSize);
        }

        private async Task<Stream> GetImageStreamAsync(Uri uri, ImageSize imageSize)
        {
            this.httpClient.DefaultRequestHeaders.UserAgent.Clear();
            this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(this.settings.Value.UserAgent);

            var imageStream = new MemoryStream();
            using (var responseStream = await this.httpClient.GetStreamAsync(uri))
            {
                responseStream.CopyTo(imageStream);
                imageStream.Position = 0;
            }

            if (this.settings.Value.UseCacheStorage)
            {
                this.SaveOriginalFileToStorage(imageStream, uri);
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
                image.Mutate(x => x.Resize(size.Width, size.Height));
                image.SaveAsPng(resultStream);
                resultStream.Position = 0;
            }

            if (this.settings.Value.UseCacheStorage)
            {
                throw new NotImplementedException();
            }
            return resultStream;
        }

        private static Stream OpenImageStream(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        private ImageSize StringToImageSize(string value)
        {
            ImageSize imageSize;
            if (this.settings.Value.PredefinedImageSizes.TryGetValue(value, out imageSize))
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

        private static string GetPathName(Uri uri, string? defaultExtension = null)
        {
            char dirSeparator = Path.DirectorySeparatorChar;

            // construct path in the hard disk
            string strLocalPath = uri.LocalPath.Replace('/', dirSeparator);

            // check if the path ends with / to can crate the file on the HD
            if (strLocalPath.EndsWith(dirSeparator.ToString()))
            {
                string fileName = string.IsNullOrEmpty(uri.Query) ? "default" : uri.Query.GetHashCode().ToString();
                strLocalPath =
                    (string.IsNullOrEmpty(defaultExtension)) ?
                    string.Format("{0}{1}", strLocalPath, fileName)
                    : string.Format("{0}{1}.{2}", strLocalPath, fileName, defaultExtension);
            }

            return String.Format("{0}{1}{2}", dirSeparator, uri.Host, strLocalPath.Replace("%20", " "));
        }

        private void SaveOriginalFileToStorage(Stream imageStream, Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
