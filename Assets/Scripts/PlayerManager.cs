using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public BallHandler ball;
    public UIManager uiManager;
    public float maxPower = 10f;
    public float minPower = 4f;
    public float error = 0.05f;

    [SerializeField]
    float maxSwipeDistance = 4f;

    void Start()
    {
        if (uiManager != null && ball != null)
        {
            float t = Mathf.InverseLerp(minPower, maxPower, ball.GetPerfectShotRequiredPower());
            Debug.Log(t);
            uiManager.UpdatePerfectZone(t, error);
            uiManager.UpdateBackboardZone(0.8f, error);

        }
        
    }

    //void Update()
    //{
        
    //}

    protected virtual void OnEnable()
    {
        //GestureSwipe.onSwipeStart += SwipeStart;
        GestureSwipe.onSwipeUpdate += SwipeUpdate;
        GestureSwipe.onSwipeEnd += SwipeEnd;
    }

    protected virtual void OnDisable()
    {
        //GestureSwipe.onSwipeStart -= SwipeStart;
        GestureSwipe.onSwipeUpdate -= SwipeUpdate;
        GestureSwipe.onSwipeEnd -= SwipeEnd;
    }

    //private void SwipeStart(Vector2 position)
    //{
        
    //}

    private void SwipeUpdate(Vector2 startPosition, Vector2 endPosition)
    {
        float distance = endPosition.y - startPosition.y;
        distance = Mathf.Clamp(distance, 0.0f, maxSwipeDistance);
        float t = Mathf.InverseLerp(0f, maxSwipeDistance, distance);

        //Update power slider
        if (uiManager == null) return;
        uiManager.UpdatePowerSlider(t);
    }

    private void SwipeEnd(Vector2 startPosition, Vector2 endPosition)
    {
        float distance = endPosition.y - startPosition.y;
        distance = Mathf.Clamp(distance, 0f, maxSwipeDistance);
        float t = Mathf.InverseLerp(0f, maxSwipeDistance, distance);

        float perfectT = Mathf.InverseLerp(minPower, maxPower, ball.GetPerfectShotRequiredPower());
        float remappedValue = Utilities.RemapValue(t, perfectT, 0.8f, error);

        ball.Shoot(Mathf.Lerp(minPower, maxPower, remappedValue));
        Debug.Log("raw: " + t);
        Debug.Log("remapped: " + remappedValue);
    }

    
}
