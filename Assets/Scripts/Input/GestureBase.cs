using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;
using EnhancedTouchSupport = UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport;


public class GestureBase : MonoBehaviour
{
    protected Finger currentFinger;
       
    public virtual void StartTouch()
    {
        currentFinger = EnhancedTouch.activeTouches[0].finger;
            
    }
    public virtual void EndTouch()
    {

    }
        
}
