using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;

class Skia
{
    static readonly string[] latexes;

    static Skia()
    {
        using (StreamReader sr = new StreamReader("input.txt"))
        {
            latexes = sr.ReadToEnd().Split(Environment.NewLine);
        }
    }

    static readonly SKPaint redStroke = new SKPaint
    {
        Style = SKPaintStyle.Stroke,
        Color = SKColors.Red,
        StrokeWidth = .1f,
        IsAntialias = true
    };

    public static void Generate(string filename)
    {
        using (var stream = new SKFileWStream($"{filename}.pdf"))
        using (var document = SKDocument.CreatePdf(stream))
        {
            for (int i = 0; i < latexes.Length; i++)
            {
                var painter = new CSharpMath.SkiaSharp.MathPainter(12) { LaTeX = $@"\displaystyle {latexes[i]}" };
                var rect = painter.Measure.Value;
                using (var canvas = document.BeginPage(rect.Width, rect.Height))
                {
                    canvas.Translate(0, -rect.Top);
                    painter.Draw(canvas, new SKPoint(0, 0));
                    canvas.DrawRect(rect.Left, rect.Top, rect.Width, rect.Height, redStroke);
                    document.EndPage();
                }
            }
        }
    }

    private static void Main()
    {
        string filename = nameof(Skia);
        Generate(filename);
        using (Process p = new Process())
        {
            p.StartInfo.FileName = "pdflatex.exe";
            p.StartInfo.Arguments = "LaTeX.tex";
            p.Start();
            p.WaitForExit();
            p.StartInfo.Arguments = "Comparer.tex";
            p.Start();
            p.WaitForExit();
            /*
            p.StartInfo.FileName = "magick";
            p.StartInfo.Arguments = $"convert -density 100  Comparer.pdf Comparer-%02d.png";
            p.Start();
            p.WaitForExit();
            */
        }
    }
}

