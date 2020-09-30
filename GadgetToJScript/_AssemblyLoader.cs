using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GadgetToJScript
{
    class _AssemblyLoader
    {

        public static Assembly load(String path) {
            try{
                Assembly _assembly = Assembly.LoadFrom(path);
                return _assembly;

            }catch(BadImageFormatException ex) {
                Console.WriteLine("Error _AssemblyLoader: " + ex.Message + " : " + ex.StackTrace);
                return null;
            }
            catch (Exception exG){
                Console.WriteLine("Error _AssemblyLoader: " + exG.Message + " : " + exG.StackTrace);
                return null; 
            }
            
        }
        public static Assembly compile(String csFile, String references)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                parameters.ReferencedAssemblies.AddRange(references.Split(new[] { ',' }).Select(reference => reference).ToArray());

                CompilerResults results = provider.CompileAssemblyFromFile(parameters, csFile);

                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(String.Format("Error ({0}): {1}: {2}", error.ErrorNumber, error.ErrorText, error.Line));
                    }

                    throw new InvalidOperationException(sb.ToString());
                }

                Assembly _compiled = results.CompiledAssembly;

                return _compiled;
            }catch(Exception ex)
            {
                Console.WriteLine("Error _AssemblyLoader: " + ex.Message + " : " + ex.StackTrace);
                return null;
            }
        }

    }
}
