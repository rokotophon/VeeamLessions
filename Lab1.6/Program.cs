using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lab1._6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter an expression:");
            var exp = Console.ReadLine();

            var x = GetCorrectValue();

            //var language = "C#";
            //if (!CodeDomProvider.IsDefinedLanguage(language))
            //    throw new Exception($"\"{language}\" isn't defined language");

            //CompilerParameters compileParams = new CompilerParameters()
            //{
            //    GenerateExecutable = false,
            //    OutputAssembly = "Temp.dll",
            //    IncludeDebugInformation = true,
            //    GenerateInMemory = false,
            //    TreatWarningsAsErrors = false,
            //    CompilerOptions = "/optimize"
            //};
            //compileParams.ReferencedAssemblies.AddRange(new[] { "System.dll", "CalcLib.dll"});

            //var provider = CodeDomProvider.CreateProvider(language);
            //if (provider.Supports(GeneratorSupport.EntryPointMethod))
            //    compileParams.MainClass = "CalcHolder.MakeCalc";

            var sourceCode =
@"using CalcLib;
class CalcHolder
    {  
    public static double MakeCalc(double x)  
    {
        return " + 
        exp.Replace("MyExp", "Calc.MyExp")
        .Replace("MyLog", "Calc.MyLog")
        .Replace("MyAbs", "Calc.MyAbs") +
@";
    }  
    }  
";
            var name = "Temp.dll";
            using (var peStream = new MemoryStream())
            {
                var compile = GenerateCode(name, sourceCode);
                var result = compile.Emit(peStream);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation errors:");

                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

                    return;
                }

                Console.WriteLine("Compilation success.");

                using (var file = File.OpenWrite(name))
                {
                    file.Write(peStream.ToArray());
                }
            }

            AppDomain domain;
            try
            {
                domain = AppDomain.CreateDomain("CalcDomain");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                domain = AppDomain.CurrentDomain;
            }
            var ass = domain.Load(File.ReadAllBytes(name));
            var calc = ass.GetType("CalcHolder").GetTypeInfo().DeclaredMethods.First();
            var c = calc.Invoke(null, new[] { (object)x });
            Console.WriteLine(c);
        }

        private static double GetCorrectValue()
        {
            var x = double.NaN;
            while(true)
            {
                Console.WriteLine("Enter the X value:");
                var str = Console.ReadLine();
                if (double.TryParse(str, out x))
                    return x;
                Console.WriteLine($"Error: \"{str}\" isn't a numerical value!");
            }
        }

        private static CSharpCompilation GenerateCode(string assemblyName, string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile("CalcLib.dll"),
            };

            return CSharpCompilation.Create(assemblyName,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }


    }
}
