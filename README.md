## Description
A tool for generating .NET serialized gadgets that can trigger .NET assembly load/execution when deserialized using BinaryFormatter from JS/VBS based scripts.
<br>The gadget being used triggers a call to Assembly.Load when deserialized via jscript/vbscript, this means it can be used in the same way to trigger in-memory load of your own shellcode loader at runtime.
<br><br> Lastly, the tool was created mainly for automating WSH scripts weaponization for RT engagements (LT, Persistence), the shellcode loader which was used for PoC is removed and replaced by an example assembly implemented in the "TestAssemblyLoader.cs" class for PoC purpose.

## TLDR
- Generates js, vbs, hta, vba.
- Bypasses AMSI without having to update AmsiEnable registry key or Hijack loadlibrary (AMSI.dll), basically it is more of a signature based bypass at the moment.
- Bypasses .NET 4.8+ newly introduced controls for blocking "Assembly.Load"

## Details
- Leverages ActivitySurrogateSelector to create a Surrogate class which act as a wrapper to deserialize a gadget built in a way to trigger a call to "Activator.CreateInstance(Assembly.Load(your_assembly_bytes).GetType())".
- Bypasses AMSI (to be specific, bypasses AMSI signature based detection) => Does not require "d.DynamicInvoke(al.ToArray()).CreateInstance(entry_class)"
- Leveraging TextFormattingRunProperties based gadget as a first deserialization stage to disable ActivitySurrogateSelector Type check therefore bypassing fixes introduced recently in (.NET Framework 4.8+)
- Doesn't rely mainly on exposing a .NET based COM object hence no need to call d.DynamicInvoke(al.ToArray()).CreateInstance(entry_class)
- Delegates are being used only to trigger payload execution during deserialization (Func<Assembly, IEnumerable<Type>>),a requirement for proper chaining of the gadget elements.
- Serialized gadgets or Streams length is calculated at runtime and automatically populated in the generated WSH scripts.
- Generates JS/HTA scripts relying on registration-free activation of .NET based COM components, may help in case of generating VBA or registering unregistered COM objects. (In the future may be, not a requirement unless you wanna avoid "New ActiveXObject" for AV evasion or want to use another alternative of BinaryFormatter which requires registration )

## Usage: 
  ``-w, --scriptType=VALUE     js, vbs, vba or hta``<br>
  ``-o, --output=VALUE         Generated payload output file, example:
                               C:\Users\userX\Desktop\output (Without extension)``<br>
  ``-r, --regfree              registration-free activation of .NET based COM
                               components``<br>
  ``-h, --help=VALUE           Show Help``
  
## Testing Notes
- Tested with visual studio 2017 - x86 build option.
- Tested with multiple shellcode loaders, try using either C:\windows\syswow64\cscript.exe or C:\windows\system32\cscript.exe to run generated js/vbs scripts, it all depends on which Arch (x86/x64) used for build and shellcode generation.
- Make sure Windows Defender is turned off when using GadgetToJScript.exe to generate WSH scripts, can 
be turned on once scripts are generated.
## Credits & References
The tool is based on the awesome research/work done by:
- @tyranid https://googleprojectzero.blogspot.com/2017/04/ => made serializing/deserializing unserializable classes possible)
- @pwntester yoserial.net project https://github.com/pwntester/ysoserial.net
- @olekmirosh https://community.microfocus.com/t5/Security-Research-Blog/New-NET-deserialization-gadget-for-compact-payload-When-size/ba-p/1763282
- @monoxgas https://silentbreaksecurity.com/re-animating-activitysurrogateselector/

## Disclaimer
GadgetToJScript should be used for authorized red teaming and/or nonprofit educational purposes only. 
Any misuse of this software will not be the responsibility of the author. 
Use it at your own networks and/or with the network owner's permission.

