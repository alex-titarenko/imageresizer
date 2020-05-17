using System;
using System.Collections.Generic;


namespace TAlex.ImageProxy
{
    public class ProxySettings
    {
        public string Small { get; set; }

        public string Medium { get; set; }

        public string Detail { get; set; }

        public bool UseCacheStorage { get; set; }

        public TimeSpan ClientCacheMaxAge { get; set; }

        public string UserAgent { get; set; }

        public IDictionary<string, ImageSize> PredefinedImageSizes =>
            new Dictionary<string, ImageSize>
            {
                [nameof(Small)] = ImageSize.Parse(Small, nameof(Small)),
                [nameof(Medium)] = ImageSize.Parse(Medium, nameof(Medium)),
                [nameof(Detail)] = ImageSize.Parse(Detail, nameof(Detail))
            };
    }
}
