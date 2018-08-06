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
using System;

namespace BridgeUI.Common
{
    /// <summary>
    /// 右键菜单
    /// </summary>
    public class RadiaMenuPanel : SinglePanel
    {
        [SerializeField]
        private Control.RadialMenu m_radiaPrefab;
        private Control.RadialMenu _radiaMenu;

        protected override void Awake()
        {
            base.Awake();
            InitRadiaMenu();
        }

        private void InitRadiaMenu()
        {
            _radiaMenu = Instantiate(m_radiaPrefab);
            _radiaMenu.transform.SetParent(transform, false);
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);
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

