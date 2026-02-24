using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;
using EnhancedTouchSupport = UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour
{
    public delegate void OnSwipeStart(Vector2 position);
    public static event OnSwipeStart onSwipeStart;

    public delegate void OnSwipeEnd(Vector2 startPosition, Vector2 endPosition);
    public static event OnSwipeEnd onSwipeEnd;

    public delegate void OnSwipeUpdate(Vector2 startPosition, Vector2 currentPosition);
    public static event OnSwipeUpdate onSwipeUpdate;

    public static bool disableInputs = true;

    [SerializeField]
    float maxHoldTime = 1.5f;
    

    List<RaycastResult> uiEventList = new List<RaycastResult>();
    PointerEventData pointer;
    Finger currentFinger;

    float holdTimer = 0f;
    bool holding = false;
    bool pressStart = false;
    bool mouseInput = false;
    Vector2 startPosition;

    void Awake()
    {
        pointer = new PointerEventData(EventSystem.current);
    }

    private void Start()
    {
        StartCoroutine(Wait());
    }


    public static void SetDisableInputs(bool value)
    {
        disableInputs = value;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        disableInputs = false;
    }

    void Update()
    {
        if (disableInputs) return;

        #region MouseInput
        if (Input.GetMouseButtonDown(0))
        {
            mouseInput = true;
            StartSwipe(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndSwipe(Input.mousePosition);
            mouseInput = false;
        }
        #endregion

        #region SwipeUpdate
        if (!pressStart || !holding)
            return;

        holdTimer += Time.deltaTime;

        if (holdTimer >= maxHoldTime)
        {
            EndSwipe(GetCurrentPosition());
            return;
        }

        Vector2 current = NormalizeScreenPosition(GetCurrentPosition());
        onSwipeUpdate?.Invoke(startPosition, current);
        #endregion
    }

    public void StartSwipe(Vector2 currentPosition)
    {
        if (disableInputs) return;

        // Prevent gesture if UI pressed
        uiEventList.Clear();
        pointer.position = currentPosition;
        EventSystem.current.RaycastAll(pointer, uiEventList);

        if (uiEventList.Count > 0)
            return;

        startPosition = NormalizeScreenPosition(currentPosition);

        pressStart = true;
        holding = true;
        holdTimer = 0f;

        onSwipeStart?.Invoke(currentPosition);
    }

    public void EndSwipe(Vector2 currentPosition)
    {
        if (disableInputs) return;

        if (!pressStart)
            return;

        // Prevent gesture if UI pressed
        uiEventList.Clear();
        pointer.position = startPosition;
        EventSystem.current.RaycastAll(pointer, uiEventList);

        if (uiEventList.Count > 0)
            return;

        holding = false;
        pressStart = false;
        holdTimer = 0f;

        currentPosition = NormalizeScreenPosition(currentPosition);
        onSwipeEnd?.Invoke(startPosition, currentPosition);
    }

    Vector2 GetCurrentPosition()
    {
        if (mouseInput)
            return Input.mousePosition;

        if (currentFinger != null && currentFinger.currentTouch.valid)
            return currentFinger.currentTouch.screenPosition;

        return startPosition;
    }

    #region TouchInput

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += TouchFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += TouchFingerUp;
    }

    void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= TouchFingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= TouchFingerUp;
        EnhancedTouchSupport.Disable();
    }

    void TouchFingerDown(Finger finger)
    {
        if (finger.index != 0)
            return;

        mouseInput = false;
        currentFinger = finger;

        StartSwipe(finger.screenPosition);
    }

    void TouchFingerUp(Finger finger)
    {
        if (finger.index != 0)
            return;

        EndSwipe(finger.screenPosition);
        currentFinger = null;
    }
    #endregion

    Vector2 NormalizeScreenPosition(Vector2 screenPos)
    {
        return new Vector2(
            screenPos.x / Screen.width,
            screenPos.y / Screen.height
        );
    }
}