using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]


    public class uTweenRect : uTween<Vector2> {

		public RectTransform mRectTransform;
		public RectTransform cacheRectTransform {
			get {
				return mRectTransform;
			}
		}

		protected override void OnUpdate (float factor, bool isFinished) {
			value = from + factor * (to - from);
			cacheRectTransform.sizeDelta = value;
		}

		public static uTweenRect Begin(RectTransform go, Vector2 from, Vector2 to, float duration, float delay) {
			uTweenRect comp = Begin<uTweenRect>();
            comp.value = from;
			comp.from = from;
			comp.to = to;
			comp.delay = delay;
			
			if (duration <=0) {
				comp.Sample(1, true);
				comp.enabled = false;
			}
			return comp;
		}
	}
}
