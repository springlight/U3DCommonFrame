
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：
 * UIMgr,继承自MgrBase，除了有注册脚本对应的Msg之外
 * 额外扩展了，保存每一个UIPanel(单独的游戏UI界面，比如装备界面，和背包界面)用户交互的组件，比如Button
 * 
 * 
 *  -------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MgrBase
{
    /// <summary>
    /// 存储每个Panel的交互控件，如button
    /// key:componet name
    /// </summary>
    private Dictionary<string, GameObject> children = new Dictionary<string, GameObject>();
    public static UIMgr ins;

    void Awake()
    {
        //Debug.LogError("UIMgr Awake");
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
        //Debug.LogError("Register UIBehaviour name is {0}"+ name);
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
