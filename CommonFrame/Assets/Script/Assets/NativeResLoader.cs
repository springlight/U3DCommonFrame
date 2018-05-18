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

                case (ushort)AssetEvent.HunkRes:
                    {
                        HunkAssetRes tmpMsg = (HunkAssetRes)recMsg;
                       
                    }

                    break;
            }
        }


        public void GetRes(string sceneName,string bundleName,bool isSingle,ushort backId)
        {
           // if(IL)
        }
    }
}
