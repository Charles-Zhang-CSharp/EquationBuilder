using AngouriMath;
using CSharpMath.SkiaSharp;
using System.Text;

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
                    EquationBuilder <Expression> <Output Folder> <Output Name>: Generate outputs based on expression

                    Example: 
                      EquationBuilder sqrt(sum((V-u)^2)/n)
                    """);
                return;
            }
            // Check correct number of arguments
            if (args.Length != 3)
            {
                Console.WriteLine("Error: Incorrect number of arguments");
                return;
            }

            string inputExpression = args[0];
            string outputPath = Path.GetFullPath(args[1]);
            string outputName = args[2];
            Directory.CreateDirectory(outputPath);

            GenerateFromExpression(inputExpression, outputPath, outputName);
        }

        private static void GenerateFromExpression(string inputExpression, string outputPath, string outputName)
        {
            // Parse inputs
            string equation = ParseInput(inputExpression, out Entity expressionEntity);

            // Get LaTeX
            File.WriteAllText(Path.Combine(outputPath, $"{outputName}.txt"), equation);

            // Generate rendering
            MathPainter painter = new() { LaTeX = equation }; // or TextPainter
            using Stream png = painter.DrawAsStream(format: SkiaSharp.SKEncodedImageFormat.Png)!;
            using FileStream output = File.OpenWrite(Path.Combine(outputPath, $"{outputName}.png"));
            png.CopyTo(output);

            // Generate metadata output
            string report = AnalyzeExpression(expressionEntity);
            File.WriteAllText(Path.Combine(outputPath, $"{outputName}.yaml"), report);
        }

        #region Routines
        private static string ParseInput(string input, out Entity expressionEntity)
        {
            expressionEntity = MathS.FromString(input); // This gives us a tree structure
            return ConvertExpression(expressionEntity);
        }

        private static string AnalyzeExpression(Entity expr)
        {
            StringBuilder builder = new();
            var variables = GatherVariables(expr);
            builder.AppendLine($"Variables: {string.Join(", ", variables)}");
            return builder.ToString().TrimEnd();
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
