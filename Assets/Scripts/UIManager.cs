using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text startText;
    [SerializeField]
    public GameObject rewardPanel;
    [SerializeField]
    public GameObject endPanel;
    [SerializeField]
    UnityEngine.UI.Slider powerSlider;
    [SerializeField]
    UnityEngine.UI.Slider timeRemainingSlider;
    [SerializeField]
    TMP_Text scoreText;

    [SerializeField]
    private RectTransform perfectZoneImage;
    [SerializeField]
    private RectTransform backboardZoneImage;

    [SerializeField]
    RectTransform powerSliderRect;

    private void Awake()
    {

    }

    private void Start()
    {
    }

    public void UpdatePowerSlider(float value)
    {
        if (powerSlider == null) return;
        
        powerSlider.value = value;
       
    }

    public void UpdatePerfectZone(float value, float error)
    {
        if (perfectZoneImage == null) return;

        UpdateZone(perfectZoneImage, value, error);
    }
    
    public void UpdateBackboardZone(float value, float error)
    {
        if (perfectZoneImage == null) return;

        UpdateZone(backboardZoneImage, value, error);

    }

    void UpdateZone(RectTransform zone, float value, float error)
    {
        float halfError = error / 2f;
        float zoneMin = Mathf.Clamp01(value - halfError);
        float zoneMax = Mathf.Clamp01(value + halfError);

        zone.anchorMin = new Vector2(zoneMin, 0);
        zone.anchorMax = new Vector2(zoneMax, 1);
        zone.offsetMin = Vector2.zero;
        zone.offsetMax = Vector2.zero;
    }

    
}
