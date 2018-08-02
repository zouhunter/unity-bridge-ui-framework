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
    public class SelectAblesPanel : SinglePanel
    {
        [SerializeField, HideInInspector]
        protected List<Selectable> selectables;

        protected override void Awake()
        {
            base.Awake();
            RegistSelectables();
        }

        public virtual void RegistSelectables()
        {
            for (int i = 0; i < selectables.Count; i++)
            {
                var index = i;
                var item = selectables[i];

                if(item is Button)
                {
                    (item as Button).onClick.AddListener(() =>
                    {
                        this.Open(GetPort(index), GetData(index));
                    });
                }
                else if(item is Toggle)
                {
                    (item as Toggle).onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            this.Open(GetPort(index), GetData(index));
                        }
                        else
                        {
                            this.Close(GetPort(index));
                        }
                    });
                }
              
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