using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cafe
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI coinText;
        public Canvas canvas;

        [Header("Coin Animation")]
        public Image coinImagePrefab; // UI Image prefab for the animation



        private int currentCoins = 0;

        void Start()
        {
            // Subscribe to game events
            GameManager.Instance.OnScoreChanged += UpdateCoins;

            // Initialize UI
            UpdateCoins(0);
        }

        void Update()
        {

        }



        public void UpdateCoins(int newCoins)
        {
            currentCoins = newCoins;
            if (coinText != null)
            {
                coinText.text = "Coins: " + currentCoins.ToString();
            }
        }

        public void AddCoins(int amount)
        {
            UpdateCoins(currentCoins + amount);
        }

        public void AnimateCoinReward(int amount, Vector3 worldPosition)
        {
            if (coinImagePrefab != null && coinText != null && canvas != null)
            {
                StartCoroutine(CoinAnimation(amount, worldPosition));
            }
            else
            {
                // Fallback: just update coins immediately
                AddCoins(amount);
            }
        }

        System.Collections.IEnumerator CoinAnimation(int amount, Vector3 worldPosition)
        {
            // Convert world position to screen position
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            Vector2 screenPos2D = new Vector2(screenPosition.x, screenPosition.y);

            // Create UI coin image
            Image coinImage = Instantiate(coinImagePrefab, canvas.transform);
            RectTransform coinRect = coinImage.rectTransform;

            // Set initial position (convert screen position to canvas position)
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos2D,
                canvas.worldCamera,
                out canvasPosition
            );
            coinRect.anchoredPosition = canvasPosition;

            // Get target position (coin text position in canvas coordinates)
            Vector3 coinTextScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, coinText.transform.position);
            Vector2 coinTextScreenPos2D = new Vector2(coinTextScreenPos.x, coinTextScreenPos.y);
            Vector2 targetPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                coinTextScreenPos2D,
                canvas.worldCamera,
                out targetPosition
            );

            // Animate the single coin image
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Smooth curve animation
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                // Linear movement with slight arc
                Vector2 currentPos = Vector2.Lerp(canvasPosition, targetPosition, smoothT);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * 30f; // Small arc height in UI units

                coinRect.anchoredPosition = currentPos;

                // Fade out as it approaches target
                float alpha = Mathf.Lerp(1f, 0f, t * 1.5f); // Start fading before reaching target
                Color color = coinImage.color;
                color.a = alpha;
                coinImage.color = color;

                yield return null;
            }

            // Clean up
            Destroy(coinImage.gameObject);

            // Update coins with the full amount
            AddCoins(amount);
        }

    }
}
