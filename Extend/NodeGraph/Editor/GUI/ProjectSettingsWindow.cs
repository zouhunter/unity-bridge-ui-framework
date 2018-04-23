//using UnityEngine;
//using UnityEditor;

//using System;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Collections;
//using System.Collections.Generic;

//using Model=NodeGraph.DataModel;

//namespace NodeGraph {
//	public class ProjectNGSettingsWindow : EditorWindow {

//        [SerializeField] private string m_cacheDir;

//        //[MenuItem(NGSettings.GUI_TEXT_MENU_PROJECTWINDOW_OPEN, false, 41)]
//		public static void Open () {
//            GetWindow<ProjectNGSettingsWindow>();
//		}

//        private void Init() {
//			LogUtility.Logger.filterLogType = LogType.Warning;
//			this.titleContent = new GUIContent("Project NGSettings");
//			this.minSize = new Vector2(300f, 100f);
//			this.maxSize = new Vector2(1000f, 400f);
//		}

//        public void OnEnable () {
//			Init();
//		}

//		public void OnFocus() {
//		}

//		public void OnDisable() {
//		}

//        public string DrawFolderSelector(string label, 
//            string dialogTitle, 
//            string currentDirPath, 
//            string directoryOpenPath, 
//            Func<string, string> onValidFolderSelected = null) 
//        {
//            string newDirPath;
//            using(new EditorGUILayout.HorizontalScope()) {
//                if (string.IsNullOrEmpty (label)) {
//                    newDirPath = EditorGUILayout.TextField(currentDirPath);
//                } else {
//                    newDirPath = EditorGUILayout.TextField(label, currentDirPath);
//                }

//                if(GUILayout.Button("Select", GUILayout.Width(50f))) {
//                    var folderSelected = 
//                        EditorUtility.OpenFolderPanel(dialogTitle, directoryOpenPath, "");
//                    if(!string.IsNullOrEmpty(folderSelected)) {
//                        if (onValidFolderSelected != null) {
//                            newDirPath = onValidFolderSelected (folderSelected);
//                        } else {
//                            newDirPath = folderSelected;
//                        }
//                    }
//                }
//            }
//            return newDirPath;
//        }

//		public void OnGUI () {

//			using (new EditorGUILayout.VerticalScope()) {

//				GUILayout.Label("Project NGSettings", new GUIStyle("BoldLabel"));
//                GUILayout.Space(8f);

//                string cacheDir = Application.dataPath;

//                using (new EditorGUILayout.HorizontalScope ()) {                    
//                    var newCacheDir = DrawFolderSelector ("Bundle Cache Directory", "Select Cache Folder", 
//                        cacheDir,
//                        Application.dataPath + "/../",
//                        (string folderSelected) => {
//                            var projectPath = Directory.GetParent(Application.dataPath).ToString();

//                            if(projectPath == folderSelected) {
//                                folderSelected = string.Empty;
//                            } else {
//                                var index = folderSelected.IndexOf(projectPath);
//                                if(index >= 0 ) {
//                                    folderSelected = folderSelected.Substring(projectPath.Length + index);
//                                    if(folderSelected.IndexOf('/') == 0) {
//                                        folderSelected = folderSelected.Substring(1);
//                                    }
//                                }
//                            }
//                            return folderSelected;
//                        }
//                    );
//                    if (newCacheDir != cacheDir) {
//                        //NGSettings.UserNGSettings.AssetBundleBuildCacheDir = newCacheDir;
//                    }
//                }
//                EditorGUI.BeginDisabledGroup(!Directory.Exists(cacheDir));
//                //using (new EditorGUI.DisabledScope (!Directory.Exists (cacheDir))) 
//                {
//                    using (new EditorGUILayout.HorizontalScope ()) {
//                        GUILayout.FlexibleSpace ();
//                        #if UNITY_EDITOR_OSX
//                        string buttonName = "Reveal in Finder";
//                        #else
//                        string buttonName = "Show in Explorer";
//                        #endif
//                        if (GUILayout.Button (buttonName)) {
//                            EditorUtility.RevealInFinder (cacheDir);
//                        }
//                    }
//                }
//                EditorGUI.EndDisabledGroup();
//			}
//		}
//	}
//}
