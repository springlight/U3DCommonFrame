
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 11:15:59
 * 版本号：v1.0
 * 本类主要用途描述：框架通用工具类
 *  -------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTool
{
    public  const int IdSpan = 3000;
}

public enum MgrId
{
    GameMgr = 0,
    UIMgr = FrameTool.IdSpan,
    NPCMgr = FrameTool.IdSpan * 2,
    AssetMgr = FrameTool.IdSpan * 3


}
