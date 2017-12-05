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
using BridgeUI.Model;
using BridgeUI;
using System;

//public class DataDic : IDictionary
//{
//    public object this[object key]
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }

//        set
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public int Count
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public bool IsFixedSize
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public bool IsReadOnly
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public bool IsSynchronized
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public ICollection Keys
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public object SyncRoot
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public ICollection Values
//    {
//        get
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public void Add(object key, object value)
//    {
//        throw new NotImplementedException();
//    }

//    public void Clear()
//    {
//        throw new NotImplementedException();
//    }

//    public bool Contains(object key)
//    {
//        throw new NotImplementedException();
//    }

//    public void CopyTo(Array array, int index)
//    {
//        throw new NotImplementedException();
//    }

//    public IDictionaryEnumerator GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }

//    public void Remove(object key)
//    {
//        throw new NotImplementedException();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        throw new NotImplementedException();
//    }
//}

public class Demo : MonoBehaviour
{
    private const string pane01 = "Panel01";
    private IUIFacade uiFacade;
    private void Awake()
    {
        uiFacade = UIFacade.Instence;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Open:MainPanel"))
        {
            var dic = new Dictionary<string,object>();
            dic["title"] = "我是主面板";
            uiFacade.Open(PanelNames.MainPanel, dic);
        }
        for (int i = 0; i < 2; i++)
        {
            if(GUILayout.Button("Open:Panel01  " + i))
            {
                OpenPanel01(i);
            }
        }
        if (GUILayout.Button("Close " + pane01))
        {
            uiFacade.Close(pane01);
        }
        if (GUILayout.Button("Hide " + pane01))
        {
            uiFacade.Hide(pane01);
        }
    }

    private void OpenPanel01(int index)
    {
        var dic = new Hashtable();
        dic[0] = "我是panel01";
        var handle = uiFacade.Open(pane01, index + "你好panel01");
        Debug.Log(index + "handle:" + handle);
        handle.RegistCallBack((panel, data) =>
        {
            Debug.Log(index + "onCallBack" + panel + ":" + data);
        });
        handle.RegistCreate((panel) =>
        {
            Debug.Log(index + "onCreate:" + panel);
        });
        handle.RegistClose((panel) =>
        {
            Debug.Log(index + "onCloese:" + panel);
        });

        handle.Send(dic);
    }
}
