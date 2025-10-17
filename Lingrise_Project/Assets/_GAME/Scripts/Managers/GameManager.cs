using System;
using System.Collections;
using UnityEngine;

namespace AgeOfWar
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CameraController cameraController;

        [SerializeField] private GameState gameState;
        public GameState GameState => gameState;

        private void Setup()
        {
            gameState = GameState.Setup;
            gridManager.OnInit();
            cameraController.OnInit();
        }

        private void Start()
        {
            Setup();
            gameState = GameState.Playing;
        }

        private void Update()
        {
            if (gridManager.OnUpdateGameplay()) return;
        }
    }
}

