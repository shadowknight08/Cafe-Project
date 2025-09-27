using UnityEngine;

namespace Cafe
{
    public class CoffeeBeanResource : MonoBehaviour, IInteractable
    {
        [Header("Visual Settings")]
        public GameObject beanPrefab;
        public Transform spawnPoint;
        public float spawnHeight = 1f;


        private GameObject currentBean;
        private Vector3 startPosition;

        void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            startPosition = spawnPoint.position;
            SpawnBean();
        }

        void Update()
        {

        }

        public void Interact(PlayerController player)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.CanCollectBean())
            {
                if (inventory.AddBean())
                {
                    // Bean collected successfully
                    SpawnBean(); // Spawn a new bean
                }
            }
        }

        public string GetInteractionText()
        {
            return "Collecting coffee beans...";
        }

        public bool CanInteract(PlayerController player)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            return inventory != null && inventory.CanCollectBean();
        }

        void SpawnBean()
        {
            if (beanPrefab != null)
            {
                currentBean = Instantiate(beanPrefab, startPosition, Quaternion.identity);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * spawnHeight, 0.3f);
        }
    }
}
