using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetBase : MonoBase {

    public ushort[] msgIds;
    public override void ProcessEvent(MsgBase msg)
    {

    }

    public void RegisterSelf(MonoBase mono, params ushort[] msgids)
    {
        UIMgr.ins.RegisterMsg(mono, msgids);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgids)
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
