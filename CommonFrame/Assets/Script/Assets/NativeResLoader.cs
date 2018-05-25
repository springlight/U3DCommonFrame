/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/16 17:59:23
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Assets
{
    public delegate void NativeResCallBack(NativeResCallBackNode loader);
    /// <summary>
    /// 该类没有发送消息功能，需要一个委托作为回调
    /// </summary>
    public class NativeResCallBackNode
    {
        public string sceneName;
        public string bundleName;
        public string resName;
        public ushort backMsgId;
        public bool isSingle;
        public NativeResCallBackNode nextValue;
        public NativeResCallBack callBack;

        public NativeResCallBackNode(bool isSingle, string sceneName, string bundleName, string resName, ushort backMsgId, NativeResCallBack callback, NativeResCallBackNode node)
        {
            this.isSingle = isSingle;
            this.sceneName = sceneName;

            this.bundleName = bundleName;
            this.resName = resName;
            this.backMsgId = backMsgId;
            this.callBack = callback;
            this.nextValue = node;
        }

        public void Dispose()
        {
            callBack = null;
            nextValue = null;
        }
    }
    
    public class NativeResCallBackMgr
    {
        Dictionary<string, NativeResCallBackNode> mgr = null;
        public NativeResCallBackMgr()
        {
            mgr = new Dictionary<string, NativeResCallBackNode>();
        }
        /// <summary>
        /// 来了请求添加的过程
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="node"></param>
        public void AddBundle(string bundleName,NativeResCallBackNode node)
        {
            if (mgr.ContainsKey(bundleName))
            {
                NativeResCallBackNode tmp = mgr[bundleName];
                while(tmp.nextValue != null)
                {
                    tmp = tmp.nextValue;
                }
                tmp.nextValue = node;
            }
            else
            {
                mgr.Add(bundleName, node);
            }
        }
        /// <summary>
        /// 加载完后，消息向上层传递完成了，就把这些缓存命令删除
        /// </summary>
        /// <param name="bundleName"></param>
        public void Dispose(string bundleName)
        {
            if (mgr.ContainsKey(bundleName))
            {
                NativeResCallBackNode tmp = mgr[bundleName];
                ///释放每一个node
                while(tmp.nextValue != null)
                {
                    NativeResCallBackNode curNode = tmp;
                    tmp = tmp.nextValue;
                    curNode.Dispose();
                }
                tmp.Dispose();
                mgr.Remove(bundleName);
            }
              

        }
        /// <summary>
        /// 加载完成后的回调
        /// </summary>
        /// <param name="bundle"></param>
        public void CallBackRes(string bundle)
        {
            if (mgr.ContainsKey(bundle))
            {
                NativeResCallBackNode tmp = mgr[bundle];
                do
                {
                    tmp.callBack(tmp);
                    tmp = tmp.nextValue;
                } while (tmp != null);
            }
        }
    }

    public class NativeResLoader:AssetBase
    {

        void Awake()
        {
            msgIds = new ushort[] {
                (ushort)AssetEvent.ReleaseBundleObj,
                (ushort)AssetEvent.ReleaseAll,
                (ushort)AssetEvent.ReleaseSceneBundle,
                (ushort)AssetEvent.ReleaseSceneObj,
                (ushort)AssetEvent.ReleaseSingleBundle,
                (ushort)AssetEvent.ReleaseSingleObj,
                (ushort)AssetEvent.HunkRes,
                (ushort)AssetEvent.TestRes
            };
            RegisterSelf(this, msgIds);
        }

        public override void ProcessEvent(MsgBase recMsg)
        {
            switch (recMsg.MsgId)
            {
                case (ushort)AssetEvent.ReleaseBundleObj:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadBundleResObjs(tmpMsg.sceneName, tmpMsg.bundleName);
                    }
                  
                    break;
                case (ushort)AssetEvent.ReleaseAll:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadAllAssetbundleAndResObj(tmpMsg.sceneName);
                    }
                    break;
                case (ushort)AssetEvent.ReleaseSceneBundle:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadAllAssetBundle(tmpMsg.sceneName);
                    }
                    break;
                case (ushort)AssetEvent.ReleaseSceneObj:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadAllRes(tmpMsg.sceneName);
                    }
                    break;
                case (ushort)AssetEvent.ReleaseSingleBundle:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadAssetBundle(tmpMsg.sceneName,tmpMsg.bundleName);
                    }
                    break;
                case (ushort)AssetEvent.ReleaseSingleObj:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        ILoaderMgr.ins.UnLoadResObj(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName);
                    }
                   
                    break;
                //请求资源
                case (ushort)AssetEvent.HunkRes:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                        GetRes(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName, tmpMsg.isSingle, tmpMsg.backMsgId);
                    }

                    break;
                case (ushort)AssetEvent.TestRes:
                    {
                        HunkAssetResBack tmpMsg = (HunkAssetResBack)recMsg;
                        Debug.LogError("tmpMsg.Length ==" + tmpMsg.value.Length);
                        Debug.LogError("tmpMsg.value[0] ==" + tmpMsg.value[0].name);
                        GameObject tmpGo = Instantiate(tmpMsg.value[0]) as GameObject;

                       // GetRes(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName, tmpMsg.isSingle, tmpMsg.backMsgId);
                    }

                    break;
            }
        }
        /// <summary>
        /// node 回调
        /// </summary>
        /// <param name="tmpNode"></param>
        public void SendToBackMsg(NativeResCallBackNode tmpNode)
        {
            if (tmpNode.isSingle)
            {
                UnityEngine.Object tmpObj = ILoaderMgr.ins.GetSingleRes(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
                ReleasbBack.Changer(tmpNode.backMsgId, tmpObj);
                SendMsg(ReleasbBack);
            }
            else
            {
                UnityEngine.Object [] tmpObjs = ILoaderMgr.ins.GetMutilRes(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
                ReleasbBack.Changer(tmpNode.backMsgId, tmpObjs);
                SendMsg(ReleasbBack);
            }
        }

        HunkAssetResBack resBackMsg = null;
        HunkAssetResBack ReleasbBack
        {
            get
            {
                if(resBackMsg == null)
                {
                    resBackMsg = new HunkAssetResBack();
                }
                return resBackMsg;
            }

        }

        NativeResCallBackMgr callBack = null;
        NativeResCallBackMgr callBackMgr
        {
            get
            {
                if(callBack == null)
                {
                    callBack = new NativeResCallBackMgr();
                }
                return callBack;
            }
        }
        //sceneone/load.ld
        void LoadProgrecess(string bundleName,float progress)
        {
            if(progress >= 1.0f)
            {
                //上层的回调
                callBackMgr.CallBackRes(bundleName);
                callBackMgr.Dispose(bundleName);
            }
        }

        public void GetRes(string sceneName,string bundleName,string res,bool isSingle,ushort backId)
        {
            ///没有加载
            if (!ILoaderMgr.ins.IsLoadingAssetBundle(sceneName, bundleName))
            {
                ILoaderMgr.ins.LoadAsset(sceneName, bundleName, LoadProgrecess);
                string bundleFullName = ILoaderMgr.ins.GetBundleRetateName(sceneName, bundleName);
                if (!string.IsNullOrEmpty(bundleFullName))
                {
                    NativeResCallBackNode tmpNode = new NativeResCallBackNode(isSingle, sceneName, bundleName, res, backId, SendToBackMsg, null);
                    //fullname sceneone/load.ld
                    callBackMgr.AddBundle(bundleFullName, tmpNode);

                }
                else
                {
                    Debug.LogError("没有该bundle==" + bundleName);
                }
            }
            //表示已经加载完成
            else if (ILoaderMgr.ins.IsLoadingBundleFinish(sceneName, bundleName))
            {
                if (isSingle)
                {
                    UnityEngine.Object tmpObj = ILoaderMgr.ins.GetSingleRes(sceneName, bundleName, res);
                    ReleasbBack.Changer(backId, tmpObj);
                    SendMsg(ReleasbBack);
                }
                else
                {
                    UnityEngine.Object [] tmpObjs = ILoaderMgr.ins.GetMutilRes(sceneName, bundleName, res);
                    ReleasbBack.Changer(backId, tmpObjs);
                    SendMsg(ReleasbBack);
                }
            }
            //已经加载，但没完成
            else
            {
                string bundleFullName = ILoaderMgr.ins.GetBundleRetateName(sceneName, bundleName);
                if (!string.IsNullOrEmpty(bundleFullName))
                {
                    NativeResCallBackNode tmpNode = new NativeResCallBackNode(isSingle, sceneName, bundleName, res, backId, SendToBackMsg, null);
                    callBackMgr.AddBundle(bundleFullName, tmpNode);

                }
                else
                {
                    Debug.LogError("没有该bundle==" + bundleName);
                }
            }
        }
    }
}
