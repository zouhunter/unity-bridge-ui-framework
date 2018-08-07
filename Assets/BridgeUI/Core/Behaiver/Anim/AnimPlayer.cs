#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 11:27:06
    * 说    明：       
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.uTween;
namespace BridgeUI
{
    public abstract class AnimPlayer : ScriptableObject
    {
        [SerializeField]
        private bool reverse;
        protected MonoBehaviour panel;
        protected UnityAction onComplete { get; set; }
        protected int completedTween;
        private List<uTweener> _tweeners;
        public virtual List<uTweener> tweeners
        {
            get
            {
                if (_tweeners == null){
                    _tweeners = CreateTweeners();
                }
                return _tweeners;
            }
        }
        public virtual void SetContext(MonoBehaviour context)
        {
            panel = context;
            panel.StartCoroutine(UpdateState());
        }

        private IEnumerator UpdateState()
        {
            var wait = new WaitForEndOfFrame();
            while (panel != null)
            {
                yield return wait;
                Update();
            }
        }
        protected virtual void Update()
        {
            if (tweeners != null)
            {
                foreach (var tween in tweeners)
                {
                    if (tween != null)
                    {
                        tween.Update();
                    }
                }
            }
        }
        public virtual void PlayAnim(UnityAction onComplete)
        {
            if (onComplete != null)
            {
                this.onComplete = onComplete;
            }

            if (tweeners != null)
            {
                completedTween = 0;

                foreach (var tween in tweeners)
                {
                    if (!reverse)
                    {
                        tween.ResetToBeginning();
                        tween.PlayForward();
                    }
                    else
                    {
                        tween.ResetToComplete();
                        tween.PlayReverse();
                    }

                    tween.AddOnFinished(CompleteOne);
                }
            }
        }

        protected virtual void CompleteOne()
        {
            if (++completedTween >= tweeners.Count)
            {
                completedTween = 0;
                if (onComplete != null)
                {
                    onComplete.Invoke();
                }
            }
        }

        protected abstract List<uTweener> CreateTweeners();
    }
}
