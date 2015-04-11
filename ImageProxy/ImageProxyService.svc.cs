using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Windows.Media.Imaging;
using TAlex.ImageProxy.Extensions;


namespace TAlex.ImageProxy
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ImageProxyService : IImageProxyService
    {
        #region Fields

        private const string DownloadUserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        private static readonly List<string> _imageSizeSettings = new List<string>()
        {
            "Icon", "Small", "Medium", "Detail"
        };

        private static readonly string _imageCachePath;
        private static readonly IDictionary<string, ImageSize> _imageSizes = new Dictionary<string, ImageSize>();

        #endregion

        #region Properties

        private TimeSpan ClientCacheMaxAge
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["ClientCacheMaxAge"], CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region Constructors

        static ImageProxyService()
        {
            try
            {
                _imageSizes.Clear();
                _imageSizeSettings.ForEach(x => _imageSizes.Add(GetImageSize(x)));

                _imageCachePath = ConfigurationManager.AppSettings["ImageCachePath"];

                if (!Directory.Exists(_imageCachePath))
                {
                    Directory.CreateDirectory(_imageCachePath);
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError(exc.Message, exc);
            }
        }

        #endregion

        #region IImageProxyService Members

        public Stream GetImage(string size, string url)
        {
            if (String.IsNullOrWhiteSpace(size) || String.IsNullOrWhiteSpace(url))
            {
                Trace.TraceError("Uri and size parameters can't be empty.");
                return GetErrorStream();
            }

            try
            {
                ImageSize imageSize = StringToImageSize(size);
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext context = WebOperationContext.Current;
                    if (context.IncomingRequest.IfModifiedSince.HasValue)
                    {
                        context.OutgoingResponse.StatusCode = HttpStatusCode.NotModified;
                        return null;
                    }
                    SetCacheHeaders(context.OutgoingResponse);
                }
                return GetCachedStream(NormalizeUrl(url), imageSize);
            }
            catch (Exception exc)
            {
                Trace.TraceError("An error was occured during taking image by uri: {0}. Error: {1}", url, exc);
                return GetErrorStream();
            }
        }

        #endregion

        #region Methods

        private void SetCacheHeaders(OutgoingWebResponseContext response)
        {
            response.ContentType = "image/png";
            response.Headers[HttpResponseHeader.CacheControl] = "public";
            response.LastModified = new DateTime(1900, 1, 1);
            TimeSpan maxAge = ClientCacheMaxAge;
            response.Headers[HttpResponseHeader.Expires] = (DateTime.Now + maxAge).ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
        }

        private Stream GetErrorStream()
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            }
            return new MemoryStream(Encoding.UTF8.GetBytes("<h1>Error 404</h1><h2>Requested page not found</h2>"));
        }

        private Stream GetCachedStream(string url, ImageSize imageSize)
        {
            Uri uri = new Uri(url);
            string requestFileName = GetDownloadPath(uri);
            string cachedFileName = ResolveFileName(requestFileName, imageSize.ToString());

            //looking for cached file on disk
            if (File.Exists(cachedFileName))
            {
                return OpenImageStream(cachedFileName);
            }
            
            // looking for original file disk
            string original = ResolveFileName(requestFileName, ImageSize.OriginalImageSize);

            if (File.Exists(original))
            {
                return GetResizedImage(OpenImageStream(original), imageSize, requestFileName);
            }
            
            return GetImageStream(uri, imageSize);
        }

        private Stream GetImageStream(Uri uri, ImageSize imageSize)
        {
            string fileName = GetDownloadPath(uri);
            string folderPath = Path.Combine(_imageCachePath, uri.Host + Path.GetDirectoryName(uri.LocalPath));

            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, folderPath);
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string originalName = ResolveFileName(fileName, ImageSize.OriginalImageSize);
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.UserAgent, DownloadUserAgent);
                webClient.DownloadFile(uri, originalName);
            }

            if (imageSize.Name == ImageSize.OriginalImageSize)
            {
                return OpenImageStream(originalName);
            }
            else
            {
                return GetResizedImage(OpenImageStream(originalName), imageSize, fileName);
            }
        }

        private Stream GetResizedImage(Stream stream, ImageSize size, string fileName)
        {
            BitmapFrame frame = ImageHelper.ReadBitmapFrame(stream);
            return ResizeAndSave(frame, size, fileName);
        }

        private Stream ResizeAndSave(BitmapFrame image, ImageSize size, string realName)
        {
            BitmapFrame resized = image.ResizeImage(size.Width, size.Height);

            //saving to fileSystem
            string name = ResolveFileName(realName, size.ToString());
            resized.SaveToFile(name);
            return OpenImageStream(name);
        }

        private static Stream OpenImageStream(string fileName)
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        private ImageSize StringToImageSize(string value)
        {
            ImageSize imageSize;
            if (_imageSizes.TryGetValue(value, out imageSize))
            {
                return imageSize;
            }
            return ImageSize.Parse(value);
        }

        private string GetDownloadPath(Uri uri)
        {
            return String.IsNullOrWhiteSpace(uri.Query)
                ? String.Format("{0}{1}", _imageCachePath, GetPathName(uri))
                : String.Format("{0}{1}_{2}", _imageCachePath, GetPathName(uri), uri.Query.GetHashCode());
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

        private static string NormalizeUrl(string url)
        {
            var targetUrl = url;
            if (url.StartsWith("base64:", StringComparison.InvariantCultureIgnoreCase))
            {
                targetUrl = Encoding.UTF8.GetString(Convert.FromBase64String(url.Substring(7)));
            }
            return targetUrl.StartsWith(Uri.UriSchemeHttp) ? targetUrl : (Uri.UriSchemeHttp + Uri.SchemeDelimiter + targetUrl);
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

        private static KeyValuePair<string, ImageSize> GetImageSize(string name)
        {
            return new KeyValuePair<string, ImageSize>(name, ImageSize.Parse(ConfigurationManager.AppSettings[name], name));
        }

        #endregion
    }
}
