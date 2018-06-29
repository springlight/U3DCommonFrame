﻿using Assets.Script;
using Assets.Script.Common;
using Assets.Script.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTwo : UIBase {

    public override void ProcessEvent(MsgBase msg)
    {
        switch (msg.MsgId)
        {
            case (ushort)UIEventGuo.Load:
                Debug.LogError("PanelTwo 接收到了消息 Load 消息");
                break;
            case (ushort)UIEventGuo.Regist:
                Debug.LogError("PanelTwo 接收到了消息 Regist 消息");
                break;
        }
        base.ProcessEvent(msg);
    }

    private void Start()
    {
        ushort[] ids = new ushort[]
        {
            (ushort)UIEventGuo.Regist,
        };
        RegisterSelf(this, ids);
        UIMgr.ins.GetGameObj("Button2").GetComponent<UIBehaviour>().AddBtnEvtListener(OnClickSendMsg);
    }

    void OnClickSendMsg()
    {
        Debug.LogError("点击panel one 按钮");
        MsgBase msg = new MsgBase((ushort)UIEventGuo.Load);
        UIMgr.ins.SendMsg(msg);
    }
}