using OfficeOpenXml.Interfaces.Drawing.Text;
using SkiaSharp;

namespace EverrichFakeCrawler.Interfaces
{
    internal class SkiaSharpTextMeasurer : ITextMeasurer
    {
        private SKFontStyle ToSkFontStyle(MeasurementFontStyles style)
        {
            switch (style)
            {
                case MeasurementFontStyles.Regular:
                    return SKFontStyle.Normal;
                case MeasurementFontStyles.Bold:
                    return SKFontStyle.Bold;
                case MeasurementFontStyles.Italic:
                    return SKFontStyle.Italic;
                case MeasurementFontStyles.Bold | MeasurementFontStyles.Italic:
                    return SKFontStyle.BoldItalic;
                default:
                    return SKFontStyle.Normal;
            }
        }

        public TextMeasurement MeasureText(string text, MeasurementFont font)
        {
            var skFontStyle = ToSkFontStyle(font.Style);
            var tf = SKTypeface.FromFamilyName(font.FontFamily, skFontStyle);
            using (var paint = new SKPaint())
            {
                paint.TextSize = font.Size;
                paint.Typeface = tf;
                var rect = SKRect.Empty;
                paint.MeasureText(text.AsSpan(), ref rect);
                // The scaling factors below are needed to translate the Skia measurements to Excel column widths.
                return new TextMeasurement(rect.Width * 2, rect.Height);
            }

        }

        public bool ValidForEnvironment()
        {
            // you can use this method to check if your text measurer
            // works in the current environment.
            return true;
        }
    }
}
