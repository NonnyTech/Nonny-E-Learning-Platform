using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class CertificateDocument : IDocument
	{
		private readonly string _studentName;
		private readonly string _courseTitle;
		private readonly string _completionDate;
		private readonly string _logoPath;
		public CertificateDocument(string studentName, string courseTitle, string completionDate, string logoPath)
		{

			_studentName = studentName;
			_courseTitle = courseTitle;
			_completionDate = completionDate;
			_logoPath = logoPath;
		}
		public void Compose(IDocumentContainer container)
		{
			container.Page(page =>
			{
				page.Margin(50);
				page.Size(PageSizes.A4);
				page.PageColor(Colors.White);
				page.DefaultTextStyle(x => x.FontSize(16).FontColor(Colors.Black));

				page.Content()
					.Column(column =>
					{
						column.Spacing(15);

						// Logo
						if (File.Exists(_logoPath))
						{
							var logoBytes = File.ReadAllBytes(_logoPath);

							column.Item().AlignCenter().Width(150).Height(50).Image(logoBytes);
						}

						// Title
						column.Item().AlignCenter().Text("Certificate of Completion")
							.FontSize(30).Bold().FontColor(Colors.Black);

						// Subtitle
						column.Item().AlignCenter().Text("This is proudly awarded to")
							.FontSize(18).Italic().FontColor(Colors.Grey.Darken1);

						// Student Name
						column.Item().AlignCenter().Text(_studentName)
							.FontSize(24).Bold().FontColor(Colors.Blue.Medium);

						// Course Completed
						column.Item().AlignCenter().Text("for successfully completing the course:")
							.FontSize(18).FontColor(Colors.Black);

						column.Item().AlignCenter().Text(_courseTitle)
							.FontSize(22).Bold().FontColor(Colors.Green.Darken1);

						// Completion Date
						column.Item().AlignCenter().Text($"Completed on {_completionDate}")
							.FontSize(16).Italic().FontColor(Colors.Grey.Darken1);

						// Spacer
						column.Item().PaddingVertical(40);

						// Signatures
						column.Item().Row(row =>
						{
							row.RelativeItem().Column(col =>
							{
								col.Item().Text("Director's Signature: ______________________")
									.FontSize(14).Italic();
							});

							row.RelativeItem().Column(col =>
							{
								col.Item().Text("Instructor's Signature: ______________________")
									.FontSize(14).Italic().AlignRight();
							});
						});

						// Footer
						column.Item().AlignCenter().Column(col =>
						{
							col.Item().Text("NonnyPlus E-Learning System").FontSize(14).Bold();
						});
					});
			});
		}
	}
}
