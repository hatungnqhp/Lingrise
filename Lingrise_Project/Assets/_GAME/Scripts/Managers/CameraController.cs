using System;
using System.Collections;
using System.Collections.Generic;
using AgeOfWar;
using UnityEngine;

namespace AgeOfWar
{
    public class CameraController : MonoBehaviour, IManager
    {
        [SerializeField] private Camera mainCamera;

        [Header("Ratio to grid")]
        [SerializeField] private float screenWidthRatio = 2f;
        [SerializeField] private float screenHeighRatio = 3.2f;

        [Header("Ratio to screen heigh")]
        [SerializeField] private float cameraPivotRatio = 0.25f;
        [SerializeField] private float platformPivotRatio = 0.35f;

        [Header("Zoom")]
        [SerializeField] private float minZoom = 0.8f;
        [SerializeField] private float maxZoom = 1.2f;

        public void OnInit()
        {
            SetupCamera();
        }

        public bool OnUpdateGameplay()
        {
            return true;
        }

        private void SetupCamera()
        {
            float gridSize = Mathf.Max(GridManager.Instance.Width, GridManager.Instance.Height);
            float screenWidth = gridSize * screenWidthRatio;
            float screenHeigh = gridSize * screenHeighRatio;

            mainCamera.transform.position = new Vector3(
                gridSize / 2,
                cameraPivotRatio * screenHeigh,
                -10
            );
            PlatformManager.Instance.transform.position = new Vector3(
                PlatformManager.Instance.transform.position.x, //
                platformPivotRatio * screenHeigh,
                PlatformManager.Instance.transform.position.z
            );

            float screenAspect = (float)Screen.height / Screen.width;
            float targetAspect = screenHeigh / screenWidth;
            float zoom = Mathf.Clamp(targetAspect / screenAspect, minZoom, maxZoom);

            //mainCamera.orthographicSize = screenWidth * screenAspect / 2f;
            mainCamera.orthographicSize = screenWidth * screenAspect / 2f * zoom;

            Debug.Log($"Setup camera: screenAspect {screenAspect}, " +
                    "targetAspect {targetAspect}, zoom {zoom}", this);
        }
    }
}