﻿using System;
using System.Collections.Generic;


namespace TAlex.ImageProxy
{
    public class ImageResizerSettings
    {
        public string SmallSize { get; set; }

        public string MediumSize { get; set; }

        public string DetailSize { get; set; }

        public bool UseCacheStorage { get; set; }

        public TimeSpan ClientCacheMaxAge { get; set; }

        public string UserAgent { get; set; }

        public IDictionary<string, ImageSize> PredefinedImageSizes =>
            new Dictionary<string, ImageSize>
            {
                [nameof(SmallSize)] = ImageSize.Parse(SmallSize, nameof(SmallSize)),
                [nameof(MediumSize)] = ImageSize.Parse(MediumSize, nameof(MediumSize)),
                [nameof(DetailSize)] = ImageSize.Parse(DetailSize, nameof(DetailSize))
            };
    }
}
