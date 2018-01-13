using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;

public class TipInformation : SinglePanel
{
    public const string closeKey = "close";
    public InputField.OnChangeEvent onGetText;
    public float lifeTime = 10;
    public Text title;
    public Text info;
    private float timer;
    private bool autoClose = true;
    void Update()
    {
        if (autoClose)
        {
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                timer = 0f;
                Destroy(gameObject);
            }
        }
    }

    void OnReceive(string title, string info)
    {
        gameObject.SetActive(true);
        if (string.IsNullOrEmpty(title))
        {
            this.title.text = "";
        }
        else
        {
            this.title.text = title;
        }
        this.info.text = info;
        onGetText.Invoke(this.info.text);
    }

    protected override void HandleData(object message)
    {
        string title = null;
        string info = null;
        if (message is string)
        {
            info = (string)message;
        }
        else
        {
            if (message is string[])
            {
                var strs = (string[])message;

                if (strs != null)
                {
                    if (strs.Length > 1)
                    {
                        title = strs[0];
                        info = strs[1];
                    }
                    else
                    {
                        info = strs[0];
                    }
                }
            }
            else
            {
                var dic = message as Hashtable;
                var title_u = dic["title"];
                var info_u = dic["info"];
                var autoCloseData = dic["autoClose"];
                var time = dic["lifeTime"];
                if (time != null)
                {
                    lifeTime = (float)time;
                }
                if (autoCloseData != null)
                {
                    autoClose = (bool)autoCloseData;
                    timer = 0f;
                }
                if (title_u != null)
                {
                    title = (string)title_u;
                }
                if (info_u != null)
                {
                    info = (string)info_u;
                }
            }
        }

        OnReceive(title, info);
    }
}
