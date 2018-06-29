/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/10 18:33:21
 * 版本号：v1.0
 * 本类主要用途描述：
 * 管理一个场景下的的Assetbundle
 *  -------------------------------------------------------------------------*/

using Assets.Script.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    
   public  class IABSceneMgr
    {
        private IABMgr abMgr;
        private Dictionary<string, string> allAsset = new Dictionary<string, string>();
        public IABSceneMgr(string sceneName)
        {
            abMgr = new IABMgr(sceneName);
        }
        /// <summary>
        /// 给商城ILoaderMgr调用
        /// </summary>
        /// <param name="sceneName"></param>
        public void ReadConfiger(string sceneName)
        {
            string txtFileName = "Record.txt";
            string path = IPathTools.GetAssetBundlePath() + "/" + sceneName + txtFileName;
      //     string path = Application.persistentDataPath +"/AssetBundle/" + sceneName + txtFileName;
            ReadConfig(path);
        }


        private void ReadConfig(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader br = new StreamReader(fs);
            ///读第一行
            string line = br.ReadLine();
            int allCnt = int.Parse(line);
            for(int i = 0; i <allCnt; i++)
            {
                string tmpStr = br.ReadLine();
                string [] arr = tmpStr.Split(" ".ToCharArray());
                allAsset.Add(arr[0], arr[1]);
            }
            br.Close();
            fs.Close();
        }

        public void LoadAsset(string bundleName ,LoadProgress progress,LoadAssetBundleCallBack callBack)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                string tmpValue = allAsset[bundleName];
                abMgr.LoadAssetBundle(tmpValue, progress, callBack);
            }
            else
            {
                Debug.LogError("lOADaSSET ERROR");
            }
        }

        public string GetBundleReateName(string bundleName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                return allAsset[bundleName];
            }
            return null;
        }
        public IEnumerator LoadAssetSys(string bundleName)
        {
            yield return abMgr.LoadAssetBundle(bundleName);
        }

        public UnityEngine.Object GetSingleRes(string bundleName,string resName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                return abMgr.GetSingleRes(allAsset[bundleName], resName);
            }
            else
            {
                return null;
            }
        }

        public UnityEngine.Object [] GetMutilRes(string bundleName,string resName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                return abMgr.GetMutilRes(allAsset[bundleName], resName);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 释放单个资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        public void DisposeResObj(string bundleName,string resName)
        {

            if (allAsset.ContainsKey(bundleName))
            {
                abMgr.DisposeResObj(allAsset[bundleName], resName);
            }
            else
            {
                
            }
        }

        public void DisposeBundleRes(string bundleName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                abMgr.DisposeResObj(allAsset[bundleName]);
            }
            else
            {

            }
        }

        public void DisposeAllRes()
        {
            abMgr.DisposeAllObj();
        }

        public void DisposeBundle(string bundleName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
               // abMgr.DisposeBundle(allAsset[bundleName]);
                abMgr.DisposeBundle(bundleName);
            }
            else
            {

            }
        }
         
        public void DisposeAllBundleAndRes()
        {
            abMgr.DisposeAllBundleAndRes();
            allAsset.Clear();
        }

        public void DisposeAllbundle()
        {
            abMgr.DisposeAllBundle();
            allAsset.Clear();
        }

        public void DebugAllAsset()
        {
            foreach(string key in allAsset.Keys)
            {
                abMgr.DebugAssetBundle(allAsset[key]);
            }
        }
        /// <summary>
        /// sceneone/test.ld bundleNmae = test
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool IsLoadBundleFinisn(string bundleName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                return abMgr.IsLoadingFinish(allAsset[bundleName]);
            }
            else
            {
                Debug.LogError("is not contain bundle ==" + bundleName);

            }
            return false;
        }
        public bool IsLoadingAssetBundle(string bundleName)
        {
            if (allAsset.ContainsKey(bundleName))
            {
                return abMgr.IsLoadedAssetBundle(allAsset[bundleName]);
            }
            else
            {
                Debug.LogError("is not contain bundle ==" + bundleName);

            }
            return false;
        }


    }
    
}
