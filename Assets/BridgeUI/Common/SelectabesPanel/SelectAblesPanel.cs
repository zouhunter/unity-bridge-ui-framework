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
using System.Linq;
using System;

namespace BridgeUI.Common
{
    /// <summary>
    /// 只有一些button的面板
    /// </summary>
    public class SelectAblesPanel : ViewBaseComponent, IPortGroup
    {
        [HideInInspector]
        public List<Selectable> selectables;
        public virtual string[] Ports
        {
            get
            {
                return selectables.Where(x => x != null).Select(x => x.name).ToArray();
            }
        }

        protected event System.Action onRecover;

        protected override void OnInitialize()
        {
            for (int i = 0; i < selectables.Count; i++)
            {
                var index = i;
                var item = selectables[i];

                if (item is Button)
                {
                    var btn = item as Button;

                    UnityEngine.Events.UnityAction action = () =>
                    {
                        this.Open(GetPort(index), GetData(index));
                    };

                    onRecover += () =>
                    {
                        btn.onClick.RemoveListener(action);
                    };

                    btn.onClick.AddListener(action);
                }
                else if (item is Toggle)
                {
                    var toggle = (item as Toggle);
                    UnityEngine.Events.UnityAction<bool> action = (isOn) =>
                    {
                        if (isOn)
                        {
                            this.Open(GetPort(index), GetData(index));
                        }
                        else
                        {
                            this.Close(GetPort(index));
                        }
                    };

                    onRecover += () => { toggle.onValueChanged.RemoveListener(action); };

                    toggle.onValueChanged.AddListener(action);
                }

            }
        }

        protected override void OnRecover()
        {
            if (onRecover != null)
            {
                onRecover.Invoke();
                onRecover = null;
            }
        }
        public override void Close()
        {
            base.Close();

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