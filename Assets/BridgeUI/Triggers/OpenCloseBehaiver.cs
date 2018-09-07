using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Trigger
{
    [RequireComponent(typeof(PanelCore))]
    public class OpenCloseBehaiver : MonoBehaviour
    {
        [SerializeField]
        private Selectable selectable;
        [SerializeField, HideInInspector]
        private int index;
        [SerializeField, HideInInspector]
        private string panelName;
        [SerializeField]
        private bool fromIndex;

        private PanelCore panelCore;

        private void Awake()
        {
            panelCore = GetComponentInParent<PanelCore>();

            if (selectable is Button)
            {
                (selectable as Button).onClick.AddListener(OpenClosePanel);
            }
            else if (selectable is Toggle)
            {
                (selectable as Toggle).onValueChanged.AddListener(OpenClosePanel);
            }
            else if (selectable is InputField)
            {
                (selectable as InputField).onEndEdit.AddListener(OpenClosePanel);
            }
        }

        private void OpenClosePanel(bool arg0)
        {
            if (arg0)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }

        private void OpenClosePanel()
        {
            if (!IsOpened())
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }

        private bool IsOpened()
        {
            if (fromIndex)
            {
                return panelCore.IsOpen(index);
            }
            else
            {
                return panelCore.IsOpen(panelName);
            }
        }

        private void OpenClosePanel(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }

        private void OpenPanel()
        {
            if (fromIndex)
            {
                panelCore.Open(index);
            }
            else
            {
                panelCore.Open(panelName);
            }
        }

        private void ClosePanel()
        {
            if (fromIndex)
            {
                panelCore.Close(index);
            }
            else
            {
                panelCore.Close(panelName);
            }
        }
    }

}