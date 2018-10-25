using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI.Control.Infinate;
using System;

[System.Serializable]
public class DeletateInfinate: IInfinateScrollDelegate
{
    public ScrollViewItem prefab;
    private List<string> _datas;
    public List<string> datas
    {
        get
        {
            if(_datas == null)
            {
                _datas = new List<string>();
                for (int i = 0; i < 1000000; i++)
                {
                    _datas.Add(i.ToString());
                }
            }
            return _datas;
        }
    }

    public ScrollViewItem GetCellView(InfinateScroll scroller, int dataIndex, int cellIndex)
    {
        ScrollViewItem cellView = scroller.GetCellView(prefab);
        if(cellView == null){
            return null;
        }
        cellView.GetComponentInChildren<Text>().text = datas[dataIndex];
        cellView.gameObject.SetActive(true);
        return cellView;
    }

    public float GetCellViewSize(InfinateScroll scroller, int dataIndex)
    {
        return 50;
    }

    public int GetNumberOfCells(InfinateScroll scroller)
    {
        return datas.Count;
    }
}

public class infinateListTest : MonoBehaviour {

    public InfinateScroll scroller;
    public DeletateInfinate rule;

    private void Start()
    {
        rule.prefab.gameObject.SetActive(false);
        scroller.Delegate = rule;
        scroller.ReloadData();
    }

}
