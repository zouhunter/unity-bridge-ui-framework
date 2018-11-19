using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Control
{
    public class RenderImage : Graphic
    {
        [SerializeField]
        private Renderer m_renderer;

        public override Material material
        {
            get
            {
                if (m_renderer == null)
                {
                    m_renderer = GetComponentInChildren<Renderer>();
                }

                if (m_renderer != null )
                {
                    if(Application.isPlaying)
                    {
                        if(m_Material != null)
                        {
                            m_renderer.material = new Material(m_Material);
                        }
                        else
                        {
                            m_renderer.material = new Material(m_renderer.material);
                        }
                        Debug.Log("new material");
                    }
                }
                return m_renderer.sharedMaterial;
            }

            set
            {
                m_renderer.material = value;
            }
        }
#if UNITY_5_6_OR_NEWER

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
    {
        if (material)
            material.color = targetColor;
    }
    public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
    {
        if (material)
        {
            var color = material.color;
            color.a = alpha;
            material.color = color;
        }
    }
    public override Color color
    {
        get
        {
            if (material)
                return material.color;
            return Color.clear;
        }

        set
        {
            Debug.Log(value);
            if (material)
                material.color = value;
        }
    }
#endif

        public override Material defaultMaterial
        {
            get
            {
                return m_Material;
            }
        }
    }
}