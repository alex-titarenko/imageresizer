﻿using System;
using System.Text.RegularExpressions;


namespace TAlex.ImageProxy
{
    public struct ImageSize
    {
        private static Regex ImageSizeRegex = new Regex(@"(?<Width>\d+)x(?<Height>\d+)", RegexOptions.Compiled);


        public const string OriginalImageSize = "Original";


        public readonly int Width;

        public readonly int Height;

        public readonly string Name;

        public ImageSize(int width, int height)
            : this(width, height, null)
        {
        }

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
            else
            {
                return Name;
            }
        }

        public static ImageSize Parse(string s)
        {
            return Parse(s, null);
        }

        public static ImageSize Parse(string s, string name)
        {
            if (String.Equals(s, OriginalImageSize, StringComparison.OrdinalIgnoreCase) ||                
                String.Equals(name, OriginalImageSize, StringComparison.OrdinalIgnoreCase))
            {
                return new ImageSize(-1, -1, OriginalImageSize);
            }

            Match match = ImageSizeRegex.Match(s);

            if (!match.Success)
            {
                throw new FormatException(String.Format("Data {0} is not correct  Image size value", s));
            }

            int width = int.Parse(match.Groups["Width"].Value);
            int height = int.Parse(match.Groups["Height"].Value);

            return new ImageSize(width, height, name);
        }
    }
}