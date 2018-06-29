/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 11:04:13
 * 版本号：v1.0
 * 本类主要用途描述：
 * 用来加载单个bundle，此类是IABResLoader的上层类，直接给加载给IABResLoader中用到的assetbundle
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
        /// <summary>
        /// 在IABRelationMgr里面调用
        /// </summary>
        /// <param name="finish">加载完成的回调</param>
        /// <param name="loadProgress">加载进度回调</param>
        public IABLoader(LoadFinish finish, LoadProgress loadProgress)
        {
            loadFinishFun = finish;
            loadProgressFun = loadProgress;
        }
        /// <summary>
        /// 上层传过来的path里已经包含了bundlename
        /// </summary>
        /// <param name="bundlename"></param>
        /// <param name="path"></param>
        /// <param name="finishFun"></param>
        /// <param name="loadProgressFun"></param>
        public IABLoader(string bundlename,string path,LoadFinish finishFun,LoadProgress loadProgressFun)
        {
            this.bundleName = bundlename;
            
            path = bundlePath;
            loadFinishFun = finishFun;
            this.loadProgressFun = loadProgressFun;
        }

        /// <summary>
        /// 要求上层传递bundle的完成路径,暂时在第二个构造方法里传递
        /// </summary>
        /// <param name="path"></param>
        public void LoadResource(string path)
        {
            bundlePath = path;
        }
        /// <summary>
        /// 要求上层传递bundle的name,暂时在第二个构造方法里传递
        /// </summary>
        /// <param name="path"></param>
        public void SetBundleName(string bundle)
        {
            bundleName = bundle;
        }

        /// <summary>
        /// 加载给定路径的assetbundle，由上层IABRealtionMgr调用
        /// </summary>
        /// <returns></returns>
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

        #region 供上层调用

        public void DebugLoader()
        {
            if(resLoader != null)
            {
                resLoader.DebugAllRes();
            }
        }
        /// <summary>
        /// 获取assetbundle包里单个资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UnityEngine.Object GetSingleRes(string name)
        {
            return resLoader[name];
        }
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
        #endregion 
    }
}
