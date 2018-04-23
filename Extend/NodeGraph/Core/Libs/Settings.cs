using System.IO;
using UnityEngine;

namespace NodeGraph
{
    public class NGSettings
    {
        /*
			if true, ignore .meta files inside NodeGraph.DataModel.
		*/
        public const bool IGNORE_META = true;



        public const string GUI_TEXT_SETTINGTEMPLATE_MODEL = "Model";
        public const string GUI_TEXT_SETTINGTEMPLATE_AUDIO = "Audio";
        public const string GUI_TEXT_SETTINGTEMPLATE_TEXTURE = "Texture";
        public const string GUI_TEXT_SETTINGTEMPLATE_VIDEO = "Video";

        public const string UNITY_METAFILE_EXTENSION = ".meta";
        public const string DOTSTART_HIDDEN_FILE_HEADSTRING = ".";
        public const string MANIFEST_FOOTER = ".manifest";
        public const char UNITY_FOLDER_SEPARATOR = '/';// Mac/Windows/Linux can use '/' in Unity.
        public const char KEYWORD_WILDCARD = '*';



        public const float WINDOW_SPAN = 20f;

        public const string GROUPING_KEYWORD_DEFAULT = "/Group_*/";
        public const string BUNDLECONFIG_BUNDLENAME_TEMPLATE_DEFAULT = "bundle_*";

        // by default, NodeGraph.DataModel's node has only 1 InputPoint. and 
        // this is only one definition of it's label.
        public const string BASE64_IDENTIFIER = "B64|";
        public const string BUNDLECONFIG_BUNDLE_OUTPUTPOINT_LABEL = "bundles";
        public const string BUNDLECONFIG_VARIANTNAME_DEFAULT = "";

        public const string DEFAULT_FILTER_KEYWORD = "";
        public const string DEFAULT_FILTER_KEYTYPE = "Any";

        public const string FILTER_KEYWORD_WILDCARD = "*";

        public const string NODE_INPUTPOINT_FIXED_LABEL = "FIXED_INPUTPOINT_ID";

        public static string GRAPH_SEARCH_CONDITION { get { return "t:" + typeof(NodeGraph.DataModel.NodeGraphObj).FullName; } }
    }
}
