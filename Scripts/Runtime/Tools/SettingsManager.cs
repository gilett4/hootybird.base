//@vadym udod

using hootybird.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Managers
{
    public class SettingsManager : RoutinesBase
    {
        public static Action<string> onValueChanged;
        public static Action<string, bool> onBoolChanged;
        public static Action<string, int> onIntChanged;
        public static Action<string, float> onFloatChanged;

        public const string PREFIX = "";

        private static Dictionary<string, SettingValueWrapper> values = 
            new Dictionary<string, SettingValueWrapper>();

        public static bool ContainsKey(string key) => values.ContainsKey(key);

        public static void Set(string key, int value)
        {
            if (!values.ContainsKey(key))
                AddValue(key, value);
            else
                values[key].IntValue = value;

            onValueChanged?.Invoke(key);
            onIntChanged?.Invoke(key, value);
        }

        public static void Set(string key, float value)
        {
            if (!values.ContainsKey(key))
                AddValue(key, value);
            else
                values[key].FloatValue = value;

            onValueChanged?.Invoke(key);
            onFloatChanged?.Invoke(key, value);
        }

        public static void Set(string key, bool value)
        {
            if (!values.ContainsKey(key))
            {
                AddValue(key, value);
                Set(key, value);

                return;
            }
            else
                values[key].IntValue = value ? 1 : 0;

            onValueChanged?.Invoke(key);
            onBoolChanged?.Invoke(key, value);
        }

        public static void Toggle(string key) => Set(key, values[key].BoolValue ? 0 : 1);

        public static int GetInt(string key) => values[key].IntValue;

        public static float GetFloat(string key) => values[key].FloatValue;

        public static bool GetBool(string key) => GetInt(key) == 1;

        public static void AddValue(string key, int defaultValue)
        {
            if (values.ContainsKey(key)) return;

            values.Add(key, new SettingValueWrapper() { key = key, defaultIntValue = defaultValue });
        }

        public static void AddValue(string key, float defaultValue)
        {
            if (values.ContainsKey(key)) return;

            values.Add(key, new SettingValueWrapper() { key = key, defaultFloatValue = defaultValue });
        }

        public static void AddValue(string key, bool value) => AddValue(key, value ? 1 : 0);

        protected class SettingValueWrapper
        {
            internal bool isFloatValue = false;

            internal string key;

            internal int defaultIntValue;
            internal float defaultFloatValue;

            internal float FloatValue
            {
                get => PlayerPrefs.GetFloat(PREFIX + key, defaultFloatValue);
                set
                {
                    if (!isFloatValue) isFloatValue = true;

                    PlayerPrefs.SetFloat(PREFIX + key, value);
                }
            }

            internal int IntValue
            {
                get => PlayerPrefs.GetInt(PREFIX + key, defaultIntValue);
                set => PlayerPrefs.SetInt(PREFIX + key, value);
            }

            internal bool BoolValue
            {
                get => IntValue == 1;
                set => IntValue = value ? 1 : 0;
            }
        }
    }
}
