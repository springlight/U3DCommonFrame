
/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 14:02:47
 * 版本号：v1.0
 * 本类主要用途描述：
 * 该类是所有管理类互相通讯的中心类。各个管理类（Mgr)的交互中转站
 * 在Project面板中，点击该脚本，设置改脚本的Script Execution Order保证该类最先实例化
 *  -------------------------------------------------------------------------*/
using Assets.Script.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgrCenter : MonoBehaviour {

    public static MgrCenter ins;
    private void Awake()
    {
        ins = this;
        InitMgr();
    }
    /// <summary>
    /// 初始化各个管理模块
    /// </summary>
    private void InitMgr()
    {
        gameObject.AddComponent<UIMgr>();
        gameObject.AddComponent<NpcMgr>();
        gameObject.AddComponent<AssetMgr>();
    }


    public void SendMsg(MsgBase msg)
    {
        MgrId mgr = msg.GetMgr();
        switch (mgr)
        {
            case MgrId.AssetMgr:
                
                break;
            case MgrId.GameMgr:
                break;
            case MgrId.NPCMgr:
                break;
            case MgrId.UIMgr:
                UIMgr.ins.SendMsg(msg);
                break;
        }
    }
}
