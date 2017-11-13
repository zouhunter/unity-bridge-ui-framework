using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]


    public class uTweenText : uTween<float> {

        public Text cacheText;

		/// <summary>
		/// number after the digit point
		/// </summary>
		public int digits;

		protected override void OnUpdate(float value, bool isFinished)
		{
			cacheText.text = (System.Math.Round(value, digits)).ToString();
		}

		public static uTweenText Begin(Text label, float from, float to, float duration, float delay) {
			uTweenText comp = Begin<uTweenText>();
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
