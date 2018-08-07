using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.uTween;
using System;

namespace BridgeUI.PanelAnim
{
    public class GroupAnimPlayer : AnimPlayer
    {
        [SerializeField]
        private List<AnimPlayer> childPlayers;
        public override void SetContext(MonoBehaviour context)
        {
            base.SetContext(context);
            foreach (var item in childPlayers)
            {
                item.SetContext(context);
            }
        }
        protected override List<uTweener> CreateTweeners()
        {
            var tweenList = new List<uTweener>();
            foreach (var item in childPlayers)
            {
                tweenList.AddRange(item.tweeners);
            }
            return tweenList;
        }
    }
}