using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using DuetSystem.Datas;
using DuetSystem.Utilities;

namespace DuetSystem.Macros
{

    /// <summary>
    /// Manages the overall status of the duet system hierarchial finite state machine (HSFM)
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {

        #region Configuration Parameters

        /// <summary>
        /// Stores the level manager for the duet system
        /// </summary>
        public GameObject levelManager;

        /// <summary>
        /// Stores the spawn manager for the duet system
        /// </summary>
        public GameObject spawnManager;

        // level blueprint

        /// <summary>
        /// A listener that gets triggered when the game state is updated
        /// </summary>
        public DuetGameEvents.DuetGameStateEvent WhenGameStateChanges;

        #endregion

        #region Game Manager Properties

        public DuetGameState CurrentGameState
        {
            get => currentGameState;
            private set => currentGameState = value;
        }

        #endregion

        #region Cached References

        /// <summary>
        /// Used for the duet system level loading/unloading operations
        /// </summary>
        private DuetGameState previousGameState;

        /// <summary>
        /// Holds the current status of the game
        /// </summary>
        private DuetGameState currentGameState = DuetGameState.ONBOARDING;

        /// <summary>
        /// Stores the instance of the system manager prefabs. Used for level unloading purposes.
        /// </summary>
        private List<GameObject> systemManagersPrefabInstance;

        #endregion

        #region Level Loading/Unloading Operations

        public void LoadLevel(string levelBlueprintName)
        {
            // Check if the Level Manager is found in the systemPrefabsInstance list
            // Call the LoadLevel method from the Level Manager
        }

        private void OnLoadLevelOperationComplete()
        {
            // Show the Duet System Commons after the level boot sequence ends

            Debug.Log("[Duet Game Macro] Load Complete.");
        }

        public void UnloadLevel(string levelName)
        {
            // Call the UnloadLevel method from the Level Manager
        }

        private void OnUnloadLevelOperationComplete()
        {
            Debug.Log("[Duet Game Macro] Unload Complete.");
            // Destroy the Duet gameobject
        }

        #endregion

        #region Initialization

        private void Start()
        {
            systemManagersPrefabInstance = new List<GameObject>();
            InstantiateSystemManagers();
            StartGame();
        }

        private void InstantiateSystemManagers()
        {
            // Instantiate system prefabs
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < systemManagersPrefabInstance.Count; ++i)
            {
                Destroy(systemManagersPrefabInstance[i]);
            }

            systemManagersPrefabInstance.Clear();
        }

        #endregion

        #region State Machine Management

        private void StartGame()
        {
            if (currentGameState != DuetGameState.ONBOARDING)
            {
                return;
            }
        }

        private void UpdateGameState(DuetGameState duetGameState)
        {
            previousGameState = currentGameState;
            currentGameState = duetGameState;

            switch (currentGameState)
            {
                case DuetGameState.ONBOARDING:

                    break;

                case DuetGameState.RUNNING:

                    break;

                case DuetGameState.TERMINATING:

                    break;

                default:

                    break;
            }

            // Check whether the Duet Game State Events is not equal to null, and if it's not, then invoke event
            WhenGameStateChanges?.Invoke(currentGameState, previousGameState);
        }

        public void PauseGame()
        {
            
        }

        public void ResumeGame()
        {
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion
    }
}
