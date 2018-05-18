/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/16 18:00:15
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using Assets.Script.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Assets
{
    public class AssetBase:MonoBase
    {
        public ushort[] msgIds;

        public override void ProcessEvent(MsgBase msg)
        {

        }

        public void RegisterSelf(MonoBase mono, params ushort[] msgids)
        {
            AssetMgr.ins.RegisterMsg(mono, msgids);
        }

        public void UnRegisterSelf(MonoBase mono, params ushort[] msgids)
        {
            AssetMgr.ins.UnRegisterMsg(mono, msgids);
        }

        public void SendMsg(MsgBase msg)
        {
            AssetMgr.ins.SendMsg(msg);
        }
        /// <summary>
        /// panel销毁的时候注销自己
        /// </summary>
        private void OnDestroy()
        {
            if (msgIds != null && msgIds.Length > 0)
                UnRegisterSelf(this, msgIds);
        }


    }
}
