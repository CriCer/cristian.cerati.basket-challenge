using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;
using EnhancedTouchSupport = UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport;

public class InputManager : MonoBehaviour
{
    private List<RaycastResult> uiEventList = new List<RaycastResult>();

    [SerializeField]
    private List<GestureBase> Gestures;

    private PointerEventData pointer = new PointerEventData(EventSystem.current);

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        EnhancedTouch.onFingerDown += TouchFingerDown;
        EnhancedTouch.onFingerUp += TouchFingerUp;

    }
    private void OnDisable()
    {
        EnhancedTouch.onFingerDown -= TouchFingerDown;
        EnhancedTouch.onFingerUp -= TouchFingerUp;
        EnhancedTouchSupport.Disable();
    }

    protected virtual void TouchFingerDown(Finger finger)
    {

        if (finger.index != 0)
            return;

        //Prevent gesture if ui element is pressed
        pointer.position = finger.currentTouch.startScreenPosition;
        EventSystem.current.RaycastAll(pointer, uiEventList);

        if (uiEventList.Count > 0)
            return;

        foreach (GestureBase gesture in Gestures)
            gesture.StartTouch();

    }

    protected virtual void TouchFingerUp(Finger finger)
    {
        if (finger.index != 0)
            return;

        //Prevent gesture if ui element is pressed
        pointer.position = finger.currentTouch.startScreenPosition;
        EventSystem.current.RaycastAll(pointer, uiEventList);

        if (uiEventList.Count > 0)
            return;


        foreach (GestureBase gesture in Gestures)
            gesture.EndTouch();

    }


}