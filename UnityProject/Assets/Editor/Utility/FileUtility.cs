using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
public static class FileUtility {
    /// <summary>
    /// Base64编码
    /// </summary>
    public static string Encode(string message)
    {
        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(message);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Base64解码
    /// </summary>
    public static string Decode(string message)
    {
        byte[] bytes = Convert.FromBase64String(message);
        return Encoding.GetEncoding("utf-8").GetString(bytes);
    }

    /// <summary>
    /// 判断数字
    /// </summary>
    public static bool IsNumeric(string str)
    {
        if (str == null || str.Length == 0) return false;
        for (int i = 0; i < str.Length; i++)
        {
            if (!Char.IsNumber(str[i])) { return false; }
        }
        return true;
    }
    /// <summary>
    /// HashToMD5Hex
    /// </summary>
    public static string HashToMD5Hex(string sourceStr)
    {
        byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] result = md5.ComputeHash(Bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                builder.Append(result[i].ToString("x2"));
            return builder.ToString();
        }
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            fs.Dispose();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    /// <summary>
    /// 初始化文件旋转路经
    /// </summary>
    /// <param name="filePath"></param>
    public static void InitFileDiractory(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    public static void RecursiveSub(string path,string ignoreFileExt = ".meta", string ignorFolderEnd = "_files",Action<string> action = null)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(ignoreFileExt)) continue;
            action(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            if (dir.EndsWith(ignorFolderEnd)) continue;
            RecursiveSub(dir, ignoreFileExt, ignorFolderEnd, action);
        }
    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    public static void Recursive(string path,string fileExt, bool deep = true, Action<string> action = null)
    {
        string[] names = Directory.GetFiles(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.ToLower().Contains(fileExt.ToLower()))
            action(filename.Replace('\\', '/'));
        }
        if (deep)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                Recursive(dir, fileExt, deep, action);
            }
        }
     
    }
    /// <summary>
    /// 遍历目录及其子目录（支持共享目录）
    /// </summary>
    public static void Recursive(string path, string fileExt, bool deep = true, Action<FileInfo> action = null)
    {
        FileInfo[] files = new DirectoryInfo(@path).GetFiles();
        foreach (FileInfo file in files)
        {
            string ext = Path.GetExtension(file.ToString());
            if (ext.ToLower().Contains(fileExt.ToLower()))
                action(file);
        }
        if (deep)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(@path).GetDirectories();
            foreach (var dir in dirs)
            {
                Recursive(dir.ToString(), fileExt, deep, action);
            }
        }

    }

    /// <summary>
    //检查文件是否存在
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsFileExist(string filePath)
    {
        if (File.Exists(filePath))
        {
            return true;
        }
        return false;
    }

    public static string ConvertWindowsPath(string path)
    {
        return path.Replace("/", "\\");
    }

    public static string GetDemandDir(string fullPath)
    {
        string demandPath;
        if (!(fullPath.EndsWith(Path.DirectorySeparatorChar.ToString())
            || fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString())))
        {
            demandPath = fullPath + Path.DirectorySeparatorChar;
        }
        else
        {
            demandPath = fullPath;
        }

        return demandPath;
    }

    public static bool IsDirectorySeparator(char c)
    {
        return (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);
    }

    public static void CopyFile(string resourcePath,string targetPath)
    {
        if (string.Equals(resourcePath, targetPath)) return;
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
        File.Copy(resourcePath, targetPath,true);//如果没有写要覆盖程序会卡死不会继续运行
    }

    /// <summary>
    /// 保存字符串
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static void SaveTextAsset(string path, string text)
    {
        try
        {
            File.WriteAllText(path, text);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }
    /// <summary>
    /// 保存数组
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static void SaveByteAsset (string path, byte[] data)
    {
        try
        {
            File.WriteAllBytes(path, data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

}
