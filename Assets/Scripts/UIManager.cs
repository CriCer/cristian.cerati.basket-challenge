using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text infoText;
    [SerializeField]
    GameObject resultPanel;
    [SerializeField]
    GameObject endPanel;
    [SerializeField]
    UnityEngine.UI.Slider powerSlider;
    [SerializeField]
    UnityEngine.UI.Slider timeRemainingSlider;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    TMP_Text finalScoreText;

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
        if (!powerSlider) return;
        
        powerSlider.value = value;
       
    }

    public void UpdatePerfectZone(float value, float error)
    {
        if (!perfectZoneImage) return;

        UpdateZone(perfectZoneImage, value, error);
    }
    
    public void UpdateBackboardZone(float value, float error)
    {
        if (!backboardZoneImage) return;

        UpdateZone(backboardZoneImage, value, error);

    }

    public void UpdateTimeSlider(float value)
    {
        if (!timeRemainingSlider) return;
        timeRemainingSlider.value = value;
    }

    public void UpdateInfoText(string text)
    {
        if (!infoText) return;
        infoText.text = text;
    }
    
    public void UpdateScoreText(int score)
    {
        if (!scoreText) return;
        scoreText.text = score.ToString();
    }

    public void ShowResultScreen(int score)
    {
        if (!resultPanel) return;
        resultPanel.SetActive(true);
        if (!finalScoreText) return;
        finalScoreText.text = "Final score: " + score.ToString();
    }

    public void ShowEndScreen()
    {
        if (!endPanel) return;
        endPanel.SetActive(true);

        if (!resultPanel) return;
        resultPanel.SetActive(false);
    }

    public void DisplayInfoText(string text)
    {
        if (!infoText) return;
        infoText.text = text;
        StartCoroutine(ClearInfoTextAfterOneSecond());
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

    IEnumerator ClearInfoTextAfterOneSecond()
    {
        yield return new WaitForSeconds(1f);
        if (!infoText) yield break;
        infoText.text = "";
    }

}
