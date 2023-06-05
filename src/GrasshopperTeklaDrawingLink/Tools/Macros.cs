namespace GTDrawingLink.Tools
{
    public class Macros
    {
        public static string LoadViewProperties(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros
    {{
        public sealed class Macro
        {{
            [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
            public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime)
            {{
                Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
                Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
                wpf.InvokeCommand(""CommandRepository"", ""View.Properties_Drawing"");
                akit.ValueChange(""view_dial"", ""gr_view_get_menu"", ""{attributeFileName}"");
                akit.PushButton(""view_modify"", ""view_dial"");
            }}
        }}
    }}
";
        }

    }
}
