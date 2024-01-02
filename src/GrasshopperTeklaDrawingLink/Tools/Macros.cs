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

        public static string InsertPartMark(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {{
    public sealed class Macro {{
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {{
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            wpf.InvokeCommand(""CommandRepository"", ""Annotations.PartMarkProperties"");
            akit.ValueChange(""pmark_dial"", ""gr_pmark_get_menu"", ""{attributeFileName}"");
            akit.PushButton(""gr_pmark_get"", ""pmark_dial"");
            akit.PushButton(""pmark_apply"", ""pmark_dial"");
            akit.PushButton(""pmark_cancel"", ""pmark_dial"");
			
            akit.Callback(""acmdCreateAppliedMarksSelected"", """", ""View_10 window_1"");			
        }}
    }}
}}";
        }

        public static string InsertRebarMark(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {{
    public sealed class Macro {{
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {{
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            wpf.InvokeCommand(""CommandRepository"", ""Annotations.ReinforcementMarkProperties"");
            akit.ValueChange(""rebar_mark_dial"", ""gr_rebar_mark_get_menu"", ""{attributeFileName}"");
            akit.PushButton(""gr_rebar_get"", ""rebar_mark_dial"");
            akit.PushButton(""rebar_apply"", ""rebar_mark_dial"");
            akit.PushButton(""rebar_cancel"", ""rebar_mark_dial"");
			
            akit.Callback(""acmdCreateAppliedMarksSelected"", """", ""View_10 window_1"");			
        }}
    }}
}}";
        }

        public static string InsertBoltMark(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {{
    public sealed class Macro {{
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {{
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            wpf.InvokeCommand(""CommandRepository"", ""Annotations.BoltMarkProperties"");
            akit.ValueChange(""smark_dial"", ""gr_smark_get_menu"", ""{attributeFileName}"");
            akit.PushButton(""gr_smark_get"", ""smark_dial"");
            akit.PushButton(""smark_apply"", ""smark_dial"");
            akit.PushButton(""smark_cancel"", ""smark_dial"");
			
            akit.Callback(""acmdCreateAppliedMarksSelected"", """", ""View_10 window_1"");			
        }}
    }}
}}";
        }

        public static string InsertWeldMark(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {{
    public sealed class Macro {{
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {{
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            wpf.InvokeCommand(""CommandRepository"", ""Annotations.WeldMarkProperties"");
            akit.ValueChange(""wld_dial"", ""gr_wld_get_menu"", ""{attributeFileName}"");
            akit.PushButton(""gr_wld_get"", ""wld_dial"");
            akit.PushButton(""wld_apply"", ""wld_dial"");
            akit.PushButton(""wld_cancel"", ""wld_dial"");
			
            akit.Callback(""acmdCreateAppliedMarksSelected"", """", ""View_10 window_1"");			
        }}
    }}
}}";
        }

        public static string InsertPourMark(string attributeFileName)
        {
            return $@"
#pragma warning disable 1633 // Unrecognized #pragma directive
#pragma reference ""Tekla.Macros.Wpf.Runtime""
#pragma reference ""Tekla.Macros.Akit""
#pragma reference ""Tekla.Macros.Runtime""
#pragma warning restore 1633 // Unrecognized #pragma directive

namespace UserMacros {{
    public sealed class Macro {{
        [Tekla.Macros.Runtime.MacroEntryPointAttribute()]
        public static void Run(Tekla.Macros.Runtime.IMacroRuntime runtime) {{
            Tekla.Macros.Akit.IAkitScriptHost akit = runtime.Get<Tekla.Macros.Akit.IAkitScriptHost>();
            Tekla.Macros.Wpf.Runtime.IWpfMacroHost wpf = runtime.Get<Tekla.Macros.Wpf.Runtime.IWpfMacroHost>();
            wpf.InvokeCommand(""CommandRepository"", ""Annotations.PourMarkProperties"");
            akit.ValueChange(""pour_mark_dial"", ""gr_pour_mark_get_menu"", ""{attributeFileName}"");
            akit.PushButton(""gr_pour_mark_get"", ""pour_mark_dial"");
            akit.PushButton(""pour_mark_apply"", ""pour_mark_dial"");
            akit.PushButton(""pour_mark_cancel"", ""pour_mark_dial"");
			
            akit.Callback(""acmdCreateAppliedMarksSelected"", """", ""View_10 window_1"");			
        }}
    }}
}}";
        }
    }
}
