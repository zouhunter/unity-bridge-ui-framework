using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]

    public class uTweenRotation : uTween<Vector3> {

		RectTransform mRectTransfrom;

		public RectTransform target;

		public RectTransform cacheRectTransfrom {
			get { 
				mRectTransfrom = target;
				return mRectTransfrom;			
			}
		}

        public Quaternion QuaternionValue
        {
            get
            {
                return cacheRectTransfrom.localRotation;
            }
            set
            {
                cacheRectTransfrom.localRotation = value;
            }
        }

		protected override void OnUpdate (float _factor, bool _isFinished)
		{
            QuaternionValue = Quaternion.Euler(Vector3.Lerp(from, to, _factor));
		}

		public static uTweenRotation Begin(RectTransform trans, Vector3 from, Vector3 to, float duration = 1f, float delay = 0f) {
			uTweenRotation comp = Begin<uTweenRotation>();
            comp.target = trans;
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