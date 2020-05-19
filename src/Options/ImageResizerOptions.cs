using System.Collections.Generic;


namespace TAlex.ImageResizer.Service.Options
{
    public class ImageResizerOptions
    {
        public string? SmallSize { get; set; }

        public string? MediumSize { get; set; }

        public string? DetailSize { get; set; }

        public string? UserAgent { get; set; }

        public IDictionary<string, ImageSize> PredefinedImageSizes
        {
            get
            {
                var dic = new Dictionary<string, ImageSize>();
                if (!string.IsNullOrWhiteSpace(this.SmallSize))
                {
                    dic.Add("small", ImageSize.Parse(SmallSize, "small"));
                }
                if (!string.IsNullOrWhiteSpace(this.MediumSize))
                {
                    dic.Add("medium", ImageSize.Parse(MediumSize, "medium"));
                }
                if (!string.IsNullOrWhiteSpace(this.DetailSize))
                {
                    dic.Add("detailed", ImageSize.Parse(DetailSize, "detailed"));
                }

                return dic;
            }
        }
    }
}
