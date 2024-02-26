using AngouriMath;
using CSharpMath.SkiaSharp;

namespace EquationBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.First() == "--help")
            {
                Console.WriteLine("""
                    EquationBuilder --help: Print this help
                    EquationBuilder <Expression>: Generate outputs based on expression

                    Example: 
                      EquationBuilder sqrt(sum((V-u)^2)/n)
                    """);
                return;
            }
            // Check correct number of arguments
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Incorrect number of arguments");
                return;
            }

            string inputExpression = args[0];
            string outputPath = "Equation.png";
            GenerateFromExpression(inputExpression, outputPath);
        }

        private static void GenerateFromExpression(string inputExpression, string outputPath)
        {
            // Parse inputs
            string equation = ParseInput(inputExpression);

            // Get LaTeX
            // ...

            // Generate rendering
            MathPainter painter = new() { LaTeX = equation }; // or TextPainter
            using Stream png = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png)!;
            using FileStream output = File.OpenWrite(outputPath);
            png.CopyTo(output);

            // Generate metadata output
            // ...
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
