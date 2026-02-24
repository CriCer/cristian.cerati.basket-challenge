using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public BallHandler ball;
    public UIManager uiManager;

    [SerializeField]
    float maxPower = 10f;
    [SerializeField]
    float minPower = 4f;
    [SerializeField]
    float error = 0.05f;
    [SerializeField]
    float maxSwipeDistance = 4f;
    [SerializeField]
    float inputMultiplier = 500f;
    


    public void NewBallPosition(Vector3 newPosition)
    {
        if (!ball) return;

        ball.NewPosition(newPosition);

        if (!uiManager) return;

        float t = Mathf.InverseLerp(minPower, maxPower, ball.GetPerfectShotRequiredPower());
        uiManager.UpdatePerfectZone(t, error);
        uiManager.UpdateBackboardZone(0.8f, error);
    }

    protected virtual void OnEnable()
    {
        InputManager.onSwipeUpdate += SwipeUpdate;
        InputManager.onSwipeEnd += SwipeEnd;
    }

    protected virtual void OnDisable()
    {
        InputManager.onSwipeUpdate -= SwipeUpdate;
        InputManager.onSwipeEnd -= SwipeEnd;
    }

    private void SwipeUpdate(Vector2 startPosition, Vector2 endPosition)
    {

        float distance = (endPosition.y - startPosition.y) * inputMultiplier;
        distance = Mathf.Clamp(distance, 0.0f, maxSwipeDistance);
        float t = Mathf.InverseLerp(0f, maxSwipeDistance, distance);

        //Update power slider
        if (uiManager == null) return;
        uiManager.UpdatePowerSlider(t);
    }

    private void SwipeEnd(Vector2 startPosition, Vector2 endPosition)
    {
        
        float distance = (endPosition.y - startPosition.y) * inputMultiplier;
        distance = Mathf.Clamp(distance, 0f, maxSwipeDistance);
        float t = Mathf.InverseLerp(0f, maxSwipeDistance, distance);

        float perfectT = Mathf.InverseLerp(minPower, maxPower, ball.GetPerfectShotRequiredPower());
        float remappedValue = Utilities.RemapValue(t, perfectT, 0.8f, error);

        ball.Shoot(Mathf.Lerp(minPower, maxPower, remappedValue));
    }

    

    
}
