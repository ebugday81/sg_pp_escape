using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using UnityEngine.UI;
using System;
using System.IO;


public class SmartMotionExpressManager : MonoBehaviour
{

    // Use this for initialization
    AndroidJavaObject InterFaceMain = null;
    string CnnTestFile;
    string SWTSFile;
    string TrainingConfigFile;
    bool initial = false;

    public delegate void GestureIDHandle(int value);
    public GestureIDHandle GestureIDHandleEvent;

    public delegate void GestureNameHandle(string str);
    public GestureNameHandle GestureNameHandleEvent;

    void Start()
    {
        
#if !UNITY_EDITOR && UNITY_ANDROID
			CnnTestFile =  Application.persistentDataPath + "/test.proto";
			SWTSFile =  Application.persistentDataPath + "/SWTS.model";
			TrainingConfigFile = Application.persistentDataPath + "/training.config";

			InterFaceMain = new AndroidJavaObject(ConstantValue.GestureDeterminePlugin);			
			StartCoroutine( GestureDataInit());
						
#elif UNITY_EDITOR
#endif

    }

    void OnApplicationQuit()
    {
        Debug.Log("SmartMotion : del start.");
        InterFaceMain.CallStatic<int>("deleteSmartMotion");
        Debug.Log("SmartMotion : del pass.");
    }

    void OnApplicationPause()
    {
        Application.Quit();
    }

    void OnApplicationFocus(bool bt)
    {
        Application.Quit();
    }

    IEnumerator GestureDataInit()
    {
        if (!File.Exists(CnnTestFile) || !File.Exists(SWTSFile) || !File.Exists(TrainingConfigFile))
        {
            Debug.Log("SmartMotion : load model start.");

            WWW www = new WWW(Application.streamingAssetsPath + "/test.proto");
            yield return www;
            System.IO.File.WriteAllBytes(CnnTestFile, www.bytes);

            Debug.Log("SmartMotion : load CnnTestFile OK.");

            www = new WWW(Application.streamingAssetsPath + "/SWTS.model");
            yield return www;
            System.IO.File.WriteAllBytes(SWTSFile, www.bytes);

            Debug.Log("SmartMotion : load SWTSFile OK.");

            www = new WWW(Application.streamingAssetsPath + "/training.config");
            yield return www;
            System.IO.File.WriteAllBytes(TrainingConfigFile, www.bytes);

            Debug.Log("SmartMotion : load TrainingConfigFile OK.");

            www = null;
        }
        Debug.Log("SmartMotion : init start.");
        InterFaceMain.CallStatic<int>("initSmartMotion", CnnTestFile, SWTSFile, TrainingConfigFile);
        Debug.Log("SmartMotion : init pass.");

        initial = true;
        yield return 0;
    }

    void FixedUpdate()
    {
        if (!initial)
        {
            return;
        }

        int label = -2;

        float[] sensorData = SensorData();

#if !UNITY_EDITOR && UNITY_ANDROID
        label = InterFaceMain.CallStatic<int>("getActivityPredict",  sensorData);                        	
#endif

        if (label > 0)
        {
            Debug.Log("SmartMotion : getActivityPredictLabel = " + label);
            GestureIDHandleEvent(label);
            Debug.Log("SmartMotion : getActivityPredictName = " + GetLabelName(label));
            GestureNameHandleEvent(GetLabelName(label));
        }
    }


    protected float[] SensorData()
    {
        float[] SensorData = new float[11];

        SensorData[0] = Time.time;
        
        SensorData[1] = GvrControllerInput.Orientation.w;
        SensorData[2] = GvrControllerInput.Orientation.x;
        SensorData[3] = GvrControllerInput.Orientation.y;
        SensorData[4] = GvrControllerInput.Orientation.z;
        SensorData[5] = GvrControllerInput.Gyro.x;
        SensorData[6] = GvrControllerInput.Gyro.y;
        SensorData[7] = GvrControllerInput.Gyro.z;
        SensorData[8] = GvrControllerInput.Accel.x;
        SensorData[9] = GvrControllerInput.Accel.y;
        SensorData[10] = GvrControllerInput.Accel.z;

        return SensorData;
    }

    public string getProductVersion
    {
        get
        {
            return InterFaceMain.CallStatic<string>("getProductVersion");
        }
    }

    string GetLabelName(int label)
    {
        switch (label)
        {
            case 0: return "non";
            case 1: return "triangle";
            case 2: return "tick";
            case 3: return "heart";
        }
        
        return "non";
    }
}
