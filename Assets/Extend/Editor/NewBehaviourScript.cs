using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewBehaviourScript : MonoBehaviour {
    public List<string> texts = new List<string>();
	// Use this for initialization
	void Start () {
        Test<string>("哈哈");
        Test(1);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void Test<T>(T a)
    {
        Debug.Log(a);
    }
}
