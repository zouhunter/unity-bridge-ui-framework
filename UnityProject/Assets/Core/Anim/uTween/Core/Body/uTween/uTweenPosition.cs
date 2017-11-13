using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]

    public class uTweenPosition : uTween<Vector3> {

        public RectTransform cachedRectTransform;
		public override Vector3 value {
			get { return cachedRectTransform.anchoredPosition;}
			set { cachedRectTransform.anchoredPosition = value;}
		}
		
		protected override void OnUpdate (float factor, bool isFinished)
		{
			value = from + factor * (to - from);
		}
		
		public static uTweenPosition Begin(RectTransform trans, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenPosition comp = Begin<uTweenPosition>();
            comp.cachedRectTransform = trans;
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
