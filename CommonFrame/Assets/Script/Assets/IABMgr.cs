/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/4/27 14:11:37
 * 版本号：v1.0
 * 本类主要用途描述：
 * 对一个场景所有的bundle包进行管理
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Assets
{
    public class IABMgr
    {
        /// <summary>
        /// 把每个一包都存起来
        /// </summary>
        private Dictionary<string, IABRelationMgr> laodHelper = new Dictionary<string, IABRelationMgr>();
    }
}
