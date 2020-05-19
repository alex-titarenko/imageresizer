using System;
using System.Text.RegularExpressions;


namespace TAlex.ImageProxy
{
    public readonly struct ImageSize
    {
        private static Regex ImageSizeRegex = new Regex(@"(?<Width>\d+)x(?<Height>\d+)", RegexOptions.Compiled);

        public const string OriginalImageSize = "Original";


        public readonly int Width;

        public readonly int Height;

        public readonly string Name;

        public ImageSize(int width, int height, string name)
        {
            Width = width;
            Height = height;
            Name = name;
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Name))
            {
                return String.Format("{0}x{1}", Width, Height);
            }

            return Name;
        }

        public static ImageSize Parse(string s)
        {
            return Parse(s, string.Empty);
        }

        public static ImageSize Parse(string s, string name)
        {
            if (String.Equals(s, OriginalImageSize, StringComparison.OrdinalIgnoreCase) ||
                String.Equals(name, OriginalImageSize, StringComparison.OrdinalIgnoreCase))
            {
                return new ImageSize(-1, -1, OriginalImageSize);
            }

            var match = ImageSizeRegex.Match(s);

            if (!match.Success)
            {
                throw new FormatException($"Data {s} is not correct  Image size value");
            }

            int width = int.Parse(match.Groups["Width"].Value);
            int height = int.Parse(match.Groups["Height"].Value);

            return new ImageSize(width, height, name);
        }
    }
}
