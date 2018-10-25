using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ThreadTest : MonoBehaviour {
    private void Start()
    {
        Thread thread = new Thread(new ThreadStart(ThreadAction));
        thread.Start();
    }

    private void ThreadAction()
    {
        var index = 0;
        while (true)
        {
            Thread.Sleep(1000);
            var randomInfo = string.Format("信息{0}", index++.ToString());
            BridgeUI.UIThreadFacade.Instence.Open(PanelNames.PopupPanel, randomInfo);
        }
       
    }
}
