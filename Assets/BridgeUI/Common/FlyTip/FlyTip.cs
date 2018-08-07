using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
namespace BridgeUI.Common
{
    public class FlyTip : PanelCore
    {
        [SerializeField]
        private Text m_title;
        [SerializeField]
        private float activeTime = 2f;
        private float timer = 0;

        protected override void HandleData(object data)
        {
            ResetTime();
            m_title.text = data.ToString();
        }

        private void ResetTime()
        {
            timer = 0;
        }

        private void Update()
        {
            if ((timer += Time.deltaTime) > activeTime)
            {
                Hide();
            }
        }
    }
}