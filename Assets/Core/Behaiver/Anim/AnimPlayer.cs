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
    public abstract class AnimPlayer : MonoBehaviour, IAnimPlayer
    {
        protected uTweener tween;
        public virtual void Update()
        {
           if(tween != null)
                tween.Update();
        }
        public virtual void PlayAnim(bool isEnter,UnityAction onComplete)
        {
            tween = CreateTweener(isEnter);
            if (onComplete != null){
                tween.AddOnFinished(onComplete);
            }
            tween.ResetToBeginning();
            tween.PlayForward();
        }
        protected abstract uTweener CreateTweener(bool isEnter);
    }
}
