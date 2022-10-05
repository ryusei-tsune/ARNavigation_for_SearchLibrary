using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_root;

    [SerializeField]
    private Text m_trueHeading;

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    void Update()
    {
        m_root.rotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
        m_trueHeading.text = ((int)Input.compass.trueHeading).ToString() + "°";
    }
}
