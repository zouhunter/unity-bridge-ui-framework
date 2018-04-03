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
    public class SingleImagePanel : SingleCloseAblePanel
    {
        [SerializeField]
        private Image m_image;
        [SerializeField]
        private Text m_title;

        protected override void Awake()
        {
            base.Awake();
            PropBinder.Record<string>("title", (y) => { m_title.text = y; });
            PropBinder.Record<Sprite>("sprite", (y) => { m_image.sprite = y; });
        }

        protected override void HandleData(object data)
        {
            base.HandleData(data);
            if (m_image.sprite != null)
            {
                m_image.GetComponent<LayoutElement>().preferredHeight = m_image.GetComponent<LayoutElement>().preferredWidth / ((float)m_image.sprite.texture.width / m_image.sprite.texture.height);
            }
        }
    }
}
