using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AgeOfWar
{
    public class PlatformManager : MonoBehaviour, IManager
    {
        public static PlatformManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OnInit()
        {

        }

        public bool OnUpdateGameplay()
        {
            return true;
        }

        public void SetupCamera(Vector3 position, float scale)
        {
            transform.position = position;
            transform.localScale = Vector3.one * scale;
        }
    }
}

