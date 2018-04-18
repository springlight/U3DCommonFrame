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
            base.ProcessEvent(msg);
        }

        private void Start()
        {
            msgIds = new ushort[] { };
            RegisterSelf(this,msgIds);
        }
    }
}
