/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo  
 * 作者：XX
 * 联系方式：XXXXXXXX@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：
 * 每一个UIPanel的直接基类，主要负责向消息中心注册自己
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Common
{
    public  class UIBase:MonoBase
    {
        public ushort[] msgIds;
        public override void ProcessEvent(MsgBase msg)
        {
           
        }

        public void RegisterSelf(MonoBase mono,params ushort [] msgids)
        {
            UIMgr.ins.RegisterMsg(mono, msgids);
        }

        public void UnRegisterSelf(MonoBase mono,params ushort [] msgids)
        {
            UIMgr.ins.UnRegisterMsg(mono, msgids);
        }
        
        public void SendMsg(MsgBase msg)
        {
            UIMgr.ins.SendMsg(msg);
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
