#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.绑定目标面板名和本地button
* ************************************************************************************/
#endregion
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Common
{
    /// <summary>
    ///利用按扭打开其他面板
    /// </summary>
    [Attributes.PanelParent]
    public sealed class ButtonOpenPanel : SinglePanel
    {
        [System.Serializable]
        public class Holder
        {
            public string panelName;
            public Button button;
        }
        public List<Holder> holders = new List<Holder>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var item in holders)
            {
                if (item.button != null && !string.IsNullOrEmpty(item.panelName))
                {
                    item.button.onClick.AddListener(() => { this.Open(item.panelName); });
                }
            }
        }
    }
}
