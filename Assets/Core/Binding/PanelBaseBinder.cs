//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Audio;
//using UnityEngine.Events;
//using UnityEngine.Sprites;
//using UnityEngine.Scripting;
//using UnityEngine.Assertions;
//using UnityEngine.EventSystems;
//using UnityEngine.Assertions.Must;
//using UnityEngine.Assertions.Comparers;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.Reflection;

//namespace BridgeUI.Binding
//{

//    public class PropertyBinder : PropertyBinder
//    {
//        public PropertyBinder(PanelBase panel) : base(panel) { }

//        /// <summary>
//        /// 接收文字显示
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="sourceName"></param>
//        internal virtual void RegistTextView(Text text, string sourceName)
//        {
//            RegistValueCharge<string>((value) => { text.text = value; }, sourceName);
//        }
//        /// <summary>
//        /// 接收图片显示
//        /// </summary>
//        /// <param name="image"></param>
//        /// <param name="sourceName"></param>
//        internal virtual void RegistImageView(Image image, string sourceName)
//        {
//            RegistValueCharge<Sprite>((value) => { image.sprite = value; }, sourceName);
//        }
//    }

//}