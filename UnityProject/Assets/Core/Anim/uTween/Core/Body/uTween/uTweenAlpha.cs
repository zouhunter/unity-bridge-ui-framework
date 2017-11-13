using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uTween {
    [System.Serializable]

    public class uTweenAlpha : uTween<float> {

		public GameObject target;
		public bool includeChildren = false;

		Graphic[] mGraphics;
        Graphic[] cachedGraphics
        {
            get
            {
                if (mGraphics == null)
                {
                    mGraphics = includeChildren ? target.GetComponentsInChildren<Graphic>() : target.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }

		float mAlpha = 0f;

		public override void StartTween ()
		{		
			base.StartTween ();
		}

		public float alpha {
			get {
				return mAlpha;
			}
			set {
				SetAlpha(target.transform, value);
				mAlpha = value;
			}
		}

		protected override void OnUpdate (float value, bool isFinished)
		{
			alpha = value;
		}

		void SetAlpha(Transform _transform, float _alpha) {
			foreach (var item in cachedGraphics) {
				Color color = item.color;
				color.a = _alpha;
				item.color = color;
			}
		}

        public static uTweenAlpha Begin(RectTransform trans, float from, float to, float duration = 1f, float delay = 0f)
        {
            uTweenAlpha comp = Begin<uTweenAlpha>();
            comp.value = from;
            comp.from = from;
            comp.to = to;
            comp.duration = duration;
            comp.delay = delay;
            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }


    }

}