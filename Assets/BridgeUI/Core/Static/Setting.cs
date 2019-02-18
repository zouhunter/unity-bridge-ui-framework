using UnityEngine;
using System.Collections;

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
                    PlayerPrefs.Save();
                }
            }
        }

        private static string prefer_script_path = "bridgeui_setting_prefer_script_path";
        private static string _script_path;
        public static string script_path
        {
            get
            {
                if (string.IsNullOrEmpty(_script_path))
                {
                    if (!PlayerPrefs.HasKey(prefer_script_path))
                    {
                        _script_path = "";
                    }
                    else
                    {
                        _script_path = PlayerPrefs.GetString(prefer_script_path);
                    }
                }
                return _script_path;
            }
            set
            {
                if (_script_path != value)
                {
                    _script_path = value;
                    PlayerPrefs.SetString(prefer_script_path, _script_path);
                    PlayerPrefs.Save();
                }
            }
        }


        private static string prefer_defult_nameSpace = "bridgeui_setting_prefer_defult_nameSpace";
        private static string _defultNameSpace;
        public static string defultNameSpace
        {
            get
            {
                if (string.IsNullOrEmpty(_defultNameSpace))
                {
                    if (!PlayerPrefs.HasKey(prefer_defult_nameSpace))
                    {
                        _defultNameSpace = "View";
                    }
                    else
                    {
                        _defultNameSpace = PlayerPrefs.GetString(prefer_defult_nameSpace);
                    }
                }
                return _defultNameSpace;
            }
            set
            {
                if (_defultNameSpace != value)
                {
                    _defultNameSpace = value;
                    PlayerPrefs.SetString(prefer_defult_nameSpace, _defultNameSpace);
                    PlayerPrefs.Save();
                }
            }
        }

        private static string prefer_common_nameSpace = "bridgeui_setting_prefer_common_nameSpace";
        private static string _commonNameSpace;
        public static string commonNameSpace
        {
            get
            {
                if (string.IsNullOrEmpty(_commonNameSpace))
                {
                    if (!PlayerPrefs.HasKey(prefer_common_nameSpace))
                    {
                        _commonNameSpace = "Common";
                    }
                    else
                    {
                        _commonNameSpace = PlayerPrefs.GetString(prefer_common_nameSpace);
                    }
                }
                return _commonNameSpace;
            }
            set
            {
                if (_commonNameSpace != value)
                {
                    _commonNameSpace = value;
                    PlayerPrefs.SetString(prefer_common_nameSpace, _commonNameSpace);
                    PlayerPrefs.Save();
                }
            }
        }


        private static string prefer_user_name = "bridgeui_setting_prefer_user_name";
        private static string _userName;
        public static string userName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    if (!PlayerPrefs.HasKey(prefer_user_name))
                    {
                        _userName = "未指定用户，请在Preference中填写！";
                    }
                    else
                    {
                        _userName = PlayerPrefs.GetString(prefer_user_name);
                    }
                }
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    PlayerPrefs.SetString(prefer_user_name, _userName);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}