#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.传入包含sprite和title的字典
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Common
{
    /// <summary>
    /// 自适应图片显示面板
    /// </summary>
    public class SingleImagePanel : ViewBaseComponent
    {
        [SerializeField]
        private Image m_image;
        [SerializeField]
        private Text m_title;

        protected override void OnMessageReceive(object data)
        {
            if(data is IDictionary)
            {
                var dic = data as IDictionary;
                m_title.text = dic["title"].ToString();
                m_image.sprite = dic["sprite"] as Sprite;
            }
        }

        protected override void OnInitialize()
        {
            if (m_image.sprite != null)
            {
                m_image.GetComponent<LayoutElement>().preferredHeight = m_image.GetComponent<LayoutElement>().preferredWidth / ((float)m_image.sprite.texture.width / m_image.sprite.texture.height);
            }
        }

        protected override void OnRecover()
        {
            m_image.sprite = null;
        }
    }
}
