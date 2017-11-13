using UnityEngine;
using UnityEngine.UI;

namespace uTween
{
    [System.Serializable]


    public class uTweenLayoutElement : uTween<float> {

        public enum Element
        {
            MinWidth = 0,
            MinHeight = 1,
            PreferredWidth = 2,
            PreferredHeight = 3,
            FlexibleWidth = 4,
            FlexibleHeight = 5
        }

        public Element tweenType = Element.PreferredHeight;

        public LayoutElement cachedLayoutElement;

        public override float value
        {
            get
            {
                return base.value;
            }

            set
            {
                base.value = value;
                switch (tweenType)
                {
                    case Element.MinWidth:
                        cachedLayoutElement.minWidth = value;
                        break;
                    case Element.MinHeight:
                        cachedLayoutElement.minHeight = value;
                        break;
                    case Element.PreferredWidth:
                        cachedLayoutElement.preferredWidth = value;
                        break;
                    case Element.PreferredHeight:
                        cachedLayoutElement.preferredHeight = value;
                        break;
                    case Element.FlexibleWidth:
                        cachedLayoutElement.flexibleWidth = value;
                        break;
                    case Element.FlexibleHeight:
                        cachedLayoutElement.flexibleHeight = value;
                        break;

                }
            }
        }

        protected override void OnUpdate (float factor, bool isFinished)
		{
			value = from + factor * (to - from);
		}

		public static uTweenLayoutElement Begin(RectTransform trans, float from, float to, float duration = 1f, float delay = 0f) {
			uTweenLayoutElement comp = Begin<uTweenLayoutElement>();
            comp.value = from;
			comp.from = from;
			comp.to = to;
			comp.duration = duration;
			comp.delay = delay;
			if (duration <= 0) {
				comp.Sample(1, true);
				comp.enabled = false;
			}
			return comp;
		}
	}
}
