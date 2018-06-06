using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TCPEvent
{
    TcpConnect = MgrId.NetMgr + 1,
    TcpSendMsg,
    Maxvalue
}

public class TCPConnectMsg : MsgBase
{
    public string ip;
    public ushort port;
    public TCPConnectMsg(ushort msgId,string ip,ushort port)
    {
        this.ip = ip;
        this.port = port;
        this.MsgId = msgId;
    }
}

public class TCPMsg : MsgBase
{
    public NetMsgBase netMsg;
    public TCPMsg(ushort id,NetMsgBase netMsg)
    {
        this.MsgId = id;
        this.netMsg = netMsg;
    }
}
public class TcpSocket : NetBase {

    private NetWorkToServer socket = null;
    public override void ProcessEvent(MsgBase msg)
    {
        switch (msg.MsgId)
        {
            case (ushort)TCPEvent.TcpConnect:
                {
                    TCPConnectMsg connectMsg = (TCPConnectMsg)msg;
                    socket = new NetWorkToServer(connectMsg.ip,connectMsg.port);
                }

                break;
            case (ushort)TCPEvent.TcpSendMsg:
                TCPMsg sendMsg = (TCPMsg)msg;
                socket.PutSendMsgToPool(sendMsg.netMsg);
                break;
        }
    }
    private void Awake()
    {
        msgIds = new ushort[]
        {
            (ushort)TCPEvent.TcpConnect,
            (ushort)TCPEvent.TcpSendMsg
        };
        RegisterSelf(this, msgIds);
    }

    public void Update()
    {
        if(socket != null)
        {
            socket.Update();
        }
    }

    
}
