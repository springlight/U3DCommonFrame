/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/28 11:43:19
 * 版本号：v1.0
 * 本类主要用途描述：
 * 加载assetbundle必须先加载manifest，此类就是加载manifest
 *  -------------------------------------------------------------------------*/

using Assets.Script.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    public class IABManifestLoader
    {
        public static IABManifestLoader ins = null;
        public AssetBundleManifest assetManifest;
        public string manifestPath;
        private bool isLoadFinish = false;
        public AssetBundle manifestLoader;
        public static IABManifestLoader Ins
        {
            get
            {
                if(ins == null)
                {
                    ins = new IABManifestLoader();
                }
                return ins;
            }

        }

        public IABManifestLoader()
        {

            manifestPath = IPathTools.GetWWWAssetBundlePath() + "/" + IPathTools.GetPlatformFloderName(Application.platform);
            Debug.LogError("mainfiestPath is .." + manifestPath);
        }
        public void SetManifestPath(string path)
        {
            manifestPath = path;
        }

        public bool IsLoadFinish()
        {
            return isLoadFinish;
        }
        /// <summary>
        /// 加载manifest
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadManifest()
        {
            WWW manifest = new WWW(manifestPath);
            yield return manifest;
            if (!string.IsNullOrEmpty(manifest.error))
            {
                Debug.LogError(manifest.error);
            }
            else
            {
                if(manifest.progress >= 1.0)
                {
                    manifestLoader = manifest.assetBundle;
                    assetManifest = manifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                    isLoadFinish = true;
                }
            }

        }

        public string [] GetDepedences(string bundleName)
        {
            return assetManifest.GetAllDependencies(bundleName);
        }

        public void UnLoadManifest()
        {
            manifestLoader.Unload(true);
        }
    }
}
