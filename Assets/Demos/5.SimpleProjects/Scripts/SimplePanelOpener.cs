using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;

public class SimplePanelOpener : MonoBehaviour
{
    public void Open(int id)
    {
        var panelBase = GetComponentInParent<IDiffuseView>();
        if(panelBase != null)
        {
            panelBase.Open(id);
        }
    }
    public void Open(string id)
    {
        var panelBase = GetComponentInParent<IDiffuseView>();
        if (panelBase != null)
        {
            panelBase.Open(id);
        }
        else
        {
            UIFacade.Instence.Open(id);
        }
    }
}
