using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]

    public class uTweenFloat : uTween<float> {

		protected override void OnUpdate(float factor, bool isFinished)
		{
            value = from + factor * (to - from);
		}

        public static uTweenFloat Begin(RectTransform trans, float from, float to, float duration, float delay)
        {
            uTweenFloat comp = Begin<uTweenFloat>();
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
