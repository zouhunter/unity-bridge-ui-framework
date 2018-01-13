using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;


public class SingleImagePanel : SinglePanel
{
    public UnityEvent openEvent;
    public Button closeBtn;
    public Image image;
    public Text title;

    public string OpenKey
    {
        get
        {
            return "openTheroryPanel";
        }
    }
    
    [System.Serializable]
    public class SData
    {
        public string title;
        public Sprite sprite;
        public SData(string title,Sprite sprite)
        {
            this.title = title;
            this.sprite = sprite;
        }
    }
    public void HandleMessage(object message)
    {
        if (message is SData)
        {
            ShowUIPanel((SData)message);
        }
        closeBtn.onClick.AddListener(()=> { Destroy(gameObject); });
    }

    void ShowUIPanel(SData data)
    {
        title.text = data.title;//.texture.name;
        image.sprite = data.sprite;
        openEvent.Invoke();
        image.GetComponent<LayoutElement>().preferredHeight = image.GetComponent<LayoutElement>().preferredWidth / ((float)image.sprite.texture.width / image.sprite.texture.height);
    }
    
}
