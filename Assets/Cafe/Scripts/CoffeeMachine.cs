using UnityEngine;
using System.Collections;

namespace Cafe
{
    public class CoffeeMachine : MonoBehaviour, IInteractable
    {
        [Header("Machine Settings")]
        public float processingTime = 3f;
        public Transform coffeeSpawnPoint;
        public GameObject coffeeCupPrefab;


        [Header("Machine State")]
        public bool isProcessing = false;
        public bool hasCoffee = false;

        private GameObject currentCoffeeCup;
        private Coroutine processingCoroutine;

        void Start()
        {

        }

        public void Interact(PlayerController player)
        {
            if (isProcessing)
            {
                return; // Machine is busy
            }

            if (hasCoffee)
            {
                // Player is collecting coffee
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null && inventory.CanAddCoffee())
                {
                    CollectCoffee(player);
                }
            }
            else
            {
                PlayerInventory inventory = player.GetComponent<PlayerInventory>();
                if (inventory != null && inventory.HasBeans())
                {
                    // Player is adding beans to machine
                    StartProcessing(player);
                }
            }
        }

        public string GetInteractionText()
        {
            if (isProcessing)
            {
                return "Processing coffee...";
            }
            else if (hasCoffee)
            {
                return "Collecting coffee...";
            }
            else
            {
                return "Adding coffee beans...";
            }
        }

        public bool CanInteract(PlayerController player)
        {
            if (isProcessing)
            {
                return false;
            }

            if (hasCoffee)
            {
                return true; // Can always collect coffee
            }

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            return inventory != null && inventory.HasBeans(); // Can add beans if player has them
        }

        void StartProcessing(PlayerController player)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.RemoveBean())
            {
                isProcessing = true;
                hasCoffee = false;

                // Processing started

                // Start processing coroutine
                if (processingCoroutine != null)
                {
                    StopCoroutine(processingCoroutine);
                }
                processingCoroutine = StartCoroutine(ProcessCoffee());
            }
        }

        IEnumerator ProcessCoffee()
        {
            yield return new WaitForSeconds(processingTime);

            // Processing complete
            isProcessing = false;
            hasCoffee = true;

            // Processing complete

            // Spawn coffee cup
            SpawnCoffeeCup();
        }

        void SpawnCoffeeCup()
        {
            if (coffeeCupPrefab != null)
            {
                Vector3 spawnPos = coffeeSpawnPoint != null ? coffeeSpawnPoint.position : transform.position + Vector3.up;
                currentCoffeeCup = Instantiate(coffeeCupPrefab, spawnPos, Quaternion.identity);
            }
        }

        void CollectCoffee(PlayerController player)
        {
            if (currentCoffeeCup != null)
            {
                Destroy(currentCoffeeCup);
                currentCoffeeCup = null;
            }

            hasCoffee = false;

            // Add coffee to player
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            inventory?.AddCoffee();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one);

            if (coffeeSpawnPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(coffeeSpawnPoint.position, 0.2f);
            }
        }
    }
}
