using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    // red signal (seconds)
    public int stopPeriod = 6;
    // red signal (seconds)
    public int runPeriod = 6;
    public bool isDecorated = false;
    public bool isTimerMode = false;
    public bool showInfoPanel = true;
    public GameObject trafficLight;
    
    // yellow signal (seconds)
    private int prepPeriod = 2;
    private int blinkPeriod = 3;

    private int time = 0;
    private int circleTime;

    private int stopPrepRunStage;
    private int runStage;
    private int blinkStage;
    private int prepStopStage;

    private string currSignalInfo;

    private TrafficLightState trafficLightState;
    void Start()
    {
        InitTrafficLight();

        int minPeriod = (runPeriod < stopPeriod) ? runPeriod : stopPeriod;
        prepPeriod = (prepPeriod < minPeriod) ? prepPeriod : minPeriod;
        blinkPeriod = (blinkPeriod < runPeriod) ? blinkPeriod : runPeriod;

        circleTime = stopPeriod + prepPeriod + runPeriod + prepPeriod;
                
        stopPrepRunStage = stopPeriod;
        runStage = stopPeriod + prepPeriod;        
        prepStopStage = circleTime - prepPeriod;
        blinkStage = prepStopStage - blinkPeriod;

        time = runStage;

        StartCoroutine("TimerController");
    }

    private void OnGUI()
    {
        if (!showInfoPanel)
            return;

        Color guiBackColor = Color.gray;
        guiBackColor.a = 0.3f;

        int size = 256;
        Color[] pix = new Color[size * size];
        for (int i = 0; i < pix.Length; ++i)
            pix[i] = guiBackColor;
        
        Texture2D mGUIBackgroundTex = new Texture2D(size, size);
        mGUIBackgroundTex.SetPixels(pix);
        mGUIBackgroundTex.Apply();

        GUIStyle InfoPanelGUIStyle = new GUIStyle();
        InfoPanelGUIStyle.normal.background = mGUIBackgroundTex;
        InfoPanelGUIStyle.normal.textColor = Color.white;
        InfoPanelGUIStyle.alignment = TextAnchor.MiddleCenter;

        float infoPanelHeight = Screen.height / 20;
        GUI.Box(new Rect(0, Screen.height - infoPanelHeight, Screen.width, infoPanelHeight), currSignalInfo, InfoPanelGUIStyle);
    }

    private void InitTrafficLight()
    {
        trafficLight = GameObject.Find("TrafficLight");
        if (trafficLight == null)
        {
            trafficLight = new GameObject();
            trafficLight.name = "TrafficLight";            
        }
        TrafficLightState trafficLightState = trafficLight.GetComponent<TrafficLightState>();
        if (trafficLightState == null)
        {
            trafficLightState = trafficLight.AddComponent<TrafficLightState>();
        }
        trafficLightState.Decorated = isDecorated;
        trafficLightState.TimerMode = isTimerMode;
    }

    private IEnumerator TimerController()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);           
            OnTimerEvent();
            time++;
        }
    }

    private void OnTimerEvent()
    {
        int workCirclePart = time % circleTime;
        if (workCirclePart == 0)
        {
            trafficLight.SendMessage("StopSignal", stopPeriod, SendMessageOptions.DontRequireReceiver);
            currSignalInfo = "Stop";
        }

        if (workCirclePart == stopPrepRunStage)
        {
            trafficLight.SendMessage("StopPrepSignal", 0, SendMessageOptions.DontRequireReceiver);
            currSignalInfo = "Stop + Intermediate";
        }

        if (workCirclePart == runStage)
        {
            trafficLight.SendMessage("RunSignal", runPeriod, SendMessageOptions.DontRequireReceiver);
            currSignalInfo = "Run";
        }

        if (workCirclePart == blinkStage)
        {
            trafficLight.SendMessage("BlinkSignal", 0, SendMessageOptions.DontRequireReceiver);
            currSignalInfo = "Flashing";
        }

        if (workCirclePart == prepStopStage)
        {
            trafficLight.SendMessage("PrepSignal", 0, SendMessageOptions.DontRequireReceiver);
            currSignalInfo = "Intermediate";
        }
    }    
}
