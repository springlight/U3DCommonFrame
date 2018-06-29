using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMgr : MgrBase {

    /// <summary>
    /// 
    /// key:componet name
    /// </summary>
    private Dictionary<string, GameObject> children = new Dictionary<string, GameObject>();
    public static NpcMgr ins;

    void Awake()
    {
        //Debug.LogError("UIMgr Awake");
        ins = this;
    }


    public void SendMsg(MsgBase msg)
    {
        if (msg.GetMgr() == MgrId.NPCMgr)
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
        Debug.LogError("Register UIBehaviour name is {0}" + name);
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
