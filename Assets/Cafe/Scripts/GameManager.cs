using UnityEngine;
using System.Collections;

namespace Cafe
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public int startingScore = 0;
        public float gameTime = 300f; // 5 minutes

        [Header("Events")]
        public System.Action<int> OnScoreChanged;
        public System.Action<float> OnTimeChanged;
        public System.Action OnGameOver;

        public static GameManager Instance { get; private set; }

        private int currentScore;
        private float currentTime;
        private bool gameActive = true;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            currentScore = startingScore;
            currentTime = gameTime;

            OnScoreChanged?.Invoke(currentScore);
            OnTimeChanged?.Invoke(currentTime);
        }

        void Update()
        {
            if (gameActive)
            {
                currentTime -= Time.deltaTime;
                OnTimeChanged?.Invoke(currentTime);

                if (currentTime <= 0)
                {
                    EndGame();
                }
            }
        }

        public void AddScore(int amount, Vector3 worldPosition = default)
        {
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);

            // Animate coin reward if UI is available
            GameUI ui = FindObjectOfType<GameUI>();
            if (ui != null)
            {
                ui.AnimateCoinReward(amount, worldPosition);
            }
        }

        public void EndGame()
        {
            gameActive = false;
            OnGameOver?.Invoke();

            Debug.Log("Game Over! Final Score: " + currentScore);
        }

        public void RestartGame()
        {
            currentScore = startingScore;
            currentTime = gameTime;
            gameActive = true;

            OnScoreChanged?.Invoke(currentScore);
            OnTimeChanged?.Invoke(currentTime);
        }

        public int GetScore()
        {
            return currentScore;
        }

        public float GetTimeRemaining()
        {
            return currentTime;
        }

        public bool IsGameActive()
        {
            return gameActive;
        }
    }
}
