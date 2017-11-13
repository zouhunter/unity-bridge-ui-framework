using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace uTween {
	[System.Serializable]
	public class uTweenScale : uTween<Vector3> {

        public RectTransform cachedRectTransform;
		public override Vector3 value {
			get { return cachedRectTransform.localScale;}
			set { cachedRectTransform.localScale = value;}
		}

		protected override void OnUpdate (float factor, bool isFinished)
		{
			value = from + factor * (to - from);
		}

		public static uTweenScale Begin(RectTransform trans, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenScale comp = Begin<uTweenScale>();
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
