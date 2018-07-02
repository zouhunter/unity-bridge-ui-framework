#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.绑定目标面板id和本地button
* ************************************************************************************/
#endregion
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace BridgeUI.Common
{
    /// <summary>
    /// 只有一些button的面板
    /// </summary>
    public class ButtonsPanel : SinglePanel
    {
        [SerializeField, HideInInspector]
        protected List<Button> btns;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < btns.Count; i++)
            {
                var index = i;
                btns[index].onClick.AddListener(() =>
                {
                    this.Open(index,GetData(index));
                });
            }
        }

        public virtual int GetPort(int index)
        {
            return index;
        }
        public virtual object GetData(int index)
        {
            return null;
        }
    }
}