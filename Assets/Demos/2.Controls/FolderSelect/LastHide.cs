using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LastHide : UIBehaviour {
    [SerializeField]
    private GameObject target;

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        if(gameObject && gameObject.activeInHierarchy)
            StartCoroutine(UpdateItemView());
    }

    private IEnumerator UpdateItemView()
    {
        if (transform.parent == null) yield break;
        yield return null;
        var index = transform.GetSiblingIndex();
        if (index == (transform.parent.childCount - 1))
        {
            target.SetActive(false);
        }
        else
        {
            target.SetActive(true);
        }
    }
}
