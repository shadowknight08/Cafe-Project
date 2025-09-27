using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cafe
{
    public class CafeManager : MonoBehaviour
    {
        [Header("Player Setup")]
        public GameObject playerPrefab;
        public Transform playerSpawnPoint;

        void Start()
        {
            SetupGame();
        }

        void SetupGame()
        {
            // Spawn player
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                SetupPlayer(player);
            }
        }

        void SetupPlayer(GameObject player)
        {
            // Player will use its own prefab references
            // Add PlayerInventory component if not present
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                inventory = player.AddComponent<PlayerInventory>();
            }
        }
    }
}
