/// Credit zge
/// Sourced from - http://forum.unity3d.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2293224

using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/UI Mesh")]
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public class UIMesh : MaskableGraphic
    {
        [SerializeField]
        private Mesh m_mesh;

        [SerializeField]
        private List<Material> m_materials;

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return m_mesh;
            }
            set
            {
                if (m_mesh == value)
                    return;

                m_mesh = value;
                SetVerticesDirty();
                SetMaterialDirty(); 
                //ResetData();
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public List<Material> Materials
        {
            get
            {
                return m_materials;
            }
            set
            {
                if (m_materials == value)
                    return;

                m_materials = value;
                SetVerticesDirty();
                SetMaterialDirty(); 
                //ResetData();
            }
        }

        //private void ResetData()
        //{
        //    if (m_mesh && false)
        //    {
        //        Debug.Log("Drawing Mesh");
        //        var cr = GetComponent<CanvasRenderer>();
        //        cr.SetMesh(m_mesh);
        //        if (m_materials != null && m_materials.Count > 0)
        //        {
        //            Debug.Log("Adding Materials");
        //            cr.materialCount = m_materials.Count;

        //            for (int i = 0; i < m_materials.Count; i++)
        //                cr.SetMaterial(m_materials[i], i);
        //        }
        //    }
        //}

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Debug.Log("Populate Mesh Data");
            vh.Clear();
            vh.FillMesh(m_mesh);
            base.OnPopulateMesh(vh);
        }


        //protected override void OnEnable()
        //{
        //    Debug.Log("Reset Data");
        //    ResetData();
        //    SetAllDirty();
        //    base.OnEnable();
        //}

        //protected override void OnValidate()
        //{
        //    Debug.Log("Validate Data");
        //    ResetData();
        //    base.OnValidate();
        //}
    }
}