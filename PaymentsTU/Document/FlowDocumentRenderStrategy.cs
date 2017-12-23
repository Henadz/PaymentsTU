using DocumentModel;
using DocumentModel.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace PaymentsTU.Document
{
	internal class FlowDocumentRenderStrategy : IRenderStrategy
	{
		public void Render(IEnumerable<ProcessingChunk> items, Stream stream, Encoding encoding)
		{
			var document = new FlowDocument();

			foreach (var chunk in items)
			{
				var section = new Section();
				foreach (var artifact in chunk.Artifacts)
				{
					/*if (artifact is NewPage)
					{
						if (page == null)
						{
							page = CreateNewPage();
						}
						else
						{
							pages.Add(ToPageContent(page));
							page = CreateNewPage();
						}
					}
					else*/
					if (artifact is DocumentModel.Paragraph)
					{
						RenderParagraph((DocumentModel.Paragraph)artifact, section);
					}
				}

				document.Blocks.Add(section);
			}
		}

		private void RenderParagraph(DocumentModel.Paragraph artifact, Section section)
		{
			var par = new System.Windows.Documents.Paragraph();
			par.TextAlignment = ToWindowsAligment(artifact.Alignment);

			//foreach (var block in artifact.TextBlocks)
			//{
			//	var inline = new Inline();
			//}

			//par.Inlines.AddRange(new Inline())
		}
		private TextAlignment ToWindowsAligment(ParagraphAlignment alignment)
		{
			switch (alignment)
			{
				case ParagraphAlignment.Right:
					return TextAlignment.Right;
				case ParagraphAlignment.Justify:
					return TextAlignment.Justify;
				case ParagraphAlignment.Center:
					return TextAlignment.Center;
				case ParagraphAlignment.Left:
				default:
					return TextAlignment.Left;
			}
		}

		public void Setup(ProcessingModel model)
		{
			throw new NotImplementedException();
		}
	}
}
