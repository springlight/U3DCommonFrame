/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 14:11:37
 * 版本号：v1.0
 * 本类主要用途描述：
 * 对一个场景所有的bundle包进行管理
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    /// <summary>
    ///单个物体，有可能多个
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
    /// </summary>
    public class AssetResObj
    {
        /// <summary>
        /// key:resName
        /// </summary>
        public Dictionary<string, AssetObj> resObjs;
        public AssetResObj(string name, AssetObj tmp)
        {
            resObjs = new Dictionary<string, AssetObj>();
            resObjs.Add(name, tmp);
        }

        public void AddResObj(string name,AssetObj tmp)
        {
            resObjs.Add(name, tmp);
        }

        public void Release(string name)
        {
            if(resObjs.ContainsKey(name))
            resObjs[name].Release();
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

        public List<UnityEngine.Object> GetResObj(string name)
        {
            if (resObjs.ContainsKey(name))
            {
                return resObjs[name].objs;
            }
            return null;
        }

    }
    public class IABMgr
    {
        /// <summary>
        /// 把每个一包都存起来
        /// </summary>
        private Dictionary<string, IABRelationMgr> loadHelper = new Dictionary<string, IABRelationMgr>();
        /// <summary>
        /// key :bundleName
        /// </summary>
        private Dictionary<string, AssetResObj> loadObj = new Dictionary<string, AssetResObj>();
        /// <summary>
        /// 是否加载了该bundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public bool IsLoadedAssetBundle(string bundleName)
        {
            return false;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="bundleName"></param>
        public void DebugAssetBundle(string bundleName)
        {

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
        /// 
        /// </summary>
        /// <param name="bundleName">bundle名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public UnityEngine.Object GetSingleRes(string bundleName,string resName)
        {
            //是否已经缓存了物体
            if (loadObj.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadObj[bundleName];
                List<UnityEngine.Object> objs = tmpRes.GetResObj(resName);
                if (objs != null)
                    return objs[0];
            }
            ///表示已经加载了bundle
            if (loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = loadHelper[bundleName];

                UnityEngine.Object tmpObj = loader.GetSingleRes(bundleName);
                AssetObj tmpAssetObj = new AssetObj(tmpObj);
                //缓存里已经有了这个包
                if (loadObj.ContainsKey(bundleName))
                {
                    AssetResObj tmpRes = loadObj[bundleName];
                    tmpRes.AddResObj(resName, tmpAssetObj);
            
                }
                else
                {
                    AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                    loadObj.Add(bundleName, tmpRes);
                }
                return tmpObj;
            }
            return null;
        }

        public UnityEngine.Object [] GetMutilRes(string bundleName,string resName)
        {
            //是否已经缓存了物体
            if (loadObj.ContainsKey(bundleName))
            {
                AssetResObj tmpRes = loadObj[bundleName];
                List<UnityEngine.Object> objs = tmpRes.GetResObj(resName);
                if (objs != null)
                    return objs.ToArray();
            }
            ///表示已经加载了bundle
            if (loadHelper.ContainsKey(bundleName))
            {
                IABRelationMgr loader = loadHelper[bundleName];

                UnityEngine.Object [] tmpObj = loader.GetMutilRes(bundleName);
                AssetObj tmpAssetObj = new AssetObj(tmpObj);
                //缓存里已经有了这个包
                if (loadObj.ContainsKey(bundleName))
                {
                    AssetResObj tmpRes = loadObj[bundleName];
                    tmpRes.AddResObj(resName, tmpAssetObj);

                }
                else
                {
                    AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
                    loadObj.Add(bundleName, tmpRes);
                }
                return tmpObj;
            }
            return null;
        }

        /// <summary>
        /// 释放指定bundle包小的指定资源res
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="resName"></param>
        public void DisposeResObj(string bundleName,string resName)
        {
            if (loadObj.ContainsKey(bundleName))
            {
                loadObj[bundleName].Release(resName);
            }
        }
        /// <summary>
        /// 释放bundle下的所有资源
        /// </summary>
        /// <param name="bundleName"></param>
        public void DisposeResObj(string bundleName)
        {
            if (loadObj.ContainsKey(bundleName))
            {
                loadObj[bundleName].ReleaseAll();
            }
            Resources.UnloadUnusedAssets();
        }

        public void DisposeAllObj()
        {
            List<string> keys = new List<string>();
            keys.AddRange(loadObj.Keys);
            for(int i = 0; i < loadObj.Count; i++)
            {
                DisposeResObj(keys[i]);
            }
            loadObj.Clear();
        }

        /// <summary>
        /// 加载assetbundle，必须先加载manifest
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundle(string bundleName)
        {
            yield return null;
        }
    }
    
}
