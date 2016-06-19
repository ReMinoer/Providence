using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace Providence
{
    public class CommandRegistry : List<IProvidenceCommand>
    {
        public CommandRegistry()
        {
        }

        public void Load()
        {
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
                    if (!typeof(IProvidenceCommand).IsAssignableFrom(type))
                        return;

                    Add((IProvidenceCommand)Activator.CreateInstance(type));
                }
            }
        }
    }
}