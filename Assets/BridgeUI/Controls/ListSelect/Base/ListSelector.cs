using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Control
{
    public abstract class ListSelector : MonoBehaviour
    {
        [SerializeField]
        protected Transform m_parent;
        [SerializeField]
        protected GameObject m_prefab;

        public IntEvent onSelectID = new IntEvent();
        public IntArrayEvent onSelectIDs = new IntArrayEvent();
        public event UnityAction<GameObject> onShow;
        public event UnityAction<GameObject> onHide;
        public int currentID { get { return _currentID; } }
        public int[] currentIDs { get { return selecteds.ToArray(); } }

        public string[] options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                ClearCreated();
                SetOptions(_options);
            }
        }
        public GameObject[] CreatedItems
        {
            get
            {
                return createdItems.ToArray();
            }
        }
        public bool singleChoise { get; set; }

        protected List<int> selecteds = new List<int>();
        protected string[] _options;
        protected event UnityAction onResetEvent;
        protected List<GameObject> createdItems = new List<GameObject>();
        private GameObjectPool _objectPool;
        protected GameObjectPool objectPool
        {
            get
            {
                if (_objectPool == null)
                {
                    _objectPool = UIFacade.PanelPool;
                }
                return _objectPool;
            }
        }
        private int _currentID;
        protected virtual void Awake()
        {
            m_prefab.gameObject.SetActive(false);
        }

        protected virtual void TriggerID()
        {
            if (selecteds.Count > 0)
            {
                if (onSelectID != null)
                {
                    _currentID = selecteds[selecteds.Count - 1];
                    onSelectID.Invoke(_currentID);
                }
            }
        }
        protected virtual void TriggerIDs()
        {
            if (selecteds.Count > 0)
            {
                if (onSelectIDs != null)
                {
                    onSelectIDs.Invoke(selecteds.ToArray());
                }
            }
        }

        protected virtual void Select(int id)
        {
            if(singleChoise)
            {
                selecteds.Clear();
            }

            if (!selecteds.Contains(id))
            {
                selecteds.Add(id);
            }

            TriggerID();
        }

        protected virtual void UnSelect(int id)
        {
            if (selecteds.Contains(id))
            {
                selecteds.Remove(id);
            }
        }

        protected virtual void OnCreateItem(int id,GameObject instence)
        {
            if(onShow != null)
            {
                onShow.Invoke(instence);
            }
        }

        protected virtual void OnSaveItem(GameObject instence)
        {
            if(onHide != null)
            {
                onHide(instence);
            }
        }

        protected virtual void SetOptions(string[] options)
        {
            if (options == null) return;

            for (int i = 0; i < options.Length; i++)
            {
                var item = objectPool.GetPoolObject(m_prefab, m_parent,false);
                createdItems.Add(item);
                OnCreateItem(i,item);
            }
        }

        protected virtual void ClearCreated()
        {
            foreach (var item in createdItems)
            {
                objectPool.SavePoolObject(item, false);
                OnSaveItem(item);
            }

            createdItems.Clear();

            if (onResetEvent != null)
            {
                onResetEvent.Invoke();
                onResetEvent = delegate { };
            }
        }
    }
}