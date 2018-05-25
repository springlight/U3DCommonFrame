/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 11:04:13
 * 版本号：v1.0
 * 本类主要用途描述：
 * 用来加载单个bundle，此类是IABResLoader的上层类，直接给加载给IABResLoader中用到的bundle
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    public delegate void LoadFinish(string bundleName);
    public delegate void LoadProgress(string bundle,float process);
    public class IABLoader:IDisposable
    {
        private IABResLoader resLoader;
        private string bundleName;
        private string bundlePath;
        private WWW wwwLoader;
        private LoadFinish loadFinishFun;
        private LoadProgress loadProgressFun;
        private float progress;

      
        public IABLoader()
        {

        }
        public IABLoader(LoadFinish finish, LoadProgress loadProgress)
        {
            loadFinishFun = finish;
            loadProgressFun = loadProgress;
        }
        public IABLoader(string name,string path,LoadFinish finishFun,LoadProgress loadProgressFun)
        {
            bundleName = name;
            path = bundlePath;
            loadFinishFun = finishFun;
            this.loadProgressFun = loadProgressFun;
        }

        /// <summary>
        /// 要求上层传递bundle的完成路径
        /// </summary>
        /// <param name="path"></param>
        public void LoadResource(string path)
        {
            bundlePath = path;
        }

        public void SetBundleName(string bundle)
        {
            bundleName = bundle;
        }
        public IEnumerator LoadBundle()
        {
            wwwLoader = new WWW(bundlePath);
            while (!wwwLoader.isDone)
            {
             
                progress = wwwLoader.progress;
                if (loadProgressFun != null)
                {
                    loadProgressFun(bundleName, progress);
                }
                yield return wwwLoader.progress;
                progress = wwwLoader.progress;
                //if (loadProgressFun != null)
                //{
                //    loadProgressFun(bundleName, progress);
                //}
            }
            if (progress >= 1.0f)
            {
                
                resLoader = new IABResLoader(wwwLoader.assetBundle);
                if (loadProgressFun != null)
                {
                    loadProgressFun(bundleName, progress);
                }
                if (loadFinishFun != null)
                    loadFinishFun(bundleName);

            }
            else
            {
                Debug.LogError("Load bundle error " + bundleName);
            }
        }

     

        public void DebugLoader()
        {
            if(resLoader != null)
            {
                resLoader.DebugAllRes();
            }
        }

        public UnityEngine.Object GetRes(string name)
        {
            return resLoader[name];
        }

        public UnityEngine.Object [] GetMutiRes(string name)
        {
            return resLoader.GetMutiRes(name);
        }

        /// <summary>
        /// 卸载单个资源
        /// </summary>
        /// <param name="obj"></param>
        public void UnLoadAssetRes(UnityEngine.Object obj)
        {
            resLoader.UnLoadRes(obj);
        }


        public void Dispose()
        {
            if (resLoader != null)
            {
                resLoader.Dispose();
                resLoader = null;
            }
        }
    }
}
