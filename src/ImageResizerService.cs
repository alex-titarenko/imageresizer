/*using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows.Media.Imaging;
using TAlex.ImageProxy.Extensions;
*/

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TAlex.ImageProxy
{
    public class ImageResizerService : IImageResizerService
    {
        private readonly IOptions<ImageResizerSettings> settings;
        private readonly ILogger logger;

        public ImageResizerService(IOptions<ImageResizerSettings> settings, ILogger<ImageResizerService> logger)
        {
            this.settings = settings;
            this.logger = logger;
        }

        public async System.Threading.Tasks.Task<System.IO.Stream> ResizeAsync(string size, string url)
        {
            throw new System.NotImplementedException();

            /*if (String.IsNullOrWhiteSpace(size) || String.IsNullOrWhiteSpace(url))
            {
                Trace.TraceError("Uri and size parameters can't be empty.");
                return GetErrorStream();
            }

            try
            {
                if (context.IncomingRequest.IfModifiedSince.HasValue)
                {
                    context.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
                    return null;
                }
                SetCacheHeaders(context.OutgoingResponse);
                return GetResultStream(NormalizeUrl(url), StringToImageSize(size));
            }
            catch (Exception exc)
            {
                Trace.TraceError("An error was occured during taking image by uri: {0}. Error: {1}", url, exc);
                return GetErrorStream();
            }*/
        }

        /*private void SetCacheHeaders(OutgoingWebResponseContext response)
        {
            response.ContentType = "image/png";
            response.Headers[HttpResponseHeader.CacheControl] = "public";
            response.LastModified = new DateTime(1900, 1, 1);
            response.Headers[HttpResponseHeader.Expires] = (DateTime.Now + Settings.ClientCacheMaxAge).ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
        }

        private Stream GetErrorStream()
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            }
            return new MemoryStream(Encoding.UTF8.GetBytes("<h1>Error 500</h1><h2>Internal Server Error</h2>"));
        }

        private Stream GetResultStream(Uri uri, ImageSize imageSize)
        {
            if (Settings.UseLocalCache)
            {
                string requestFileName = GetDownloadPath(uri);
                string cachedFileName = ResolveFileName(requestFileName, imageSize.ToString());

                // looking for cached file on disk
                if (File.Exists(cachedFileName))
                {
                    return OpenImageStream(cachedFileName);
                }

                // looking for original file on disk
                string original = ResolveFileName(requestFileName, ImageSize.OriginalImageSize);

                if (File.Exists(original))
                {
                    return GetResizedImage(OpenImageStream(original), imageSize, requestFileName);
                }
            }
            return GetImageStream(uri, imageSize);
        }

        private Stream GetImageStream(Uri uri, ImageSize imageSize)
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.UserAgent = Settings.UserAgent;

                using (var response = request.GetResponse())
                {
                    var imageStream = new MemoryStream();
                    using (var responseStream = response.GetResponseStream())
                    {
                        responseStream.CopyTo(imageStream);
                        imageStream.Position = 0;
                    }

                    if (Settings.UseLocalCache)
                    {
                        SaveOriginalFileToStorage(imageStream, uri);
                    }
                    return (imageSize.Name == ImageSize.OriginalImageSize) ?
                        imageStream :
                        GetResizedImage(imageStream, imageSize, GetDownloadPath(uri));
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError(exc.ToString());
                return GetErrorStream();
            }
        }

        private Stream GetResizedImage(Stream originalStream, ImageSize size, string fileName)
        {
            BitmapFrame frame = ImageHelper.ReadBitmapFrame(originalStream);
            BitmapFrame resizedFrame = frame.ResizeImage(size.Width, size.Height);

            if (Settings.UseLocalCache)
            {
                string name = ResolveFileName(fileName, size.ToString());
                resizedFrame.SaveToFile(name);
                return OpenImageStream(name);
            }
            return resizedFrame.GetStream();
        }

        private static Stream OpenImageStream(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        private ImageSize StringToImageSize(string value)
        {
            ImageSize imageSize;
            if (Settings.PredefinedImageSizes.TryGetValue(value, out imageSize))
            {
                return imageSize;
            }
            return ImageSize.Parse(value);
        }

        private string GetDownloadPath(Uri uri)
        {
            return String.IsNullOrWhiteSpace(uri.Query)
                ? String.Format("{0}{1}", Settings.LocalCachePath, GetPathName(uri))
                : String.Format("{0}{1}_{2}", Settings.LocalCachePath, GetPathName(uri), uri.Query.GetHashCode());
        }

        private static string ResolveFileName(string file, string size)
        {
            string path = String.Format("{0}\\{1}_{2}",
                Path.GetDirectoryName(file),
                Path.GetFileNameWithoutExtension(file),
                size.ToLowerInvariant());

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, path);
            }

            return path;
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

        private static string GetPathName(Uri uri, string defaultExtension = null)
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
        }*/
    }
}
