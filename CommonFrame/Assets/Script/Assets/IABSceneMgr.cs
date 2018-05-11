/*-------------------------------------------------------------------------
 * 版权所有：langzi.guo
 * 作者：langzi.guo
 * 联系方式：1184068184@qq.com
 * 创建时间： 2018/5/10 18:33:21
 * 版本号：v1.0
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using Assets.Script.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.Script.Assets
{
    
   public  class IABSceneMgr
    {
        private IABMgr abMgr;
        private Dictionary<string, string> allAsset = new Dictionary<string, string>();
        public IABSceneMgr(string sceneName)
        {
            abMgr = new IABMgr(sceneName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        public void ReadConfiger(string sceneName)
        {
            string txtFileName = "Record.txt";
            string path = IPathTools.GetAssetBundlePath() + "/" + sceneName + txtFileName;
        }


        private void ReadConfig(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader br = new StreamReader(fs);
            ///读第一行
            string line = br.ReadLine();
            int allCnt = int.Parse(line);
            for(int I = 0; I <allCnt; i++)
            {
                string tmpStr = br.ReadLine();
                string [] arr = tmpStr.Split(" ".ToCharArray());
                allAsset.Add(arr[0], arr[1]);
            }
            br.Close();
            fs.Close();
        }

        
    }
    
}
