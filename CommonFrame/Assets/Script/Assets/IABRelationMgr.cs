/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 13:40:47
 * 版本号：v1.0
 * 本类主要用途描述：
 * 单个bundle管理
 * 主要处理bundle的依赖和被依赖关系.保存bundle的依赖，和依赖该bundle的关系
 * 和IABResLoader 和IABLoader是层层递进的关系
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Assets
{
    public class IABRelationMgr
    {
        bool isLoadFinish = false;
        /// <summary>
        /// 保存依赖关系
        /// eg；xx 依赖yy,还依赖aa
        /// 
        /// </summary>
        List<string> depedenceBundle = null;

        /// <summary>
        /// 记载被依赖关系
        /// eg ，yy aa refer xx
        /// </summary>
        List<string> refferBundle = null;
        string bundleName;
        IABLoader assetLoader;
        private LoadProgress loadProcess;
        public IABRelationMgr()
        {
            depedenceBundle = new List<string>();
            refferBundle = new List<string>();
        }


        public bool IsLoadFinish { get { return isLoadFinish; } }

        public IEnumerator LoadAssetBundle()
        {
            yield return assetLoader.LoadBundle();
        }
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
        /// 
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
            //如果所有的都删除了，则删除该bundle
            if(refferBundle.Count <= 0)
            {
                Dispose();
                return true;
            }
            return false;
        }
        public void Dispose()
        {
            if(assetLoader != null)
            {
                assetLoader.Dispose();
            }
        }

        public UnityEngine.Object GetSingleRes(string bundleName)
        {
            return assetLoader.GetRes(bundleName);
        }

        public UnityEngine.Object[] GetMutilRes(string bundleName)
        {
            return assetLoader.GetMutiRes(bundleName);
        }

        public void DebugAsset()
        {
            if(assetLoader != null)
            {
                assetLoader.DebugLoader();
            }
        }

        public void Init(string bundleName, LoadProgress progress)
        {
            isLoadFinish = false;
            this.bundleName = bundleName;
            loadProcess = progress;
            assetLoader = new IABLoader(BundleLoadFinish, progress);
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
        /// <summary>
        /// 设置依赖
        /// </summary>
        /// <param name="depedences"></param>
        public void SetDepedences(string [] depedences)
        {
            if(depedences.Length > 0)
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
            for(int i = depedenceBundle.Count - 1; i >= 0 ; i--)
            {
                if (bundleName.Equals(depedenceBundle[i]))
                {
                    depedenceBundle.RemoveAt(i);
                }
            }
        }
    }
}
