/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 13:57:50
 * 版本号：v1.0
 * 本类主要用途描述：
 * 每个UIPanel，用于收集每个UI控件
 *  -------------------------------------------------------------------------*/

using Assets.Script.Common;
using Assets.Script.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script
{
    public class LoadPanelTest:UIBase 
    {


        //每一个Panel都有要处理自己的消息
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)UIEventGuo.Load:
                    break;
                case (ushort)UIEventGuo.Regist:
                    break;
          
            }
            base.ProcessEvent(msg);
        }

        private void Awake()
        {
            //注册自己感兴趣的id
            msgIds = new ushort[] {
                (ushort)UIEventGuo.Load,
                (ushort)UIEventGuo.Regist,
            };
            RegisterSelf(this,msgIds);
           // UIMgr.ins.GetGameObj("lightOn").GetComponent<UIBehaviour>().AddBtnEvtListener();
        }
    }

    /// <summary>
    /// 该页面需要的id
    /// </summary>
    public enum UIEventGuo
    {
        Load = MgrId.UIMgr,
        Regist,
        MaxValue
    }

    public enum UIEventZhang
    {
        NpcAttck = UIEventGuo.MaxValue,
    }
}
