using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using BridgeUI;

public class Panel : SingleCloseAblePanel {
    public string subPanelName;
    public Button subBtn;
    protected override void Awake()
    {
        base.Awake();
        subBtn.onClick.AddListener(()=> {
            this.Open(subPanelName);
        });
    }
    
}
