using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Common
{
    /// <summary>
    /// 右键菜单
    /// </summary>
    public class RadiaMenuPanel : ViewBaseComponent
    {
        [SerializeField]
        private Control.RadialMenu m_radiaPrefab;
        public Control.RadialMenu MenuPrefab { get { return m_radiaPrefab; } }
 
        private Control.RadialMenu _radiaMenu;

        protected override void OnInitialize()
        {
            _radiaMenu = Object.Instantiate(MenuPrefab);
            _radiaMenu.transform.SetParent(transform, false);
        }
        protected override void OnRecover()
        {
            GameObject.Destroy(_radiaMenu.gameObject);
        }
        protected override void OnMessageReceive(object data)
        {
            if (data is UnityAction<Control.RadialMenu>)
            {
                _radiaMenu.Reset();
                (data as UnityAction<Control.RadialMenu>).Invoke(_radiaMenu);
                _radiaMenu.SetPosition(Input.mousePosition);
                _radiaMenu.ActivateMenu();
            }
        }
    }
}

