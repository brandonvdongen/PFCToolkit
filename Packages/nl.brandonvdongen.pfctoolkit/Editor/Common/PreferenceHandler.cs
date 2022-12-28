using System.Collections.Generic;
using UnityEditor;

namespace PFCToolkit.Base {

    [InitializeOnLoad]
    public abstract class PreferenceHandler {
        public static Dictionary<string, PreferenceHandler> Preferences = new Dictionary<string, PreferenceHandler>();

        internal const string PREF_PREFIX = "PFCTOOLS2PREF";
        public string path = "pfc.unassigned";
        public string name = "pfc.unnamed";

        public PreferenceHandler(string SettingName, string SettingPath) {
            this.path = SettingPath;
            this.name = SettingName;
            Preferences.Add(SettingPath, this);
        }
    }
    public class BoolPreferenceHandler : PreferenceHandler {

        public bool cachedValue = false;
        public bool defaultValue = false;

        public BoolPreferenceHandler(string SettingName, string SettingPath, bool defaultValue = true) : base(SettingName, SettingPath) {
            this.defaultValue = defaultValue;
        }

        public bool IsEnabled {
            get { bool val = EditorPrefs.GetBool(PREF_PREFIX + path, defaultValue); cachedValue = val; return val; }
            set { EditorPrefs.SetBool(PREF_PREFIX + path, value); cachedValue = value; }
        }

        public void Toggle() {
            IsEnabled = !IsEnabled;
        }
    }

    public class FloatPreferenceHandler : PreferenceHandler {
        public float cachedValue = 0f;
        public float defaultValue = 0.5f;

        public FloatPreferenceHandler(string SettingName, string SettingPath, float defaultValue = 0.5f) : base(SettingName, SettingPath) {
            this.defaultValue = defaultValue;
        }

        public float Value {
            get { float val = EditorPrefs.GetFloat(PREF_PREFIX + path, defaultValue); cachedValue = val; return val; }
            set { EditorPrefs.SetFloat(PREF_PREFIX + path, value); cachedValue = value; }
        }
    }
}