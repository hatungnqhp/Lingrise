using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static Vector3 GetMouseWorldPos()
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePos = Input.mousePosition;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        return worldPos;
    }
}
