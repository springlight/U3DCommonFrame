/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/10 18:33:49
 * 版本号：v1.0
 * 本类主要用途描述：
 * 总的模块管理者，继承自Mono
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Script.Assets
{
    public class ILoaderMgr:MonoBehaviour
    {
        public static ILoaderMgr ins;
        public IABSceneMgr sceneMgr;
        /// <summary>
        /// key :sceneName
        /// </summary>
        private Dictionary<string, IABSceneMgr> loadMgr = new Dictionary<string, IABSceneMgr>();
        private void Awake()
        {
            ins = this;
            //第一步，加载IABManifest,唯一一个Manifest
            StartCoroutine(IABManifestLoader.ins.LoadManifest());
            //第二步，读取场景文件
          
        }
        //第二步，读取配置场景文件
        public void ReadConfiger(string sceneName)
        {
            if (!loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = new IABSceneMgr(sceneName);
                tmpMgr.ReadConfiger(sceneName);
                loadMgr.Add(sceneName, tmpMgr);
            }
        }

        public void LoadAsset(string sceneName,string bundleName,LoadProgress progress)
        {
            if (!loadMgr.ContainsKey(sceneName))
            {
                ReadConfiger(sceneName);
            }
           
            IABSceneMgr tmpMgr = loadMgr[sceneName];
            tmpMgr.LoadAsset(bundleName, progress, LoadCallback);
          
        }

        public void LoadCallback(string sceneName,string bundleName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                StartCoroutine(tmpMgr.LoadAssetSys(bundleName));
            }
            else
            {
                Debug.LogError("");
            }
        }


        #region 由下层提供
        public UnityEngine.Object GetSingleRes(string sceneName,string bundleName,string resName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                return tmpMgr.GetSingleRes(bundleName, resName);
            }
            return null;
        }

        public UnityEngine.Object []  GetMutilRes(string sceneName, string bundleName, string resName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                return tmpMgr.GetMutilRes(bundleName, resName);
            }
            return null;
        }
        /// <summary>
        /// 释放资源
        /// 释放某一个资源
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        public void UnLoadResObj(string sceneName,string bundleName,string resName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
            //    tmpMgr.DisposeBundleRes
                 tmpMgr.DisposeResObj(bundleName, resName);
            }
        }
        /// <summary>
        /// 释放整个包
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="bundleName"></param>
        public void UnLoadBundleResObjs(string sceneName,string bundleName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DisposeBundleRes(bundleName);
               
            }
        }
        /// <summary>
        /// 释放整个场景加载的object
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnLoadAllRes(string sceneName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DisposeAllRes();

            }
        }
        /// <summary>
        /// 释放一个bundle
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="bundleName"></param>
        public void UnLoadAssetBundle(string sceneName,string bundleName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DisposeBundle(bundleName);

            }
        }

        public void UnLoadAllAssetBundle(string sceneName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DisposeAllbundle();
                //资源一下子回收太多，主动释放内存
                System.GC.Collect();

            }
        }
        /// <summary>
        /// 释放一个场景的全部bundle和object
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnLoadAllAssetbundleAndResObj(string sceneName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DisposeAllBundleAndRes();
                //资源一下子回收太多，主动释放内存
                System.GC.Collect();

            }
        }

        private void OnDestroy()
        {
            loadMgr.Clear();
            System.GC.Collect();
        }

        public void DebugAllAssetBundle(string sceneName)
        {
            if (loadMgr.ContainsKey(sceneName))
            {
                IABSceneMgr tmpMgr = loadMgr[sceneName];
                tmpMgr.DebugAllAsset();
                //资源一下子回收太多，主动释放内存
                System.GC.Collect();

            }
        }
        #endregion

    }
}
