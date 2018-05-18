/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/16 18:03:26
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Assets
{
    public enum AssetEvent
    {
        HunkRes = MgrId.AssetMgr + 1,
        ReleaseSingleObj,
        ReleaseBundleObj,
        ReleaseSceneObj,
        ReleaseSingleBundle,
        ReleaseSceneBundle,
        ReleaseAll,
    }
    /// <summary>
    /// 上层发给我们的消息
    /// </summary>
    public class HunkAssetRes:MsgBase
    {
        public string sceneName;
        public string bundleName;
        public string resName;
        public ushort backMsgId;
        public bool isSingle;//获取单个还是多个



        public HunkAssetRes(bool isSingle,ushort msgId,string sceneName,string bundleName,string resName,ushort backMsgId)
        {
            this.isSingle = isSingle;
            this.sceneName = sceneName;
            this.MsgId = msgId;
            this.bundleName = bundleName;
            this.resName = resName;
            this.backMsgId = backMsgId;
        }
    }
    /// <summary>
    /// 返回给上层的消息
    /// </summary>
    public class HunkAssetResBack : MsgBase
    {
        public Object[] value;
        public HunkAssetResBack()
        {
            this.MsgId = 0;
            this.value = null;
        }
        public void Changer(ushort msgId,params Object[] tmpValue)
        {
            this.MsgId = msgId;
            this.value = tmpValue;
        }

        public void Changer(ushort msgId)
        {
            this.MsgId = msgId;
        }

        public void Changer(params Object[] tmpValue)
        {
            this.value = tmpValue;
        }
    }
}
