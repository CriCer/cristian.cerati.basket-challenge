using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utilities : MonoBehaviour
{
    public static float RemapValue(float raw, float perfectValue, float backboardValue, float error)
    {
        float p0 = perfectValue - error / 2f;
        float p1 = perfectValue + error / 2f;
        float b0 = backboardValue - error / 2f;
        float b1 = backboardValue + error / 2f;

        if (raw >= p0 && raw <= p1)
            return perfectValue;

        if (raw >= b0 && raw <= b1)
            return backboardValue;

        if (raw < p0)
            return Mathf.InverseLerp(0f, p0, raw) * perfectValue;

        if (raw > p1 && raw < b0)
            return perfectValue + Mathf.InverseLerp(p1, b0, raw) * (backboardValue - perfectValue);

        return backboardValue + Mathf.InverseLerp(b1, 1f, raw) * (1f - backboardValue);
    }

}
