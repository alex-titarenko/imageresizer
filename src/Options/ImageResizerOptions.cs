using System.Collections.Generic;


namespace TAlex.ImageProxy.Options
{
    public class ImageResizerOptions
    {
        public string? SmallSize { get; set; }

        public string? MediumSize { get; set; }

        public string? DetailSize { get; set; }

        public bool UseCacheStorage { get; set; }

        public string? UserAgent { get; set; }

        public IDictionary<string, ImageSize> PredefinedImageSizes
        {
            get
            {
                var dic = new Dictionary<string, ImageSize>();
                if (!string.IsNullOrWhiteSpace(this.SmallSize))
                {
                    dic.Add(nameof(SmallSize).ToLowerInvariant(), ImageSize.Parse(SmallSize, nameof(SmallSize)));
                }
                if (!string.IsNullOrWhiteSpace(this.MediumSize))
                {
                    dic.Add(nameof(MediumSize).ToLowerInvariant(), ImageSize.Parse(MediumSize, nameof(MediumSize)));
                }
                if (!string.IsNullOrWhiteSpace(this.DetailSize))
                {
                    dic.Add(nameof(DetailSize).ToLowerInvariant(), ImageSize.Parse(DetailSize, nameof(DetailSize)));
                }

                return dic;
            }
        }
    }
}
