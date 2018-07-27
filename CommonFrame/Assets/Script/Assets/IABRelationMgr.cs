/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 13:40:47
 * 版本号：v1.0
 * 本类主要用途描述：
 * 单个bundle管理
 * 主要处理bundle的依赖和被依赖关系.保存bundle的依赖，和依赖该bundle的关系
 * 同时，初始化了IABLoader类
 * 和IABResLoader 和IABLoader是层层递进的关系
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
    public class IABRelationMgr
    {
       private bool isLoadFinish = false;
        /// <summary>
        /// 保存依赖关系
        /// eg；xx 依赖yy,还依赖aa
        /// 则depedenceLst 里有（yy, aa）
        /// </summary>
        private List<string> depedenceBundle = null;

        /// <summary>
        /// 记载被依赖关系
        /// eg ，yy aa refer xx
        /// </summary>
        private List<string> refferBundle = null;
        private string bundleName;
        //加载assetbundle的类
        private IABLoader abLoader;
        private LoadProgress loadProcess;
        public IABRelationMgr()
        {
            depedenceBundle = new List<string>();
            refferBundle = new List<string>();
        }
        public void Init(string bundleName, LoadProgress progress)
        {
            isLoadFinish = false;
            this.bundleName = bundleName;
            loadProcess = progress;
            string bundlePath = IPathTools.GetWWWAssetBundlePath() + "/" + bundleName;
            Debug.LogError(" [IABRelationMgr] bundle Path is " + bundlePath);
            abLoader = new IABLoader(bundleName, bundlePath, BundleLoadFinish, progress);
 
        }


        public bool IsLoadFinish { get { return isLoadFinish; } }

        #region Refference
        /// <summary>
        /// 添加被依赖项
        /// </summary>
        /// <param name="bundleName"></param>
        public void AddRefference(string bundleName)
        {
            refferBundle.Add(bundleName);
        }
        /// <summary>
        /// 获取所有被依赖
        /// </summary>
        /// <returns></returns>
        public List<string> GetRefference()
        {
            return refferBundle;
        }
        /// <summary>
        /// 移除被依赖的关系
        /// 比如B,C 依赖D，那么D就是被B，C依赖，如果B，C被移除了，那么
        /// D所有的被依赖关系就没了。然后就可以删除D
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns>表示是否释放了自己</returns>
        public bool RemoveRefference(string bundleName)
        {
            for(int i = refferBundle.Count - 1; i > 0; i--)
            {
                if (refferBundle[i].Equals(bundleName))
                {
                    refferBundle.RemoveAt(i);
                    
                }
            }
            //
            if(refferBundle.Count <= 0)
            {
                Dispose();
                return true;
            }
            return false;
        }
        #endregion
        #region Dependences
        /// <summary>
        /// 设置依赖
        /// </summary>
        /// <param name="depedences"></param>
        public void SetDepedences(string[] depedences)
        {
            if (depedences.Length > 0)
            {
                depedenceBundle.AddRange(depedences);
            }
        }

        public List<string> GetDepedences()
        {
            return depedenceBundle;
        }

        public void RemoveDepence(string bundleName)
        {
            for (int i = depedenceBundle.Count - 1; i >= 0; i--)
            {
                if (bundleName.Equals(depedenceBundle[i]))
                {
                    depedenceBundle.RemoveAt(i);
                }
            }
        }
        #endregion
        #region 由下层提供API


        //开始加载assetbundle
        public IEnumerator LoadAssetBundle()
        {
            yield return abLoader.LoadBundle();
        }

        public void Dispose()
        {
            if(abLoader != null)
            {
                abLoader.Dispose();
            }
        }

        public UnityEngine.Object GetSingleRes(string resName)
        {
          //  return abLoader.GetSingleRes(bundleName);
            return abLoader.GetSingleRes(resName);
        }

        public UnityEngine.Object[] GetMutilRes(string resName)
        {
            return abLoader.GetMutiRes(resName);
        }

        public void DebugAsset()
        {
            if(abLoader != null)
            {
                abLoader.DebugLoader();
            }
        }

     
        public LoadProgress GetProgress()
        {
            return loadProcess;
        }
        public void BundleLoadFinish(string bundleName)
        {
            isLoadFinish = true;
        }

        public string GetBundleName()
        {
            return bundleName;
        }
       
        #endregion
    }
}
