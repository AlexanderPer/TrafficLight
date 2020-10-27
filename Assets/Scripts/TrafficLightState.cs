using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightState : MonoBehaviour
{    
    private GameObject trafficLight;
    private RawImage stopLampImage;
    private RawImage prepLampImage;
    private RawImage runLampImage;

    private bool decorated = false;
    private bool timerMode = false;
    private float indentPercent = 0.08f;
    private int timerPeriod = 0;

    void Awake()
    {
        CreateDashboard();
        SetDecorated(decorated);
        StartCoroutine("RunTimerPanel");
    }    
    // Create traffic light information panel
    private void CreateDashboard()
    {
        float panelHeight = Screen.height / 1.6f;
        float panelWidth = panelHeight / 3;
        float indent = panelWidth * indentPercent;
        float lampImageSize = panelWidth - 2 * indent;
        
        trafficLight = gameObject;
        trafficLight.AddComponent<Canvas>();
        Canvas trafficLightCanvas = trafficLight.GetComponent<Canvas>();
        trafficLightCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject mainPanel = new GameObject("MainPanel");
        mainPanel.AddComponent<CanvasRenderer>();
        Image im = mainPanel.AddComponent<Image>();
        im.color = Color.black;

        RectTransform rt = mainPanel.GetComponent<RectTransform>();                
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);

        mainPanel.transform.SetParent(trafficLightCanvas.transform, false);

        stopLampImage = CreateLamp(mainPanel.transform, lampImageSize, panelWidth, Color.red);
        prepLampImage = CreateLamp(mainPanel.transform, lampImageSize, 0, Color.yellow);
        runLampImage = CreateLamp(mainPanel.transform, lampImageSize, -panelWidth, Color.green);
    }

    private RawImage CreateLamp(Transform parent, float size, float posY, Color color)
    {
        GameObject trafficLamp = new GameObject("TrafficLamp");
        trafficLamp.AddComponent<CanvasRenderer>();
        RawImage lampImage = trafficLamp.AddComponent<RawImage>();
        
        RectTransform rt = trafficLamp.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);        
        rt.SetPositionAndRotation(new Vector3(0, posY, 0), Quaternion.identity);

        trafficLamp.transform.SetParent(parent, false);

        InitLampColor(lampImage, color);

        return lampImage;
    }

    private void InitLampColor(RawImage lampImage, Color color)
    {
        Shader lampShader = Shader.Find("TrafficLightShader");
        Material lampMaterial = new Material(lampShader);
        lampMaterial.SetColor("_ActiveColor", color);
        lampImage.material = lampMaterial;
    }

    public bool Decorated
    {
        get { return decorated; }
        set {
            decorated = value;
            SetDecorated(decorated);
        }
    }

    public bool TimerMode
    {
        get { return timerMode; }
        set { timerMode = value; }
    }

    private void SetDecorated(bool isDecor)
    {
        float decor = isDecor ? 1 : 0;
        stopLampImage.material.SetFloat("_IsDecor", decor);
        prepLampImage.material.SetFloat("_IsDecor", decor);
        runLampImage.material.SetFloat("_IsDecor", decor);
    }

    private void DeactivateAll()
    {
        stopLampImage.material.SetFloat("_IsActive", 0);
        prepLampImage.material.SetFloat("_IsActive", 0);
        runLampImage.material.SetFloat("_IsActive", 0);
        
        stopLampImage.material.SetFloat("_IsBlink", 0);
        prepLampImage.material.SetFloat("_IsBlink", 0);
        runLampImage.material.SetFloat("_IsBlink", 0);
    }
    
    public void StopSignal(int period = 0)
    {
        timerPeriod = period;
        UpdateTimerPanel();
        DeactivateAll();        
        stopLampImage.material.SetFloat("_IsActive", 1);

        if (timerMode)
        {
            prepLampImage.material.SetFloat("_IsActive", 1);
            prepLampImage.material.SetColor("_ActiveColor", Color.red);
            prepLampImage.material.SetFloat("_IsTimer", 1);
        }
    }
    public void StopPrepSignal()
    {
        DeactivateAll();
        stopLampImage.material.SetFloat("_IsActive", 1);        
        prepLampImage.material.SetFloat("_IsActive", 1);

        if (timerMode)
        {
            prepLampImage.material.SetColor("_ActiveColor", Color.yellow);
            prepLampImage.material.SetFloat("_IsTimer", 0);
        }
    }

    public void PrepSignal()
    {
        DeactivateAll();
        prepLampImage.material.SetFloat("_IsActive", 1);

        if (timerMode)
        {
            prepLampImage.material.SetColor("_ActiveColor", Color.yellow);
            prepLampImage.material.SetFloat("_IsTimer", 0);
        }
    }

    public void RunSignal(int period = 0)
    {
        timerPeriod = period;
        UpdateTimerPanel();
        DeactivateAll();
        runLampImage.material.SetFloat("_IsActive", 1);
        if (timerMode)
        {
            prepLampImage.material.SetFloat("_IsActive", 1);
            prepLampImage.material.SetColor("_ActiveColor", Color.green);
            prepLampImage.material.SetFloat("_IsTimer", 1);
        }
    }

    public void BlinkSignal(int timePeriod = 0)
    {        
        runLampImage.material.SetFloat("_IsBlink", 1);
    }

    private IEnumerator RunTimerPanel()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);            
            if (timerPeriod > 0)
            {
                timerPeriod--;
                UpdateTimerPanel();                
            }
        }
    }

    private void UpdateTimerPanel()
    {
        stopLampImage.material.SetFloat("_TimerValue", timerPeriod);
        prepLampImage.material.SetFloat("_TimerValue", timerPeriod);
        runLampImage.material.SetFloat("_TimerValue", timerPeriod);
    }
}
