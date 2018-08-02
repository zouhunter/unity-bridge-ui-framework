using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BridgeUI
{
    public static class Setting
    {
        private static string prefer_bundle_name_format = "bridgeui_setting_prefer_bundle_name_format";
        private static string _bundleNameformat;
        public static string bundleNameFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_bundleNameformat))
                {
                    if (!PlayerPrefs.HasKey(prefer_bundle_name_format))
                    {
                        _bundleNameformat = "bridgeui/panels/{0}.assetbundle";
                    }
                    else
                    {
                        _bundleNameformat = PlayerPrefs.GetString(prefer_bundle_name_format);
                    }
                }
                return _bundleNameformat;
            }
            set
            {
                if (_bundleNameformat != value)
                {
                    _bundleNameformat = value;
                    PlayerPrefs.SetString(prefer_bundle_name_format, _bundleNameformat);
                }
            }
        }
    }
}