using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
public enum ErrorSocket
{
    Success = 0,
    TimeOut,
    SocketNull,
    SocketUnConnect,
    ConenctSuccess,
    ConnectUnSucesssUnkown,
    ConnectError,
    SendUnSucessUnkown,
    RecvUnSuccessUnkown,
    DisConnectUnkown,
    SendSucess,
    DisConnectSucess

}

public class NetSocket  {

    public delegate void CallBackNormal(bool success, ErrorSocket error, string exception);
   // public delegate void CallBackSend(bool success, ErrorSocket error, string exception);
    public delegate void CallBackRecv(bool success, ErrorSocket error, string exception,byte [] btyeMsg,string strMsg);
    // public delegate void CallBackDisConnect(bool success, ErrorSocket error, string exception);

    private CallBackNormal callBackConnect;
    private CallBackNormal callBackSend;
    private CallBackNormal callBackDisConnect;
    private CallBackRecv callBackRecv;
	
    private ErrorSocket errorSocket;

    private Socket clientSocket;
    private string addressIp;
    private ushort port;
    private byte[] recvBuf;
    SocketBuffer recvBuffer;
    public NetSocket()
    {
        recvBuffer = new SocketBuffer(0,RecvMsgOver);
        recvBuf = new byte[1024];
    }



  

    #region Connect
    public void AsyncConnect(string ip, ushort port, CallBackNormal connectCallback, CallBackRecv callBackRecv)
    {
        errorSocket = ErrorSocket.Success;
        this.callBackConnect = connectCallback;
        this.callBackRecv = callBackRecv;
        if (clientSocket != null && clientSocket.Connected)
        {
            this.callBackConnect(false, ErrorSocket.ConnectError, "connect repeat");
        }
        else if (clientSocket == null || !clientSocket.Connected)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(iPAddress,port);
            IAsyncResult connect = clientSocket.BeginConnect(endPoint, ConnectCallBack, clientSocket);
            //超时检测
           bool isTimeOut =  WriteDot(connect);
            if (!isTimeOut)
            {
                callBackConnect(false, errorSocket, "连接超时");
            }
        }
    }

    public bool IsConnected()
    {
        if (clientSocket != null && clientSocket.Connected)
            return true;
        else
            return false;
    }

    private void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndConnect(ar);
            if (!clientSocket.Connected)
            {
                errorSocket = ErrorSocket.ConnectUnSucesssUnkown;
                callBackConnect(false, errorSocket, "连接超时");
                return;
            }
            else
            {
                //开始接受消息
                errorSocket = ErrorSocket.ConenctSuccess;
                callBackConnect(true, errorSocket, "连接成功");
            }
        }
        catch (Exception e)
        {
            callBackConnect(false, errorSocket, e.Message);
        }
    }
    #endregion

    #region TimeOut Check
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ar"></param>
    /// <returns>true 表示可以写入
    /// false ,表示超时</returns>
    bool WriteDot(IAsyncResult ar)
    {
        int i = 0;
        while(ar.IsCompleted == false)
        {
            i++;
            if(i > 20)
            {
                errorSocket = ErrorSocket.TimeOut;
                return false;
            }
            Thread.Sleep(100);
        }
        return true;
    }
    #endregion

    #region Recv
    public void Recive()
    {
        if(clientSocket != null && clientSocket.Connected)
        {
            IAsyncResult recv = clientSocket.BeginReceive(recvBuf, 0, recvBuf.Length, SocketFlags.None, ReciveCallback,clientSocket);
            if (!WriteDot(recv))
            {
                callBackRecv(false, ErrorSocket.RecvUnSuccessUnkown, "recive false", null, "");
            }
        }
    }

    private void ReciveCallback(IAsyncResult ar)
    {
        try
        {
            if (!clientSocket.Connected)
            {
                callBackRecv(false, ErrorSocket.RecvUnSuccessUnkown, "connect false", null, "");
                return;
            }
            else
            {
                int length = clientSocket.EndReceive(ar);
                //接受的数据为空
                if (length == 0) return;
                //todo
               recvBuffer.RecvByte(recvBuf, length);
            }
        }
        catch (Exception e)
        {

            callBackRecv(false, ErrorSocket.RecvUnSuccessUnkown, e.Message, null, "");
        }
        Recive();
    }
    #endregion

    #region RecvMsgOver
    public void RecvMsgOver(byte[] allBytes)
    {
        callBackRecv(true, ErrorSocket.Success, "", null, "recv back success");
    }


    #endregion

    #region Send

    public void AsyncSend(byte[] sendBuf,CallBackNormal sendCallBack)
    {
        errorSocket = ErrorSocket.Success;
        this.callBackSend = sendCallBack;
        if(clientSocket == null)
        {
            this.callBackSend(false, ErrorSocket.SocketNull, "");
        }
        else if(!clientSocket.Connected)
        {
            callBackSend(false, ErrorSocket.SocketUnConnect, "");
        }
        else
        {
           IAsyncResult asySend =  clientSocket.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, SendCallback, clientSocket);
            if (!WriteDot(asySend))
            {
                callBackConnect(false, ErrorSocket.SendUnSucessUnkown, "连接超时");
            }
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            int byteSend = clientSocket.EndSend(ar);
            if(byteSend > 0)
            {
                callBackSend(true, ErrorSocket.SendSucess, "");
            }
            else
            {
                callBackSend(false, ErrorSocket.SendUnSucessUnkown, "");
            }
        }
        catch (Exception e)
        {

            callBackSend(false, ErrorSocket.SendUnSucessUnkown, "");
        }
    }
    #endregion

    #region DisConnect
    public void AsyncDisConnect(CallBackNormal disConnectCallback)
    {
        try
        {
            errorSocket = ErrorSocket.Success;
            this.callBackDisConnect = disConnectCallback;
            if(clientSocket == null)
            {
                this.callBackDisConnect(false, ErrorSocket.DisConnectUnkown, "client is null");
            }else if (!clientSocket.Connected)
            {
                this.callBackDisConnect(false, ErrorSocket.DisConnectUnkown, "client is unconnect");
            }
            else
            {
                IAsyncResult asyncDisConnect = clientSocket.BeginDisconnect(false, DisConnectCallBack, clientSocket);
                if (!WriteDot(asyncDisConnect))
                {
                    callBackDisConnect(false, ErrorSocket.DisConnectUnkown, "连接超时");
                }
            }
        }
        catch (Exception)
        {
            callBackDisConnect(false, ErrorSocket.DisConnectUnkown, "");
            throw;
        }
    }

    private void DisConnectCallBack(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndDisconnect(ar);
            clientSocket.Close();
            clientSocket = null;
            this.callBackDisConnect(true, ErrorSocket.DisConnectSucess, "");
        }
        catch (Exception)
        {

            this.callBackDisConnect(true, ErrorSocket.DisConnectSucess, "disconnect failed");
        }
    }
    #endregion
}
