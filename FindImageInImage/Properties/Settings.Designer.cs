﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FindImageInImage.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//ClixSenseFinds//")]
        public string ClickSenseFindDir {
            get {
                return ((string)(this["ClickSenseFindDir"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//ClixSenseClose//")]
        public string ClickSenseCloseDir {
            get {
                return ((string)(this["ClickSenseCloseDir"]));
            }
            set {
                this["ClickSenseCloseDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//NemoFind//")]
        public string NemoFindDir {
            get {
                return ((string)(this["NemoFindDir"]));
            }
            set {
                this["NemoFindDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//NemoDots//")]
        public string NemoDotDir {
            get {
                return ((string)(this["NemoDotDir"]));
            }
            set {
                this["NemoDotDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".//NemoCloses//")]
        public string NemoCloseDir {
            get {
                return ((string)(this["NemoCloseDir"]));
            }
            set {
                this["NemoCloseDir"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ClickSenseScreenIndex {
            get {
                return ((int)(this["ClickSenseScreenIndex"]));
            }
            set {
                this["ClickSenseScreenIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int NemoScreenIndex {
            get {
                return ((int)(this["NemoScreenIndex"]));
            }
            set {
                this["NemoScreenIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int MegaScreenIndex {
            get {
                return ((int)(this["MegaScreenIndex"]));
            }
            set {
                this["MegaScreenIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("45")]
        public int ScreenZoomFactorX {
            get {
                return ((int)(this["ScreenZoomFactorX"]));
            }
            set {
                this["ScreenZoomFactorX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("27")]
        public int ScreenZoomFactorY {
            get {
                return ((int)(this["ScreenZoomFactorY"]));
            }
            set {
                this["ScreenZoomFactorY"] = value;
            }
        }
    }
}
