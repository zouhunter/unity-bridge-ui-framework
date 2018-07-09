using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Control
{
    [System.Serializable]
    public class ToolData
    {
        public string title;
        public Sprite sprite;
        public Texture texture;
        public GameObject prefab;
        public ToolData() { }
        public ToolData(string title, Texture texture, GameObject prefab)
        {
            this.title = title;
            this.texture = texture;
            this.prefab = prefab;
        }
        public ToolData(string title, Sprite sprite, GameObject prefab)
        {
            this.title = title;
            this.sprite = sprite;
            this.prefab = prefab;
        }
    }
}