using System;

namespace DocumentModel
{
    public sealed class SvgBlock : TextBlock
    {
        /// <summary>
        /// Used for serialization
        /// </summary>
        internal SvgBlock()
        {
        }

        public string Svg { get; private set; }

        public int? Height { get; set; }
        public int? Width { get; set; }

        public SvgBlock(string svg) : this(svg, string.Empty)
        {
        }

        public SvgBlock(string svg, string text) : base(text)
        {
            if (string.IsNullOrEmpty(svg.Trim())) throw new ArgumentNullException("svg");
            Svg = svg;
        }

        public SvgBlock(string svg, FormatedText text)
            : base(text)
        {
            if (string.IsNullOrEmpty(svg.Trim())) throw new ArgumentNullException("svg");
            Svg = svg;
        }
    }
}
