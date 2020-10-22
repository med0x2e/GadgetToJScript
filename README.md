
## Description
A tool for generating .NET serialized gadgets that can trigger .NET assembly load/execution when deserialized using BinaryFormatter from JS/VBS/VBA scripts.
<br>The current gadget triggers a call to Activator.CreateInstance() when deserialized using BinaryFormatter from jscript/vbscript/vba, this means it can be used to trigger execution of your .NET assembly of choice via the default/public constructor.
<br><br>The tool was created mainly for automating WSH scripts weaponization for RT engagements (Initial Access, Lateral Movement, Persistence), the shellcode loader which was used for PoC is removed and replaced by an example assembly implemented in the TestAssembly project.


## Details:
- Bypasses AMSI without having to update AmsiEnable registry key or Hijack loadlibrary (AMSI.dll).
- Bypasses .NET 4.8+ newly introduced controls for blocking "Assembly.Load" and ActivitySurrogateSelector Type checks.
- Gadget supports proper deserialzation of payloads targetting .NET Framework 3.5 up to 4.8+ environments.
- Depends on ActivitySurrogateSelector to create a Surrogate class which act as a wrapper to deserialize a gadget built in a way to trigger a call to "Activator.CreateInstance(Assembly.Load(your_assembly_bytes).GetType())".
- Leveraging TextFormattingRunProperties based gadget as a first deserialization stage to disable ActivitySurrogateSelector Type check therefore bypassing fixes introduced recently in (.NET Framework 4.8+)
- Doesn't rely mainly on exposing a .NET based COM object hence no need to call d.DynamicInvoke(al.ToArray()).CreateInstance(entry_class)
- Delegates are used only to trigger payload execution during deserialization (Func<Assembly, IEnumerable<Type>>),a requirement for proper chaining of the gadget elements.
- Serialized gadgets or Streams length is calculated at runtime and automatically populated in the generated WSH scripts (Not required for VBA).
- Generates VBS/VBA and JS/HTA scripts relying on registration-free activation of .NET based COM components

## Usage & Notes:
You can either use the binaries located in the Release folder or use VS2017 and retarget your project to .NET version 3.5 or 4.x in order to build G2JS from source.
* Use the .NET 3.5 version of G2JS to:
	* Generate WSH scripts targeted to run in environments where .NET Framework 4.x is not available or less than 4.8. (3.5 is the minimal required .NET Framework version), This requires to build your C# payload or .NET Assembly with .NET framework <= 3.5.
* Use the .NET 4.x version of G2JS to:
	* Generate WSH scripts meant to run in environments where .NET Framework 4.x is available. (Includes 4.8+)
	* Generate WSH scripts for environments with .NET 4.8+ using <b>“--bypass”</b> option to bypass type checking.
  
* C# source code payload should be placed within the default public constructor of your class. (Example of running Mimikatz from jscript; https://gist.github.com/med0x2e/cc10d42b1f581507013e801da2651c74)
* Tests were conducted on the latest windows 10 Enterprise 1909 release and Windows 7 with different .NET framework versions installed.
* Make sure Windows Defender is turned off when using GadgetToJScript.exe to generate WSH scripts, can
be turned back on once scripts are generated.

|||
|----|----|
| ``-w, --scriptType=js`` |  ``Set to js, vbs, vba or hta`` |
|``-a, --assembly=TestAssembly.dll`` | ``.NET Assembly, example: -a C:\Users\userX\Desktop\shellcode_loader.dll/exe, alternatively you can specify a c# source file instead using -c cmdline switch.`` |
|``-c, --csfile=TestAssembly.cs`` |  ``C# source code file, example: -c C:\Users\userX\Desktop\shellcode_loader.cs, make sure to place your code within the default constructor of your class and specify any required dependencies using -d cmdline switch.`` |
|``-d, --references=Example.dll``  |  ``Reference Assemblies, example: -d System.Window- s.Forms.dll, System.dll`` |
|``-b, --bypass``  |  ``Bypass type check controls introduced in .NET version 4.8+, by default set to false, set to true (--bypass/-b) in case WSH scripts are being generated to run on .NET version > 4.8+ environments. this option should be used only with .NET 4.x G2JS executable`` |
|``-e, --encodeType=b64``  |  ``VBA gadgets encoding: b64 or hex (default set to b64)`` |
|``-o, --output=output``  |  ``Generated payload output file, example: -o C:\Users\userX\Desktop\output (Without extension)`` |
|``-r, --regfree`` | ``registration-free activation of .NET based COM components, applicable to JS/HTA scripts only.``  |
|``-h, --help``  |  ``Show Help`` |

## OPSEC Tip:
Use the .NET 3.5 version of G2JS to generate WSH Registration-free JScript/HTA scripts which can still bypass .NET framework 4.8+ type checking without having to disable such mitigation using the first ``TextFormattingRunProperties`` stage gadget. consider this as a <u>**better/cleaner**</u> bypass to execute Reg-free JScript/HTA payloads targeting .NET 4.8+ environments as it does not require:
 * The first stage gadge
 * Creating a <i>``Shell``</i> object
 * Reading the current .NET framework version from the registry
 * Setting the <i>``COMPLUS_Version``</i> environment variable.
 
-> less IOCs + targetting 3.5 < .NET < 4.x Envs.

## Credits & References
The tool is based on the awesome research/work done by:
- @tyranid https://googleprojectzero.blogspot.com/2017/04/ => made serializing/deserializing unserializable classes possible)
- @pwntester yoserial.net project https://github.com/pwntester/ysoserial.net
- @olekmirosh https://community.microfocus.com/t5/Security-Research-Blog/New-NET-deserialization-gadget-for-compact-payload-When-size/ba-p/1763282
- @monoxgas https://silentbreaksecurity.com/re-animating-activitysurrogateselector/
- @zcgonvh http://www.zcgonvh.com/post/weaponizing_CVE-2020-0688_and_about_dotnet_deserialize_vulnerability.html

## Disclaimer
GadgetToJScript should be used for authorized red teaming and/or nonprofit educational purposes only.
Any misuse of this software will not be the responsibility of the author.
Use it at your own networks and/or with the network owner's permission.
