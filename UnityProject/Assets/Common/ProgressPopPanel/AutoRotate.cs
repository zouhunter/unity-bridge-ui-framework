using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class AutoRotate : MonoBehaviour {
    public Vector3 dir;
    public float speed = 10;

    private Quaternion dirQ;

    // Update is called once per frame
    void Update () {
        transform.rotation = (Quaternion.Euler(dir * speed * Time.deltaTime) * transform.rotation);
    }
}
