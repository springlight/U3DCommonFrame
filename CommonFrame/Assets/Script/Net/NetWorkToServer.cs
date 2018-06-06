using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetWorkToServer  {
    //消息接受队列
    private Queue<NetMsgBase> recvMsgPool = null;
    //消息发送队列
    private Queue<NetMsgBase> sendMsgPool = null;

    private NetSocket clientSocket;

    private Thread sendThread;
    private Thread recvThread;


    public NetWorkToServer(string ip,ushort port)
    {
        recvMsgPool = new Queue<NetMsgBase>();
        sendMsgPool = new Queue<NetMsgBase>();
        clientSocket = new NetSocket();
        clientSocket.AsyncConnect(ip, port, AsyncConnectCallBack, AsyncCallBackRecv);
    }

    private  void AsyncConnectCallBack(bool success, ErrorSocket error, string exception)
    {
        //连接成功，之后开启发送线程
        if (success)
        {
            sendThread = new Thread(LoopSendMsg);
            sendThread.Start();
        }
        else
        {
            
        }
    }

    private  void AsyncCallBackRecv(bool success, ErrorSocket error, string exception, byte[] btyeMsg, string strMsg)
    {
        if (success)
        {

        }
        else
        {
            //处理错误信息
            Debug.LogError("recv error ");
        }
    }

    public void PutRecvMsgToPool(byte [] recvMsg)
    {
        NetMsgBase tmp = new NetMsgBase(recvMsg);
        recvMsgPool.Enqueue(tmp);
     
    }

  public  void Update()
    {
        if(recvMsgPool != null )
        {
            //每一帧都把收到的消息都取出来
            while(recvMsgPool.Count > 0)
            {
                NetMsgBase msg = recvMsgPool.Dequeue();
                AnalyseData(msg);
            }
        }
    }

    private void AnalyseData(NetMsgBase msg)
    {
        MgrCenter.ins.SendMsg(msg);
    }

    #region send thread
    private void LoopSendMsg()
    {
        while(clientSocket != null && clientSocket.IsConnected())
        {
            lock (sendMsgPool)
            {
                while (sendMsgPool.Count > 0)
                {
                    NetMsgBase tmpBody = sendMsgPool.Dequeue();
                    clientSocket.AsyncSend(tmpBody.buffer, AsyncSendCallBack);
                }
            }
            Thread.Sleep(100);
            
        }
    }
    private void AsyncSendCallBack(bool success, ErrorSocket error, string exception)
    {
        if (success)
        {
            
        }
    }

    ///上层往发送队列里发消息
    public void PutSendMsgToPool(NetMsgBase msg)
    {
        lock (sendMsgPool)
        {
            sendMsgPool.Enqueue(msg);
        }
    }
    #endregion

    #region Disconnect
    void CallBackDisconnect(bool success, ErrorSocket error, string exception)
    {
        if (success)
        {
            sendThread.Abort();

        }
        else
        {

        }
    }
    /// <summary>
    /// 断开连接，供上层应用
    /// </summary>
    public void DisConnect()
    {
        if(clientSocket != null && clientSocket.IsConnected())
        {
            clientSocket.AsyncDisConnect(CallBackDisconnect);
           
        }
          
    }

    #endregion
}
