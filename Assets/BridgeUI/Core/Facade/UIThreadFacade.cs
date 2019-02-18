using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace BridgeUI
{
    public class UIThreadFacade : MonoBehaviour
    {
        private UIFacadeInternal uiFacade { get { return BridgeUI.UIFacade.Instence; } }
        private Queue<Action> mainThreadActions = new Queue<Action>();
        public static UIThreadFacade Instence { get; private set; }

        private void Awake()
        {
            Instence = this;
        }
        private void Update()
        {
            if(mainThreadActions.Count > 0)
            {
                var action = mainThreadActions.Dequeue();
                action.Invoke();
            }
        }
        public void Close(string panelName)
        {
            mainThreadActions.Enqueue(new Action(()=> {
                uiFacade.Close(panelName);
            }));
        }

        public void Close(IPanelGroup parentGroup, string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                uiFacade.Close(parentGroup,panelName);
            }));
        }

        public void Hide(string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                uiFacade.Hide(panelName);
            }));
        }

        public void Hide(IPanelGroup parentGroup, string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                uiFacade.Hide(parentGroup,panelName);
            }));
        }

        public void IsPanelOpen(string panelName, UnityAction<bool> onJudge = null)
        {
            mainThreadActions.Enqueue(new Action(() => {
                var isOpen = uiFacade.IsPanelOpen(panelName);
                if (onJudge != null)
                {
                    onJudge.Invoke(isOpen);
                }
            }));
        }

        public void IsPanelOpen(IPanelGroup parentGroup, string panelName,UnityAction<bool> onJudge = null)
        {
            mainThreadActions.Enqueue(new Action(() => {
                var isOpen = uiFacade.IsPanelOpen(panelName);
                if (onJudge != null)
                {
                    onJudge.Invoke(isOpen);
                }
            }));
        }

        public void Open(string panelName, object data = null)
        {
            mainThreadActions.Enqueue(new Action(() =>
            {
                uiFacade.Open(panelName, data);
            }));
        }

        public void Open(IUIPanel parentPanel, string panelName, object data = null)
        {
            mainThreadActions.Enqueue(new Action(() =>
            {
                uiFacade.Open(parentPanel, panelName, data);
            }));
        }

        public void RegistClose(UnityAction<IUIPanel> onClose)
        {
            uiFacade.RegistClose(onClose);
        }

        public void RegistCreate(UnityAction<IUIPanel> onCreate)
        {
            uiFacade.RegistCreate(onCreate);
        }

        public void RemoveClose(UnityAction<IUIPanel> onClose)
        {
            uiFacade.RemoveClose(onClose);
        }

        public void RemoveCreate(UnityAction<IUIPanel> onCreate)
        {
            uiFacade.RemoveClose(onCreate);

        }
    }
}
