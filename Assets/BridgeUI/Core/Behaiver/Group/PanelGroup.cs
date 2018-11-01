#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion

using System.Collections.Generic;
using BridgeUI.Graph;

namespace BridgeUI
{
    /// <summary>
    /// 用于标记ui打开的父级
    /// [3维场景中可能有多个地方需要打开用户界面]
    /// </summary>
    public class PanelGroup : PanelGroupBase
    {
        public List<Graph.UIGraph> graphList = new List<Graph.UIGraph>();
        public override List<UIGraph> GraphList
        {
            get
            {
                return graphList;
            }
        }
        private void Awake()
        {
            LunchPanelGroupSystem();
        }
    }
}