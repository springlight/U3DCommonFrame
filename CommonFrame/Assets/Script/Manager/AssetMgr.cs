/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/16 18:56:03
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Manager
{
    public class AssetMgr:MgrBase
    {
        public static AssetMgr ins;

        void Awake()
        {
            ins = this;
        }


        public void SendMsg(MsgBase msg)
        {
            if (msg.GetMgr() == MgrId.AssetMgr)
            {
                ProcessEvent(msg);
            }
            else
            {
                //通知消息处理中心
                MgrCenter.ins.SendMsg(msg);
            }

        }
       
    }
}
