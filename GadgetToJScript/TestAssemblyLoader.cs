using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace GadgetToJScript
{
    class TestAssemblyLoader
    {
        public static Assembly compile()
        {
            // Shellcode loader would make more sense here, just make sure your code is located within the default constructor.
            string _testClass = @"
                    
                using System;
                using System.Runtime.InteropServices;

                    public class TestClass
                    {
                        " + "[DllImport(\"User32.dll\", CharSet = CharSet.Unicode)]" +
                        @"public static extern int MessageBox(IntPtr h, string m, string c, int t);

                        public TestClass(){
                            " + "MessageBox((IntPtr)0, \"Test .NET Assembly Constructor Called.\", \"Coolio\", 0);" +
                        @"}
                    }
            
            ";

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("System.dll");


            CompilerResults results = provider.CompileAssemblyFromSource(parameters, _testClass);

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
        }

    }
}
