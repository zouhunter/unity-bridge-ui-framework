using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BridgeUI;

public class SpeedTest : MonoBehaviour {
	private void Start () {
        OpenPanelTest();
    }

    private void OpenPanelTest()
    {
        var startTime = System.DateTime.Now;
        UIFacade.Instence.Open("PopupPanel", new string[] { "标题", "内容" }).RegistCreate(
            (x) => { var endTime = System.DateTime.Now;
                var openTime = endTime - startTime;
                Debug.Log("打开一个通知面板的时间是：" + openTime.Milliseconds);
            }
            );
       
    }
}
