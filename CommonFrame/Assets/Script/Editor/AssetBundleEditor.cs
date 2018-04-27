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
            //开始遍历Scenes下面的直接子文件夹W
            if (tmpFile is DirectoryInfo)
            {
                // Debug.Log("tmpFile is Dir and name is " + tmpFile.Name);
                //经测试，这里不能用Combine，得出的为\连接号，应该为/

                // string tmpPath = Path.Combine(path, tmpFile.Name);
                string tmpPath = path + "/" + tmpFile.Name;
                 //Debug.Log("tmpPath is " + tmpPath);
                SceneOverView(tmpPath);
            }

        }
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 遍历Art/Scenes下的直接子文件夹
    /// </summary>tmpPath is F:/unity/U3DCommonFrame/CommonFrame/Assets/Art/SceneOne
    /// <param name="scenePath"> F:/unity/U3DCommonFrame/CommonFrame/Assets/Art/Scenes/SceneOne</param>
    public static void SceneOverView(string scenePath)
    {
        string txtRecordName = "Record.txt";
        string tmpPath = scenePath + txtRecordName;
        FileStream fs = new FileStream(tmpPath, FileMode.OpenOrCreate);
        StreamWriter bw = new StreamWriter(fs);
        Dictionary<string, string> readDic = new Dictionary<string, string>();
        ChangerHead(scenePath, readDic);
        foreach(string key in readDic.Keys)
        {
            bw.Write(key);
            bw.Write(" ");
            bw.Write(readDic[key]);
            bw.Write("\n");
        }
        bw.Close();
        fs.Close();
    }

    /// <summary>
    /// 截取路径
    /// </summary>
    /// <param name="fullPath"> F:/unity/U3DCommonFrame/CommonFrame/Assets/Art/Scenes/SceneOne
    /// <param name="theWriter"></param>
    public static void ChangerHead(string fullPath,Dictionary<string,string> theWriter)
    {
        //查找Assets的第一个字符A在fullPath的位置索引值
        int tmpCnt = fullPath.IndexOf("Assets");
        //Debug.Log("fullPath is == " + fullPath);
        //Debug.Log("tmpCnt is ===" + tmpCnt);
        int tmpLen = fullPath.Length;

        string replacePath = fullPath.Substring(tmpCnt, tmpLen - tmpCnt);
        //replacePath == Assets/Art/Scenes/SceneOne
        //Debug.Log("replacePath is ==" + replacePath);
        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if(dir != null)
        {
            //Debug.Log("dir name is ==" + dir.Name);
            ListFiles(dir, replacePath, theWriter);
        }
        else
        {
            Debug.LogError("this path is not exist ==" + fullPath);
        }

    }
    /// <summary>
    /// 遍历每一个功能文件夹，得到每个文件夹下的文件
    /// </summary>
    /// <param name="replacePath">Assets/Scenes\SceneOne</param>
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
                //Debug.Log("file name is ==" + file.Name);
                ChangeMark(file, replacePath, theWriter);
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
    public static void ChangeMark(FileInfo tmpFile,string replacePath,Dictionary<string,string> theWriter)
    {
        //打Assetbundle包时，要忽略.meta文件
        if(tmpFile.Extension == ".meta")
        {
            return;
        }
        string markStr = GetBundlePath(tmpFile, replacePath);

        string fullPath = tmpFile.FullName;
        int assetCount = fullPath.IndexOf("Assets");
        string assetPath = fullPath.Substring(assetCount, fullPath.Length - assetCount);
        //getATPath需要的路径是Assets/XX/xx
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        assetImporter.assetBundleName = markStr;
        if (tmpFile.Extension == ".unity")
        {
            assetImporter.assetBundleVariant = "u3d";

        }
        else
        {
            assetImporter.assetBundleVariant = "ld";
        }
        //此处markStr == SceneOne/load Or SceneOne
        string modelName = string.Empty;
        string[] subMark = markStr.Split("/".ToCharArray());
        if(subMark.Length > 1)
        {
            modelName = subMark[1];
        }
        else
        {
            modelName = markStr;
        }
        //modelPath =  SceneOne / load.ld

        string modelPath = markStr.ToLower() + "." + assetImporter.assetBundleVariant;
        if (!theWriter.ContainsKey(modelName))
        {
            theWriter.Add(modelName,modelPath);
        }
    }
    //replacePath == Assets/Art/Scenes/SceneOne
    public static string GetBundlePath(FileInfo file,string replacePath)
    {
        //window系统路径格式
        //fullName == F:\unity\U3DCommonFrame\CommonFrame\Assets\Art\Scenes\SceneOne\Load\Foot\New Prefab.prefab
        //unity 路径是/art/scenesc
        string tmpPath = file.FullName;
        //Debug.Log("GetBundlePath is " + tmpPath);

        tmpPath = tmpPath.Replace("\\", "/");
        //转换后的  F:/unity/U3DCommonFrame/CommonFrame/Assets/Art/Scenes/SceneOne/Load/Foot/New Prefab.prefab


        //Debug.Log("GetBundlePath convert path is  " + tmpPath);
        //Debug.Log("GetBundlePath convert replacePath is  " + replacePath);



        int assetCnt = tmpPath.IndexOf(replacePath);
      
        assetCnt += replacePath.Length + 1;
        //Debug.Log("aSSETcANT 对应的字符是==" + tmpPath[assetCnt]);

        int nameCnt = tmpPath.LastIndexOf(file.Name);
      //  Debug.Log("nameCnt 对应的字符是==" + tmpPath[nameCnt]);
        int tmpCnt = replacePath.LastIndexOf("/");
      //  Debug.Log("tmpCnt 对应的字符是==" + replacePath[tmpCnt + 1]);
        string head = replacePath.Substring(tmpCnt + 1, replacePath.Length - tmpCnt - 1);
       //head == SceneOne

        int tmpLen = nameCnt - assetCnt;
        //说明SceneOne 后面还有文件夹
        if(tmpLen > 0)
        {
            string subString = tmpPath.Substring(assetCnt, tmpPath.Length - assetCnt);
            //subString =Load/Foot/New Prefab.prefab
            string[] res = subString.Split("/".ToCharArray());

            //Debug.Log("head is ====" + res.ToString());
            //for(int i = 0; i < res.Length; i++)
            //{
            //    Debug.Log("res is ==" + res[i]);
            //}
            return head + "/" + res[0];
        }
        else
        {
          //  Debug.Log("head is ==" + head);
            return head;
        }
       
    }

}
