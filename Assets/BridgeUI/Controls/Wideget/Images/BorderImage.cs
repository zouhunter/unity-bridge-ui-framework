using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace BridgeUI.Control
{
    public class BorderImage : Image
    {
        [SerializeField]
        private float left;
        [SerializeField]
        private float right;
        [SerializeField]
        private float top;
        [SerializeField]
        private float bottom;

        private Vector3[] coners = new Vector3[4];

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            rectTransform.GetLocalCorners(coners);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            vh.AddUIVertexQuad(GetLine(coners[0], coners[1], left, color));
            vh.AddUIVertexQuad(GetLine(coners[1], coners[2], top, color));
            vh.AddUIVertexQuad(GetLine(coners[2], coners[3], right, color));
            vh.AddUIVertexQuad(GetLine(coners[3], coners[0], bottom, color));
        }
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="line_width"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static UIVertex[] GetLine(Vector2 start, Vector2 end, float line_width, Color color, float span = 0)
        {
            Vector2 v1 = end - start;//沿线方向
            Vector2 v2 = new Vector2(v1.y, -v1.x).normalized;//垂直方向
            v2 *= line_width;
            Vector2 spanV2 = v2 * span;
            Vector2[] pos = new Vector2[4];
            pos[0] = start + v2 - spanV2;
            pos[1] = end + v2 - spanV2;
            pos[2] = end - spanV2;
            pos[3] = start - spanV2;
            return GetQuad(pos, color);
        }
        /// <summary>
        /// 画框
        /// </summary>
        /// <param name="vertPos"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static UIVertex[] GetQuad(Vector2[] vertPos, Color color)
        {
            UIVertex[] vs = new UIVertex[4];
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 0);
            uv[3] = new Vector2(1, 1);

            for (int i = 0; i < 4; i++)
            {
                UIVertex v = UIVertex.simpleVert;
                v.color = color;
                v.position = vertPos[i];
                v.uv0 = uv[i];
                vs[i] = v;
            }
            return vs;
        }
    }
}