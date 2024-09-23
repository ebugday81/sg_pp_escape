using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoExample : MonoBehaviour
{

    public Text GestureIDText;
    public Text GestureNameText;

    void Awake()
    {
        GetComponent<SmartMotionExpressManager>().GestureIDHandleEvent += getGestureID;
        GetComponent<SmartMotionExpressManager>().GestureNameHandleEvent += getGestureName;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void getGestureID(int value)
    {
        GestureIDText.text = "Gesture ID : " + value.ToString();
    }

    void getGestureName(string str)
    {
        GestureNameText.text = "Gesture Name : " + str;
    }
}
