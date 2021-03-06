﻿using FrameworkExtend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace DocumentModel
{
	public sealed class UrlBlock : IBlock
    {
        /// <summary>
        /// Used for serialization
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        internal UrlBlock()
        {
        }

        public UrlBlock(Uri uri)
            : this(uri, new FormattedText(""))
        {
        }

        public UrlBlock(Uri uri, [Localizable(false)] string text, Color color)
            : this(uri, new FormattedText(text))
        {
            Color = color;
        }

        public UrlBlock(Uri uri, [Localizable(false)] string text)
            : this(uri, new FormattedText(text))
        {
        }

        public UrlBlock(Uri uri, FormattedText text)
            : this(uri, new[] { text })
        {
        }

        public UrlBlock(Uri uri, IEnumerable<FormattedText> text)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (text == null)
                throw new ArgumentNullException("text");

            var lst = text.ToList();

            Url = uri;

            if (FormattedText.IsPlainEmpty(lst))
                TextBlocks = new ReadOnlyCollection<TextBlock>(new[] { new TextBlock(Url.ToString()) });
            else
                TextBlocks = new ReadOnlyCollection<TextBlock>(new List<TextBlock>(lst.Select(t => new TextBlock(t))));
        }

        public Uri Url { get; private set; }
        public ReadOnlyCollection<TextBlock> TextBlocks { get; private set; }
        public Color? Color { get; private set; }
    }
}