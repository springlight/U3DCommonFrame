/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 14:11:37
 * 版本号：v1.0
 * 本类主要用途描述：
 * IABMgr 对一个场景所有的bundle包进行管理
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{

    public delegate void LoadAssetBundleCallBack(string sceneName, string bundleName);
    /// <summary>
    /// AssetObj是对一个UnityEngine.Object的封装
    ///一个bundle包里的Object
    ///一个bundle包里可能有一个Object
    ///也可能有多个Object
    /// </summary>
    public class AssetObj
    {
        public List<UnityEngine.Object> objs;

        public AssetObj(params UnityEngine.Object [] tmpObj)
        {
            objs = new List<UnityEngine.Object>();
            objs.AddRange(tmpObj);
        }
        public void Release()
        {
            for(int i = 0; i < objs.Count; i++)
            {
                Resources.UnloadAsset(objs[i]);
            }
        }
    }
    /// <summary>
    /// 存储一个bundle包里面的obj
    /// 比如，一个bundle包含，a,b,c三个Object
    /// </summary>
    public class AssetResObj
    {
        /// <summary>
        /// key:resName
        /// resObjs 结构就是("a",a),("b",b),("c",c)
        /// </summary>
        public Dictionary<string, AssetObj> resObjs;
        public AssetResObj(string resName, AssetObj tmp)
        {
            resObjs = new Dictionary<string, AssetObj>();
            resObjs.Add(resName, tmp);
        }

        public void AddResObj(string resName, AssetObj tmp)
        {
            resObjs.Add(resName, tmp);
        }

        public void Release(string resName)
        {
            if(resObjs.ContainsKey(resName))
            resObjs[resName].Release();
        }

        public void ReleaseAll()
        {
            List<string> keys = new List<string>();
            keys.AddRange(resObjs.Keys);
            for(int i = 0; i < keys.Count; i++)
            {
                Release(keys[i]);
            }
        }

        public List<UnityEngine.Object> GetResObj(string resName)
        {
            if (resObjs.ContainsKey(resName))
            {
                return resObjs[resName].objs;
            }
            return null;
        }

    }
    /// <summary>
    /// 对一个场景所有的bundle包进行管理
    /// </summary>
    public class IABMgr
    {
        /// <summary>
        /// 把每个一包都存起来
        /// key:bundleName
        /// value:当前bundle的依赖关系管理类
        /// </summary>
        private Dictionary<string, IABRelationMgr> loadHelper = new Dictionary<string, IABRelationMgr>();
        /// <summary>
        /// key :bundleName
        /// 存储一个bundle下所有的Object
        /// </summary>
        private Dictionary<string, AssetResObj> loadObjs = new Dictionary<string, AssetResObj>();


        public IABMgr(string sceneName)
        {
            this.sceneName = sceneName;
        }

        public void LoadAssetBundle(string bundle, LoadProgress loadProgress, LoadAssetBundleCallBack callBack)
        {
            if (!loadHelper.ContainsKey(bundle))
            {
                IABRelationMgr loader = new IABRelationMgr();
                loader.Init(bundle, loadProgress);
                loadHelper.Add(bundle, loader);
                callBack(sceneName, bundle);

            }
            else
            {
                Debug.Log("IABMgr has contains bundle name ==" + bundle);
            }
        }
        /// <summary>
        /// 加载assetbundle，必须先加载manifest,存在问题
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        /// 
        public IEnumerator LoadAssetBundle(string bundleName)
        {
            while (!IABManifestLoader.ins.IsLoadFinish())
            {
                yield return null;
            }

            //加载各种依赖关系
            IABRelationMgr loader;
            //if (loadHelper.ContainsKey(bundleName))
            //       loader = loadHelper[bundleName];
            //else
            //{
            //    loader = new IABRelationMgr();
            //    loader.Init(bundleName, null);
            //}
                loader = loadHelper[bundleName];
             string[] depences = GetDepedences(bundleName);
                loader.SetDepedences(depences);
                for (int i = 0; i < depences.Length; i++)
                {
                    yield return LoadAssetBundleDependences(depences[i], bundleName, loader.GetProgress());
                }

                yield return loader.LoadAssetBundle();
            
           
           
            
        }

        /// <summary>
        /// 循环加载依赖关系
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="refName"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetBundleDependences(string bundleName, string refName, LoadProgress progress)
        {
            if (!loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = new IABRelationMgr();
                loader.Init(bundleName, progress);
                if (refName != null)
                {
                    loader.AddRefference(refName);
                }
                loadHelper.Add(bundleName, loader);
                yield return LoadAssetBundle(bundleName);
            }
            else
            {
                if (refName != null)
                {
                    IABRelationMgr loader = loadHelper[bundleName];
                    loader.AddRefference(refName);

                }
            }
        }
        /// <summary>
        /// 是否加载了该bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public bool IsLoadedAssetBundle(string bundleName)
        {
            return false;
        }
        private string sceneName;

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="bundleName"></param>
        public void DebugAssetBundle(string bundleName)
        {
            if (loadHelper.ContainsKey(bundleName))
            {
                loadHelper[bundleName].DebugAsset();
            }
        }
       /// <summary>
       /// 是否加载完成
       /// </summary>
       /// <param name="bundleName"></param>
       /// <returns></returns>
        public bool IsLoadingFinish(string bundleName)
        {
            return false;
        }
        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <param name="bundleName">bundle名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public UnityEngine.Object GetSingleRes(string bundleName,string resName)
        {
            //是否已经缓存了物体
            if (loadObjs.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadObjs[bundleName];
                List<UnityEngine.Object> objs = tmpRes.GetResObj(resName);
                if (objs != null)
                    return objs[0];
            }
            ///表示已经加载了bundle
            if (loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = loadHelper[bundleName];

               UnityEngine.Object tmpObj = loader.GetSingleRes(resName);
               // UnityEngine.Object tmpObj = loader.GetSingleRes(resName);
                AssetObj tmpAssetObj = new AssetObj(tmpObj);
                //缓存里已经有了这个bundle包
                if (loadObjs.ContainsKey(bundleName))
                {
                    AssetResObj tmpRes = loadObjs[bundleName];
                    tmpRes.AddResObj(resName, tmpAssetObj);
            
                }
                else
                {
                    AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                    loadObjs.Add(bundleName, tmpRes);
                }
                return tmpObj;
            }
            return null;
        }

        public UnityEngine.Object [] GetMutilRes(string bundleName,string resName)
        {
            //是否已经缓存了物体
            if (loadObjs.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadObjs[bundleName];
                List<UnityEngine.Object> objs = tmpRes.GetResObj(resName);
                if (objs != null)
                    return objs.ToArray();
            }
            ///表示已经加载了bundle
            if (loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = loadHelper[bundleName];

                //UnityEngine.Object [] tmpObj = loader.GetMutilRes(bundleName);
                UnityEngine.Object [] tmpObj = loader.GetMutilRes(resName);
                AssetObj tmpAssetObj = new AssetObj(tmpObj);
                //缓存里已经有了这个包
                if (loadObjs.ContainsKey(bundleName))
                {
                    AssetResObj tmpRes = loadObjs[bundleName];
                    tmpRes.AddResObj(resName, tmpAssetObj);

                }
                else
                {
                    AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                    loadObjs.Add(bundleName, tmpRes);
                }
                return tmpObj;
            }
            return null;
        }

        /// <summary>
        /// 释放指定bundle包下的指定资源res
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        public void DisposeResObj(string bundleName,string resName)
        {
            if (loadObjs.ContainsKey(bundleName))
            {
                loadObjs[bundleName].Release(resName);
            }
        }
        /// <summary>
        /// 释放bundle下的所有资源
        /// </summary>
        /// <param name="bundleName"></param>
        public void DisposeResObj(string bundleName)
        {
            if (loadObjs.ContainsKey(bundleName))
            {
                loadObjs[bundleName].ReleaseAll();
            }
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 释放缓存的所有bundle下的所有资源
        /// </summary>
        public void DisposeAllObj()
        {
            List<string> keys = new List<string>();
            keys.AddRange(loadObjs.Keys);
            for(int i = 0; i < loadObjs.Count; i++)
            {
                DisposeResObj(keys[i]);
            }
            loadObjs.Clear();
        }

        /// <summary>
        /// 释放指定的bundle
        /// </summary>
        /// <param name="bundleName"></param>
        public void DisposeBundle(string bundleName)
        {
            if (loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = loadHelper[bundleName];
                //获取该bundle所有的依赖
                List<string> depences = loader.GetDepedences();
                for(int i = 0; i < depences.Count; i++)
                {
                    if (loadHelper.ContainsKey(depences[i]))
                    {
                        ///单个依赖关系，比如A 依赖B则当前获取的是B
                        IABRelationMgr dependence = loadHelper[depences[i]];
                        //因为B被A依赖，所有要删除B的被依赖关系A
                        if (dependence.RemoveRefference(bundleName))
                        {
                            DisposeBundle(dependence.GetBundleName());
                        }
                    }
                }
                //当前的bundle没有被其它bundle依赖的话，则直接释放该bundle
                if(loader.GetRefference().Count <= 0)
                {
                    loader.Dispose();
                    loadHelper.Remove(bundleName);
                }
            }
        }
        /// <summary>
        /// 释放所有的已经下载的bundle
        /// </summary>
        public void DisposeAllBundle()
        {
            foreach (string key in loadHelper.Keys)
            {
                loadHelper[key].Dispose();
                //   DisposeBundle(key);
            }
            loadHelper.Clear();
        }

        public void DisposeAllBundleAndRes()
        {
            DisposeAllObj();
            DisposeAllBundle();
        }
     
        private string [] GetDepedences(string bundleName)
        {
            return IABManifestLoader.ins.GetDepedences(bundleName);
        }
    }
    
}
