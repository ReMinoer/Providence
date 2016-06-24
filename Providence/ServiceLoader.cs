using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace Providence
{
    static public class ServiceLoader
    {
        static public IEnumerable<ProvidenceService> Load()
        {
            var result = new List<ProvidenceService>();

            string myDocumentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string scriptFolder = Path.Combine(myDocumentPath, "Providence Scripts\\");

            if (!Directory.Exists(scriptFolder))
                Directory.CreateDirectory(scriptFolder);

            string[] assemblies = AppDomain.CurrentDomain
                             .GetAssemblies()
                             .Where(a => !a.IsDynamic)
                             .Select(a => a.Location)
                             .ToArray();

            foreach (string file in Directory.EnumerateFiles(scriptFolder, "*.cs"))
            {
                var cSharpCodeProvider = new CSharpCodeProvider();
                var compilerParameters = new CompilerParameters(assemblies)
                {
                    GenerateInMemory = false,
                    GenerateExecutable = false
                };

                CompilerResults compilerResults = cSharpCodeProvider.CompileAssemblyFromFile(compilerParameters, file);

                foreach (Type type in compilerResults.CompiledAssembly.ExportedTypes)
                {
                    if (!typeof(ProvidenceService).IsAssignableFrom(type))
                        continue;

                    result.Add((ProvidenceService)Activator.CreateInstance(type));
                }
            }

            return result;
        }
    }
}