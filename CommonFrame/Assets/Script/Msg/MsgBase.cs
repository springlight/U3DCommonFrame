/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：
 * 整个框架的消息基类，包涵一个消息id，该id类似于电脑ip地址，管理类Mgr通过
 * 这个id找到和这个id对应的mono脚本
 *  -------------------------------------------------------------------------*/

public class MsgBase
{
    /// <summary>
    /// 消息id
    /// 类似于电脑ip，根据msgId找到对应的mono
    /// </summary>
    private ushort msgId;
    public MsgBase()
    {
        msgId = 0;
    }
    public MsgBase(ushort msgId)
    {
        this.msgId = msgId;
    }
    /// <summary>
    /// 根据消息id，判断该消息属于哪个Mgr
    /// </summary>
    /// <returns>返回所属的MgrId</returns>
    public MgrId GetMgr()
    {
        int tmpId = msgId / FrameTool.IdSpan;
        int mgrId = tmpId * FrameTool.IdSpan;
        return (MgrId)mgrId;
    }
	
    public ushort MsgId
    {
        get { return msgId; }
        set { msgId = value; }
    }
    
}
