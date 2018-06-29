
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/10 18:43:01
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Common
{
    public class IPathTools
    {
        public static string GetPlatformFloderName(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Window";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                
            }
            return null;
        }

        public static string GetAppFilePath()
        {
            string path;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                path = Application.streamingAssetsPath;
            }
            else
            {
                path = Application.persistentDataPath;
            }
            return path;
        }
       public static string GetAssetBundlePath()
        {
            string platFolder = GetPlatformFloderName(Application.platform);
            // string allPath = Path.Combine(GetAppFilePath(), platFolder);
            string allPath = GetAppFilePath() + "/" + platFolder;
            return allPath;
        }

        public static string GetWWWAssetBundlePath()
        {
            string tmpStr = "";
            if(Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.OSXEditor)
            {
                tmpStr = "file:///" + GetAssetBundlePath();
            }
            else
            {
                string tmpPath = GetAssetBundlePath();
#if UNITY_ANDROID
                tmpStr = "jar:file://"+tmpPath;
#elif UNITY_STANDALONE_WIN
                tmpStr = "file:///" + tmpPath;
#else
                tmpStr = "file://"+tmpPath;
#endif

            }
            return tmpStr;
        }
    }
}
