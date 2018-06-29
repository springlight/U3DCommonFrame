/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 10:41:26
 * 版本号：v1.0
 * 本类主要用途描述：、
 * 该类主要是对assetbundle.Load(resName)的封装，直接加载给定assetbundle包里的资源,其中assetbundle由IABLoader提供
 * 
 * 
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    public class IABResLoader:IDisposable
    {
        private AssetBundle bundle;
        public IABResLoader (AssetBundle bundle)
        {
            this.bundle = bundle;
        }
        /// <summary>
        /// 获取单个资源
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public UnityEngine.Object this[string resName]
        {
            get
            {
                if(this.bundle == null || !this.bundle.Contains(resName))
                {
                    Debug.LogError("no such this resource ==" + resName);
                    return null;
                }
                return bundle.LoadAsset(resName);
            }
        }
        /// <summary>
        /// 获取bundle中所有的资源
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public UnityEngine.Object [] GetMutiRes(string resName)
        {
            if (this.bundle == null || !this.bundle.Contains(resName))
            {
                Debug.LogError("no such this resource ==" + resName);
                return null;
            }
            return bundle.LoadAssetWithSubAssets(resName);
        }
        /// <summary>
        /// 卸载单个资源
        /// </summary>
        /// <param name="resObj"></param>
        public void UnLoadRes(UnityEngine.Object resObj)
        {
            Resources.UnloadAsset(resObj);
        }
        /// <summary>
        /// 销毁bundle
        /// </summary>
        public void Dispose()
        {
            //直销毁内存中加载的bundle镜像,注意这里没有卸载
            //从assetbundle里load的资源
            bundle.Unload(false);
        }

        public void DebugAllRes()
        {
            string[] res = bundle.GetAllAssetNames();
            for(int i = 0; i < res.Length; i++)
            {
                Debug.Log("the target bundle containes res name  ==" + res[i]);
            }
        }

    }
}
