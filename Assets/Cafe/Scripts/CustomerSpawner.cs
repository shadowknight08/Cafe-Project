using UnityEngine;
using System.Collections;

namespace Cafe
{
    public class CustomerSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject customerPrefab;
        public Transform spawnPoint;
        public float spawnInterval = 5f;
        public int maxCustomers = 1;

        [Header("Customer Settings")]
        public float minPatience = 8f;
        public float maxPatience = 15f;
        public int minReward = 5;
        public int maxReward = 20;

        [Header("Events")]
        public System.Action<Customer> OnCustomerSpawned;
        public System.Action<Customer> OnCustomerServed;

        private int currentCustomerCount = 0;
        private Coroutine spawnCoroutine;

        void Start()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            StartSpawning();
        }

        void StartSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
            spawnCoroutine = StartCoroutine(SpawnCustomers());
        }

        IEnumerator SpawnCustomers()
        {
            while (true)
            {
                if (currentCustomerCount < maxCustomers)
                {
                    SpawnCustomer();
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        void SpawnCustomer()
        {
            if (customerPrefab != null)
            {
                GameObject customerObj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
                Customer customer = customerObj.GetComponent<Customer>();

                if (customer != null)
                {
                    // Set random patience and reward
                    customer.patienceTime = Random.Range(minPatience, maxPatience);
                    customer.rewardAmount = Random.Range(minReward, maxReward);

                    // Subscribe to events
                    customer.OnCustomerServed += HandleCustomerServed;
                    customer.OnCustomerLeft += OnCustomerLeft;

                    // Start waiting
                    customer.StartWaiting();

                    currentCustomerCount++;
                    OnCustomerSpawned?.Invoke(customer);
                }
            }
        }

        void HandleCustomerServed(Customer customer)
        {
            OnCustomerServed?.Invoke(customer);
        }

        void OnCustomerLeft(Customer customer)
        {
            currentCustomerCount--;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
