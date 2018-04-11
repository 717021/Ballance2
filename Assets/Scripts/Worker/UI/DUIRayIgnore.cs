using UnityEngine;
using System.Collections;

/*
 * UI 穿透脚本 
 */

public class DUIRayIgnore : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return false;
    }
}
