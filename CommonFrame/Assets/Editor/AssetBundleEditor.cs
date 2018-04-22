using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class AssetBundleEditor
{

    [MenuItem("ITools/BuildAssetBundle")]
    public static void BuildAssetBundle()
    {
        string outPath = Application.dataPath + "/AssetBundle";
        //待研究打包API
        BuildPipeline.BuildAssetBundles(outPath, 0, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    [MenuItem("ITools/MarkAssetBundle")]
    public static void MarkAssetBundle()
    {
        //
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //要遍历的文件夹
        string path = Application.dataPath + "/Art/Scenes";
       // Debug.Log("path ===" + Application.dataPath);

        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] fileInfor = dir.GetFileSystemInfos(); 

        for(int i = 0; i < fileInfor.Length; i++)
        {
           // Debug.Log("fileInforor " + i + "= " + fileInfor[i] );
            FileSystemInfo tmpFile = fileInfor[i];
            //开始遍历Scenes下面的直接子文件夹
            if(tmpFile is DirectoryInfo)
            {
                
               // Debug.Log("tmpFile is Dir and name is " + tmpFile.Name);
                string tmpPath = Path.Combine(path, tmpFile.Name);
               // Debug.Log("tmpPath is " + tmpPath);
                SceneOverView(tmpPath);
            }

        }
    }
    /// <summary>
    /// 遍历Art/Scenes下的直接子文件夹
    /// </summary>
    /// <param name="scenePath"></param>
    public static void SceneOverView(string scenePath)
    {
        string txtRecordName = "Record.txt";
        string tmpPath = scenePath + txtRecordName;
        FileStream fs = new FileStream(tmpPath, FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);
        Dictionary<string, string> readDic = new Dictionary<string, string>();
        ChangerHead(scenePath, readDic);
        bw.Close();
        fs.Close();
    }

    /// <summary>
    /// 截取路径
    /// </summary>
    /// <param name="fullPath">D:/ToluFuish/Assets/Art/Scenes/SceneOne</param>
    /// <param name="theWriter"></param>
    public static void ChangerHead(string fullPath,Dictionary<string,string> theWriter)
    {
        int tmpCnt = fullPath.IndexOf("Assets");
        Debug.Log("fullPath is == " + fullPath);
        Debug.Log("tmpCnt is ===" + tmpCnt);
        int tmpLen = fullPath.Length;

        string replacePath = fullPath.Substring(tmpCnt, tmpLen - tmpCnt);
        Debug.Log("replacePath is ==" + replacePath);
        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if(dir != null)
        {
            ListFiles(dir, replacePath, theWriter);
        }
        else
        {
            Debug.LogError("this path is not exist ==" + fullPath);
        }

    }
    /// <summary>
    /// 遍历每一个功能文件夹
    /// </summary>
    /// <param name="replacePath">Assets/Scenes/SceneOne</param>
    /// <param name="theWrite"></param>
    public static void ListFiles(FileSystemInfo info,string replacePath,Dictionary<string,string> theWriter)
    {
        DirectoryInfo dir = info as DirectoryInfo;

        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for(int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            //对于文件操作，直接改变其AssetBundleName
            if(file != null)
            {
                SetAssetBundleName(file, replacePath, theWriter);
            }
            else//如果是文件夹，递归调用每个目录里面的文件
            {
                ListFiles(files[i], replacePath, theWriter);
            }
        }
    }
    /// <summary>
    /// 给每一个文件改变assetbundle
    /// </summary>
    public static void SetAssetBundleName(FileInfo tmpFile,string replacePath,Dictionary<string,string> theWriter)
    {
        if(tmpFile.Extension == ".meta")
        {
            return;
        }
        string assetNameStr = GetBundlePath(tmpFile, replacePath);
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetNameStr);
        assetImporter.assetBundleName = replacePath;
        if(tmpFile.Extension == ".unity")
        {
            assetImporter.assetBundleVariant = "u3d";

        }
        else
        {
            assetImporter.assetBundleVariant = "ld";
        }
        //string [] subMark = replacePath
    }

    public static string GetBundlePath(FileInfo file,string replacePath)
    {
        //window系统路径格式
        //fullName == e:\\tmp\\test.text
        //unity 路径是/art/scenesc
        string tmpPath = file.FullName;
        tmpPath = tmpPath.Replace("\\", "/");

        //replacePath == Assets/Art/SceneOne

        int assetCnt = tmpPath.IndexOf(replacePath);
        assetCnt += replacePath.Length + 1;

        int nameCnt = tmpPath.LastIndexOf(file.Name);
        int tmpCnt = replacePath.LastIndexOf("/");
        string head = replacePath.Substring(tmpCnt + 1, replacePath.Length - tmpCnt - 1);
        int tmpLen = nameCnt - assetCnt;
        if(tmpLen > 0)
        {
            string subString = tmpPath.Substring(assetCnt, tmpPath.Length - assetCnt);
            string[] res = subString.Split("/".ToCharArray());
            return head + "/" + res[0];
        }
        else
        {
            return head;
        }
       
    }

}
