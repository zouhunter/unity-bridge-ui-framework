using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace AssetBundleBuilder
{
    //public interface IBulidCtrl
    //{
    //    void BuildProjectsAssetBundle(string path, BuildAssetBundleOptions option, BuildTarget target, bool record);
    //    void BuildSelectAssets(string abName, string path, UnityEngine.Object[] obj, BuildTarget target);
    //}

    public class BulidController /*: IBulidCtrl*/
    {
        //private List<string> bundlefiles = new List<string>();

       
        //void GetTextFile(string buildpath)
        //{
        //    //string bundleText = buildpath + "/bundles.txt";
        //    MD5TableParse.GetMD5CSV(buildpath, AppFixed.BundleCSV);
        //    if (File.Exists(bundleText)) File.Delete(bundleText);
        //    bundlefiles.Clear();
        //    Recursive(buildpath);
        //    FileStream fs = new FileStream(bundleText, FileMode.CreateNew);
        //    StreamWriter sw = new StreamWriter(fs);
        //    for (int i = 0; i < this.bundlefiles.Count; i++)
        //    {
        //        string file = this.bundlefiles[i];
        //        //string ext = Path.GetExtension(file);
        //        if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

        //        string md5 = FileUtility.md5file(file);
        //        string value = "";
        //        /////从本地读取文件有问题
        //        value = file.Replace(buildpath + "/", string.Empty);

        //        sw.WriteLine(value + "|" + md5);
        //    }
        //    sw.Close(); fs.Close();
        //    AssetDatabase.Refresh();
        //}

        ///// <summary>
        ///// 遍历目录及其子目录
        ///// </summary>
        //void Recursive(string path)
        //{
        //    string[] names = Directory.GetFiles(path);
        //    string[] dirs = Directory.GetDirectories(path);
        //    foreach (string filename in names)
        //    {
        //        string ext = Path.GetExtension(filename);
        //        if (ext.Equals(".meta")) continue;
        //        bundlefiles.Add(filename.Replace('\\', '/'));
        //    }
        //    foreach (string dir in dirs)
        //    {
        //        Recursive(dir);
        //    }
        //}
    }
}

