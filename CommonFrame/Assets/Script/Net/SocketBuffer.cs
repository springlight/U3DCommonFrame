using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//消息结构 4byte（head) + 2(msgid) +（body)
public class SocketBuffer
{
   //消息头
    private byte[] headbyte;
    private byte headLength = 6;
    private byte[] allRecvData;//接受到的数据
    //当前接受到的数据长度
    private int curRecvLength;
    private int allDataLength;//总共接受的数据长度

    public delegate void CallBackRecvOver(byte[] allData);
    CallBackRecvOver callBackRecvOver;
    public SocketBuffer(byte tmpHeadLength,CallBackRecvOver tmpOver)
    {
        headLength = tmpHeadLength;
        callBackRecvOver = tmpOver;
        headbyte = new byte[headLength];
    }

    public void RecvByte(byte[] recvByte,int realLength)
    {
        if (realLength == 0) return;
        //当前接受的数据，小于头的长度
        if(curRecvLength < headbyte.Length)
        {
            RecvHead(recvByte, realLength);
        }
        else
        {
            //接受的总长度
            int tmpLength = curRecvLength + realLength;
            //接受的消息刚好和一条消息长度相等
            if (tmpLength == allDataLength)
            {
                RecvOneAll(recvByte, realLength);
            }
            //接受的数据比这个消息的长度大
            //比如，一个消息长度只有1020个，但是一下子取出来了1024个
            //就导致了多取出了下一条数据
            else if(tmpLength > allDataLength)
            {
                RecvLarger(recvByte, realLength);
            }
            else
            {
                //该条消息长度是1028，取出来的是1024，少取出了数据
                RecvSmaller(recvByte, realLength);
            }
        }
    }

    private void RecvLarger(byte [] recvByte,int realLength)
    {
        //本条数据还差多少
        int tmpLength = allDataLength - curRecvLength;
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, tmpLength);
        curRecvLength += tmpLength;
        //一条消息取出来了
        RecvOneMsgOver();
        //本次多取的数据还差多少
        int remainLength = realLength - tmpLength;
        byte[] remainByte = new byte[remainLength];
        Buffer.BlockCopy(recvByte, tmpLength, remainByte, 0, remainLength);
        RecvByte(remainByte, remainLength);

    }

    private void RecvSmaller(byte[] recvByte, int realLength)
    {
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, realLength);
        curRecvLength += realLength;
    }
    private void RecvOneAll(byte[] recvByte,int realLength)
    {
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, realLength);
        curRecvLength += realLength;
        RecvOneMsgOver();
    }

    private void RecvHead(byte [] recvByte ,int realLength)
    {
        //差多少个字节才能组成一个头
        int tmpReal = headbyte.Length - curRecvLength;
        //现在接收的和已经接受的总长度是多少
        int tmpLength = curRecvLength + realLength;
        //总长度还小于头
        if(tmpLength < headbyte.Length)
        {
            Buffer.BlockCopy(recvByte, 0, headbyte, curRecvLength, realLength);
            curRecvLength += realLength;
        }
        else
        {
            //可以凑齐头部
            Buffer.BlockCopy(recvByte, 0, headbyte, curRecvLength, tmpReal);
            curRecvLength += tmpReal;

            //取出4个字节，表示数据的总长度,前4个字节不包含头部的长度，只表示消息的长度
            allDataLength = BitConverter.ToInt32(headbyte,0) + headLength;
            //body + head一整条数据
            allRecvData = new byte[allDataLength];
            //把头部的数据都copy到allRecvData中
            Buffer.BlockCopy(headbyte, 0, allRecvData, 0, headLength);

            //表示还剩多少数据没接受
            int tmpRemain = realLength - tmpReal;
            if(tmpRemain > 0)
            {
                byte[] tmpByte = new byte[tmpRemain];
                //表示将剩下的字节，传入tmpbyte
                Buffer.BlockCopy(recvByte, tmpReal, tmpByte, 0, tmpRemain);
                //继续处理剩下的数据
                RecvByte(tmpByte, tmpRemain);
            }
            else
            {
                //只有消息头的情况
                RecvOneMsgOver();
            }
        }
    }

 
    /// <summary>
    /// 一条消息接收完了
    /// </summary>
    private void RecvOneMsgOver()
    {
        if (callBackRecvOver != null)
            callBackRecvOver(allRecvData);
        curRecvLength = 0;
        allRecvData = null;
        allDataLength = 0;
    }
}
