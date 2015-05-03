using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;


namespace TAlex.ImageProxy
{
    public class ProxySettings
    {
        #region Fields

        protected static ProxySettings _current;

        private static readonly List<string> ImageSizeSettings = new List<string>()
        {
            "Small", "Medium", "Detail"
        };

        protected readonly NameValueCollection AppSettings;

        #endregion

        #region Properties

        public static ProxySettings Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new ProxySettings();
                }
                return _current;
            }
        }


        public IDictionary<string, ImageSize> PredefinedImageSizes { get; private set; }

        public bool UseLocalCache
        {
            get
            {
                return (bool)Convert.ToBoolean(AppSettings.Get("UseLocalCache"));
            }
        }

        public string LocalCachePath
        {
            get
            {
                return AppSettings.Get("LocalCachePath");
            }
        }

        public TimeSpan ClientCacheMaxAge
        {
            get
            {
                return TimeSpan.Parse(AppSettings.Get("ClientCacheMaxAge"), CultureInfo.InvariantCulture);
            }
        }

        public string UserAgent
        {
            get
            {
                return AppSettings.Get("UserAgent");
            }
        }

        #endregion

        #region Constructors

        protected ProxySettings()
        {
            AppSettings = ConfigurationManager.AppSettings;

            PredefinedImageSizes = new Dictionary<string, ImageSize>();
            ImageSizeSettings.ForEach(x => PredefinedImageSizes.Add(GetImageSize(x)));
        }

        #endregion

        #region Methods

        private KeyValuePair<string, ImageSize> GetImageSize(string name)
        {
            return new KeyValuePair<string, ImageSize>(name, ImageSize.Parse(AppSettings[name], name));
        }

        #endregion
    }
}