using AngouriMath;
using CSharpMath.SkiaSharp;

namespace LaTexRendering
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = @"sqrt(sum((V-u)^2)/n)";
            string equation = ParseInput(input);

            MathPainter painter = new() { LaTeX = equation }; // or TextPainter
            using Stream? png = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png);
            using FileStream output = File.OpenWrite(args[0]);
            png.CopyTo(output);
        }

        #region Routines
        private static string ParseInput(string input)
        {
            Entity expr = MathS.FromString(input); // This gives us a tree structure
            Report(expr); // Analysis us
            return ConvertExpression(expr);
        }

        private static void Report(Entity expr)
        {
            var variables = GatherVariables(expr);
            Console.WriteLine($"Variables: {string.Join(", ", variables)}");
        }

        private static string ConvertExpression(Entity expr)
        {
            // E.g. @"\frac23"
            return expr.Latexise();
        }
        #endregion

        #region Equation Analysis
        public static string[] GatherVariables(Entity rootEntity)
        {
            List<string> variables = [];
            GatherFrom(variables, rootEntity);
            return variables.Distinct().ToArray();

            static void GatherFrom(List<string> container, Entity entity)
            {
                if (entity.Vars.Count() > 0)
                    container.AddRange(entity.Vars.Select(v => v.Name));
                if (entity.DirectChildren.Count > 0)
                {
                    foreach (var child in entity.DirectChildren)
                        GatherFrom(container, child);
                }
            }
        }
        #endregion
    }
}
