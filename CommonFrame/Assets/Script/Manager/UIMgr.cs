
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：整个框架的消息基类，包涵一个消息id，该id类似于电脑ip地址，管理类Mgr通过
 * 这个id找到和这个id对应的mono脚本
 *  -------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MgrBase
{
    /// <summary>
    /// 存储每个Panel的交互控件，如button
    /// </summary>
    private Dictionary<string, GameObject> children = new Dictionary<string, GameObject>();
    public static UIMgr ins;

    void Awake()
    {
        ins = this;
    }

   
    public  void SendMsg(MsgBase msg)
    {
        if(msg.GetMgr() == MgrId.UIMgr)
        {
            ProcessEvent(msg);
        }
        else
        {
            //通知消息处理中心
            MgrCenter.ins.SendMsg(msg);
        }
      
    }
    public GameObject GetGameObj(string name)
    {
        if (children.ContainsKey(name))
            return children[name];
        return null;
    }
    public void RegisterGameObject(string name, GameObject go)
    {
        if (!children.ContainsKey(name))
        {
            children.Add(name, go);
        }
    }

    public void UnRegisterGameObject(string name)
    {
        if (!children.ContainsKey(name)) return;
        children.Remove(name);
    }
}
