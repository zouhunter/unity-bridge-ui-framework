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
using Menu_Generic;

namespace BridgeUI.Common
{
    /// <summary>
    /// 右键菜单
    /// </summary>
    public class RadiaMenuPanel : SinglePanel
    {
        [SerializeField]
        private RadialMenu m_radiaPrefab;
        private RadialMenu _radiaMenu;

        protected override void Awake()
        {
            base.Awake();
            InitRadiaMenu();
        }

        private void InitRadiaMenu()
        {
            _radiaMenu = Instantiate(m_radiaPrefab);
            _radiaMenu.transform.SetParent(transform,false);
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);
            if (data is UnityAction<RadialMenu>)
            {
                _radiaMenu.Reset();
                (data as UnityAction<RadialMenu>).Invoke(_radiaMenu);
                _radiaMenu.SetPosition(Input.mousePosition);
                _radiaMenu.ActivateMenu();
            }
        }
    }
}

