/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/18 14:02:47
 * 版本号：v1.0
 * 本类主要用途描述：
 * 该类主要挂载在UI中每一个交互控件上，比如Button ,Input等,向UIMgr注册Gameobject
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Script.UI
{
    public class UIBehaviour:MonoBehaviour
    {
        private void Awake()
        {
            UIMgr.ins.RegisterGameObject(name, gameObject);
        }
        public void AddBtnEvtListener(UnityAction action)
        {
            if(action != null)
            {
                Button btn = transform.GetComponent<Button>();
                btn.onClick.AddListener(action);
            }
        }

        public void RemoveBtnEvtListener(UnityAction action)
        {
            if (action != null)
            {
                Button btn = transform.GetComponent<Button>();
                btn.onClick.RemoveListener(action);
            }
        }

        private void OnDestroy()
        {
            Debug.LogError("UIBehaviour Destroy");
            UIMgr.ins.UnRegisterGameObject(name);
        }
    }
}
