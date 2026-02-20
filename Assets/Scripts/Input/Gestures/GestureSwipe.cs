using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureSwipe : GestureBase
{
    public delegate void OnSwipeStart(Vector2 position);
    public static event OnSwipeStart onSwipeStart;
    public delegate void OnSwipeEnd(Vector2 startPosition, Vector2 endPosition);
    public static event OnSwipeEnd onSwipeEnd;

    public delegate void OnSwipeUpdate(Vector2 startPosition, Vector2 currentPosition);
    public static event OnSwipeUpdate onSwipeUpdate;

    [SerializeField]
    private float maxHoldTime = 1.5f;

    private float holdTimer = 0f;
    private bool holding = false;
    private bool pressStart = false;

    public override void StartTouch()
    {
        base.StartTouch();
        pressStart = true;
        holding = true;
        holdTimer = 0f;
        onSwipeStart?.Invoke(currentFinger.currentTouch.screenPosition);
    }
    public override void EndTouch()
    {
        if (!pressStart)
            return;

        holding = false;
        pressStart = false;
        holdTimer = 0f;


        onSwipeEnd?.Invoke(currentFinger.currentTouch.startScreenPosition, currentFinger.currentTouch.screenPosition);
        //Debug.Log("release");
        //Debug.Log(currentFinger.currentTouch.startScreenPosition.y);
        //Debug.Log(currentFinger.currentTouch.screenPosition.y);

    }
    private void Update()
    {
        if (!pressStart || !holding)
            return;

        holdTimer += Time.deltaTime;

        if (holdTimer >= maxHoldTime)
        {
            EndTouch();
            return;
        }

        onSwipeUpdate?.Invoke(currentFinger.currentTouch.startScreenPosition, currentFinger.currentTouch.screenPosition);
    }
}
