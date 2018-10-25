using UnityEngine;
using System.Collections.Generic;
using System;
using BridgeUI.Model;
using System.Reflection;
using UnityEngine.Events;

namespace BridgeUI
{
    public class UIThreadFacade : MonoBehaviour
    {
        private UIFacade UIFacade { get { return UIFacade.Instence; } }
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
                UIFacade.Close(panelName);
            }));
        }

        public void Close(IPanelGroup parentGroup, string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                UIFacade.Close(parentGroup,panelName);
            }));
        }

        public void Hide(string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                UIFacade.Hide(panelName);
            }));
        }

        public void Hide(IPanelGroup parentGroup, string panelName)
        {
            mainThreadActions.Enqueue(new Action(() => {
                UIFacade.Hide(parentGroup,panelName);
            }));
        }

        public void IsPanelOpen(string panelName, UnityAction<bool> onJudge = null)
        {
            mainThreadActions.Enqueue(new Action(() => {
                var isOpen = UIFacade.IsPanelOpen(panelName);
                if (onJudge != null)
                {
                    onJudge.Invoke(isOpen);
                }
            }));
        }

        public void IsPanelOpen(IPanelGroup parentGroup, string panelName,UnityAction<bool> onJudge = null)
        {
            mainThreadActions.Enqueue(new Action(() => {
                var isOpen = UIFacade.IsPanelOpen(panelName);
                if (onJudge != null)
                {
                    onJudge.Invoke(isOpen);
                }
            }));
        }

        public void Open(string panelName, object data = null,UnityAction<IUIHandle> onGetHandle = null)
        {
            mainThreadActions.Enqueue(new Action(() => {
              var handle =  UIFacade.Open( panelName, data);
                if(onGetHandle != null)
                {
                    onGetHandle.Invoke(handle);
                }
            }));
        }

        public void Open(IUIPanel parentPanel, string panelName, object data = null, UnityAction<IUIHandle> onGetHandle =null)
        {
            mainThreadActions.Enqueue(new Action(() => {
                var handle = UIFacade.Open(parentPanel, panelName, data);
                if (handle != null)
                {
                    onGetHandle.Invoke(handle);
                }
            }));
        }

        public void RegistClose(UnityAction<IUIPanel> onClose)
        {
            UIFacade.Instence.RegistClose(onClose);
        }

        public void RegistCreate(UnityAction<IUIPanel> onCreate)
        {
            UIFacade.Instence.RegistCreate(onCreate);
        }

        public void RemoveClose(UnityAction<IUIPanel> onClose)
        {
            UIFacade.Instence.RemoveClose(onClose);
        }

        public void RemoveCreate(UnityAction<IUIPanel> onCreate)
        {
            UIFacade.Instence.RemoveClose(onCreate);

        }
    }
}
