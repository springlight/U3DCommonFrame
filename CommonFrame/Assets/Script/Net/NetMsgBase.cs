using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetMsgBase : MsgBase {

    public byte[] buffer;
    public NetMsgBase(byte [] arr)
    {
        buffer = arr;
        //消息头= （head) 4字节 + 2字节（的msgid)
        //从第四节开始出去两个字节就是消息id
        this.MsgId = BitConverter.ToUInt16(arr, 4);
    }


}
