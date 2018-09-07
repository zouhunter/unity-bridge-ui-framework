
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BridgeUI.Control.Chart
{
    public static class SpringGUIDefaultControls
    {
        public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }

        private static readonly Vector2 _defaultFunctionalGraphSize = new Vector2(300,300);
        private static readonly Vector2 _defaultPieGraphSize = new Vector2(200,200);
        //private static readonly Vector2 _defaultUITreeSize = new Vector2(300 , 400);
        //private static readonly Vector2 _defaultUITreeNodeSize = new Vector2(300 , 25);
        //private static readonly Vector2 _defaultCalendarSize = new Vector2(220 , 160);
        //private static readonly Vector2 _defaultDatePickerSize = new Vector2(180 , 25);
        //private static readonly Vector2 _defaultColoredTapeSize = new Vector2(20 , 200);
        //private static readonly Vector2 _defaultColorPicker = new Vector2(237 , 421);
        private static readonly Vector2 _defaultRadarMap = new Vector2(250 , 250);
        private static readonly Vector2 _defaultBarChart = new Vector2(450,300);

        private static GameObject CreateUIElementRoot( string name , Vector2 size )
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject( string name ,Transform parent ,Vector2 size  )
        {
            var go = CreateUIObject(name, parent);
            go.GetComponent<RectTransform>().sizeDelta = size;
            return go;
        }
        private static GameObject CreateUIObject( string name ,Transform parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go,parent);
            return go;
        }
        private static void SetParentAndAlign( GameObject child,Transform parent )
        {
            if(null == parent)
                return;
            child.transform.SetParent(parent,false);
            SetLayerRecursively(child, parent.gameObject.layer);
        }
        private static void SetLayerRecursively(GameObject child,int layer)
        {
            child.layer = layer;
            Transform t = child.transform;
            for ( int i = 0 ; i < t.childCount ; i++ )
                SetLayerRecursively(t.GetChild(i).gameObject , layer);
        }
        private static DefaultControls.Resources convertToDefaultResources( Resources resources )
        {
            DefaultControls.Resources res = new DefaultControls.Resources();
            res.background = resources.background;
            res.checkmark = resources.checkmark;
            res.dropdown = resources.dropdown;
            res.inputField = resources.inputField;
            res.knob = resources.knob;
            res.mask = resources.mask;
            res.standard = resources.standard;
            return res;
        }

        /// <summary>
        /// Create functional graph
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateFunctionalGraph( Resources resources )
        {
            GameObject functionalGraph = CreateUIElementRoot("FunctionalGraph" , _defaultFunctionalGraphSize);
            return functionalGraph;
        }

        /// <summary>
        /// Create pie graph
        /// </summary>
        /// <returns></returns>
        public static GameObject CreatePieGraph( Resources resources )
        {
            GameObject pieGraph = CreateUIElementRoot("PieGraph" , _defaultPieGraphSize);
            return pieGraph;
        }

        /// <summary>
        /// Create Line Chart Graph
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static GameObject CreateLineChartGraph( Resources resources )
        {
            // line chart 
            GameObject lienChart = CreateUIElementRoot("LineChart" , new Vector2(425 , 200));

            // x axis unit
            GameObject xUnit = DefaultControls.CreateText(convertToDefaultResources(resources));
            xUnit.transform.SetParent(lienChart.transform);
            var xrect = xUnit.GetComponent<RectTransform>();
            xrect.pivot = new Vector2(1 , 0.5f);
            xUnit.transform.localPosition = new Vector3(-215,-100);

            // y axis unit 
            GameObject yUnit = DefaultControls.CreateText(convertToDefaultResources(resources));
            yUnit.transform.SetParent(lienChart.transform);
            var yrect = yUnit.GetComponent<RectTransform>();
            yrect.pivot = new Vector2(0.5f , 0f);
            yrect.transform.localPosition = new Vector3(-212.5f , 105);
            return lienChart;
        }

        /// <summary>
        /// Create Radar Map
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static GameObject CreateRadarMap( Resources resources )
        {
            GameObject radarmap = CreateUIElementRoot("RadarMap" , _defaultRadarMap);
            return radarmap;
        }

        /// <summary>
        /// Create the bar chart.
        /// </summary>
        /// <returns>The bar chart.</returns>
        /// <param name="resources">Resources.</param>
        public static GameObject CreateBarChart( Resources resources )  
        {
            GameObject barChart = CreateUIElementRoot("BarChart", _defaultBarChart);
            Vector2 size = barChart.GetComponent<RectTransform>().sizeDelta;
            Vector2 origin = new Vector2(-size.x/ 2.0f,-size.y/2.0f);
            //create horizontal unit text template
            GameObject horizontalUnit = DefaultControls.CreateText(convertToDefaultResources(resources));
            horizontalUnit.name = "HorizontalUnitTemplate";
            horizontalUnit.transform.SetParent(barChart.transform);
            RectTransform hRect = horizontalUnit.GetComponent<RectTransform>();
            hRect.pivot = new Vector2(1,0.5f);
            hRect.transform.localPosition = origin + new Vector2(-5,0);
            Text hText = hRect.GetComponent<Text>();
            hText.alignment = TextAnchor.MiddleRight;
            hText.text = "Horizontal Unit";
            //create vertical   unit text template
            GameObject verticalUnit = DefaultControls.CreateText(convertToDefaultResources(resources));
            verticalUnit.name = "VerticalUnitTemplate";
            verticalUnit.transform.SetParent(barChart.transform);
            RectTransform vRect = verticalUnit.GetComponent<RectTransform>();
            vRect.pivot = new Vector2(0.5f,1);
            vRect.transform.localPosition = origin + new Vector2(0,-5);
            Text vText = verticalUnit.GetComponent<Text>();
            vText.alignment = TextAnchor.UpperCenter;
            vText.text = "Vertical Unit";
            return barChart;
        }
    }
}