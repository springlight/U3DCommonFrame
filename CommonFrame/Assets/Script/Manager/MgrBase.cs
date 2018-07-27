
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：
 * 框架管理类通用基类，主要负责注册和注销消息（msg)和消息对应的mono(MonoBase,也就是脚本)
 * 一个msgId，对应一个mono
 * 提供id查找mono有两种形式
 * 第一种：msgId->mgr->mono(mono script)
 * 第二种，msgId->mgr->mgrCenter->mgr->mono
 *  -------------------------------------------------------------------------*/
using Assets.Script.Manager;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MgrBase : MonoBase
{
    /// <summary>
    /// key:msgId
    /// value:EventNode，一个EventNode就是一个类似于数据结构中的链表节点，包含一个mono，和
    /// 下一个mono的指针
    /// </summary>
    private Dictionary<ushort, EventNode> evtTree = new Dictionary<ushort, EventNode>();

    /// <summary>
    /// 一个mono脚本可能对应于多个消息id
    /// </summary>
    /// <param name="mono">脚本</param>
    /// <param name="ids">消息ids</param>
    public void RegisterMsg(MonoBase mono ,params ushort [] ids)
    {
        for(int i = 0; i < ids.Length; i++)
        {
            EventNode node = new EventNode(mono);
            RegisterMsg(ids[i], node);
        }
    }
    
    /// <summary>
    /// 消息注册方法
    /// </summary>
    /// <param name="id">消息id</param>
    /// <param name="node">消息对应的mono节点</param>
    public void RegisterMsg(ushort id,EventNode node)
    {
        if (!evtTree.ContainsKey(id))
        {
            evtTree.Add(id, node);
        }
        else
        {
            //如果该消息id已经在evtTree里面，则把该mono节点放在id对应的链表尾部
            EventNode tmp = evtTree[id];
            //找到最后一个节点
            while (tmp.next != null) tmp = tmp.next;
            //把节点插入到链表最后
            tmp.next = node;
        }
    }

    /// <summary>
    /// 批量注销消息
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="ids"></param>
    public void UnRegisterMsg(MonoBase mono, params ushort[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            UnRegisterMsg(ids[i], mono);
        }
    }

    /// <summary>
    /// 注销消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="node"></param>
    public void UnRegisterMsg(ushort id,MonoBase node)
    {
        if (!evtTree.ContainsKey(id))
        {
            Debug.Log("找不到要注销的消息id" + id);
            return;
        }
        else
        {
            //该消息id对应的列表第一个nomo节点
            EventNode header = evtTree[id];
            EventNode tmp = header;
            //如果第一个节点就是要注销的节点
            if(header.mono == node)
            {
                //头节点后面还有节点
                if(header.next != null)
                {
                    header.mono = header.next.mono;
                    header.next = header.next.next;
                }
                else//只有一个节点的情况
                {
                    evtTree.Remove(id);
                }
               
            }
            //删除中间节点
            else
            {
                //找到目标节点的前一个节点
                while(tmp.next != null && tmp.next.mono != node)
                {
                    tmp = tmp.next;
                }
                //中间节点
                if( tmp.next.next != null)
                {
                    tmp.next = tmp.next.next;
                }
                else//最后一个节点
                {
                    tmp.next = null;
                }

            }
        }
    }
    /// <summary>
    /// 消息处理方法
    /// </summary>
    /// <param name="msg">要处理的消息</param>
    public override void ProcessEvent(MsgBase msg)
    {
        if (!evtTree.ContainsKey(msg.MsgId))
        {
            Debug.LogError("消息列表里不存在该消息，消息id为==" + msg.MsgId);
        }
        else
        {
            //注册该id的每一个mono都要执行
            EventNode tmp = evtTree[msg.MsgId];
            //循环遍历每个注册该消息的脚本
            do
            {
                Debug.LogError("当前处理的mono是==" + tmp.mono.ToString());
                tmp.mono.ProcessEvent(msg);
                tmp = tmp.next;
                
            } while (tmp != null);
        }
    }

   
}
