using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetLoaderOption : Editor {

        [MenuItem("AssetBundle/AssetBundleLoader/Simulation")]
        static void SetSimulation()
        {
            AssetBundleLoader.SimulateAssetBundleInEditor = !AssetBundleLoader.SimulateAssetBundleInEditor;
        }
        [MenuItem("AssetBundle/AssetBundleLoader/Simulation", true)]
        static bool SetSimuLationEnable()
        {
            Menu.SetChecked("AssetBundle/AssetBundleLoader/Simulation", AssetBundleLoader.SimulateAssetBundleInEditor);
            return true;
        }

    }
