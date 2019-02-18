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
using BridgeUI;

public class CommonPanelTest : MonoBehaviour
{
    private void OnGUI()
    {
        using (var hor = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("PopUpPanel -string[]"))
            {
               UIFacade.Instence.Open(PanelNames.PopupPanel, new string[] { "你好", "这是一个提示面板!" });
            }
            if (GUILayout.Button("PopUpPanel -enum network"))
            {
                UIFacade.Instence.Open(PanelNames.PopupPanel, NetWorkInfos.OnNetError);
                //var handle = 
                //handle.RegistCallBack((panel, data) =>
                //{
                //    Debug.Log("call back :" + data);
                //});
            }
            if (GUILayout.Button("PopUpPanel -enum file"))
            {
                UIFacade.Instence.Open(PanelNames.PopupPanel, FileInfos.FileAlreadyExists);
            }
            if (GUILayout.Button("PopUpPanel -dic"))
            {
                var table = new Hashtable();
                table["title"] = "标题 - HashTable";
                table["info"] = "信息 - HashTable";
                table["donthide"] = false;
                UIFacade.Instence.Open(PanelNames.PopupPanel, table);
            }
        }

    }
}
