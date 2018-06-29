using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBase : MonoBase {

    public ushort[] msgIds;
    public override void ProcessEvent(MsgBase msg)
    {

    }

    public void RegisterSelf(MonoBase mono, params ushort[] msgids)
    {
        msgIds = msgids;
        NpcMgr.ins.RegisterMsg(mono, msgids);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgids)
    {
        NpcMgr.ins.UnRegisterMsg(mono, msgids);
    }

    public void SendMsg(MsgBase msg)
    {
        NpcMgr.ins.SendMsg(msg);
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
