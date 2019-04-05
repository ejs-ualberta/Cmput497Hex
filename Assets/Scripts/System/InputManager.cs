using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{    

    public static Vector3 mousePosition
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return (Input.touchCount > 0) ? Input.mousePosition : Vector3.negativeInfinity;
#else
            return Input.mousePosition;
#endif
        }
    }

    public static bool GetMouseButtonDown(int button)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Input.GetMouseButtonUp(button);
#else
        return Input.GetMouseButtonDown(button);
#endif

    }

}
