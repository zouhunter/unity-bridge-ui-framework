using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace NodeGraph
{
    public class ScriptableObjUtility
    {
        /// <summary>
        /// Adds the specified hidden subAssets to the mainAsset
        /// </summary>
        public static void SetSubAssets(ScriptableObject[] subAssets, ScriptableObject mainAsset,bool clearOther = false)
        {
            var path = AssetDatabase.GetAssetPath(mainAsset);
            var oldAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (ScriptableObject subAsset in subAssets)
            {
                if (subAsset == mainAsset) continue;

                if (System.Array.Find(oldAssets, x => x == subAsset) == null)
                {
                    AddSubAsset(subAsset, mainAsset);
                }
            }
            if(clearOther)
            {
                foreach (var item in oldAssets)
                {
                    if (item == mainAsset) continue;

                    if (System.Array.Find(subAssets, x => x == item) == null)
                    {
                        Object.DestroyImmediate(item, true);
                    }
                }
            }
            //Debug.Log("[SetSubAssets] " + mainAsset +" subassets Updated.");
        }

        /// <summary>
        /// Adds the specified hidden subAsset to the mainAsset
        /// </summary>
        private static void AddSubAsset(ScriptableObject subAsset, ScriptableObject mainAsset)
        {
            if (subAsset != null && mainAsset != null)
            {
                UnityEditor.AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
            }
        }


        /// <summary>
        /// Adds the specified hidden subAsset to the mainAsset at path
        /// </summary>
        //public static void AddSubAsset(ScriptableObject subAsset, string path)
        //{
        //    if (subAsset != null && !string.IsNullOrEmpty(path))
        //    {
        //        UnityEditor.AssetDatabase.AddObjectToAsset(subAsset, path);
        //        subAsset.hideFlags = HideFlags.HideInHierarchy;
        //    }
        //}
    }
}