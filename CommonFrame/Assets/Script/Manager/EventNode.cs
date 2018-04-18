/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:32:20
 * 版本号：v1.0
 * 本类主要用途描述：
 * 用于消息-》mono链表节点
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Manager
{
    public class EventNode
    {
        public MonoBase mono;
        public EventNode next;

        public EventNode(MonoBase mono)
        {
            this.mono = mono;
            next = null;
        }

      
    }
}
