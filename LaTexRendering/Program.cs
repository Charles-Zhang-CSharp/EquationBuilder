using AngouriMath;
using CSharpMath.SkiaSharp;

namespace LaTexRendering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine()!;
            string equation = ParseInput(input);

            MathPainter painter = new MathPainter { LaTeX = equation }; // or TextPainter
            using Stream? png = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png);
            using FileStream output = File.OpenWrite(args[0]);
            png.CopyTo(output);
        }

        private static string ParseInput(string input)
        {
            // E.g. @"\frac23"
            Entity expr = MathS.FromString(input); // This gives us a tree structure
            return @"\frac23";
        }
    }
}
