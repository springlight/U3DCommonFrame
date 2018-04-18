
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：
 * 直接继承自MonoBehaviour，在以后的自定义类中，直接继承自MonoBase，便于功能扩展
 *  -------------------------------------------------------------------------*/

using UnityEngine;

public abstract class MonoBase : MonoBehaviour {
    /// <summary>
    /// 每一个脚本都要处理消息
    /// </summary>
    /// <param name="msg">需要处理的消息</param>
    public abstract void ProcessEvent(MsgBase msg);
}
