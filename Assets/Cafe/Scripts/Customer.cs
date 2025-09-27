using UnityEngine;
using System.Collections;

namespace Cafe
{
    public class Customer : MonoBehaviour, IInteractable
    {
        [Header("Customer Settings")]
        public float patienceTime = 10f;
        public int rewardAmount = 10;
        public Transform servingCounter;

        [Header("Visual Settings")]
        public GameObject customerModel;
        public GameObject patienceBar;
        public Transform patienceBarFill;


        [Header("Events")]
        public System.Action<Customer> OnCustomerServed;
        public System.Action<Customer> OnCustomerLeft;

        private bool isWaiting = false;
        private bool isServed = false;
        private float currentPatience;
        private Vector3 startPosition;
        private Coroutine patienceCoroutine;

        void Start()
        {
            startPosition = transform.position;
            currentPatience = patienceTime;

            if (patienceBar != null)
            {
                patienceBar.SetActive(false);
            }
        }

        void Update()
        {
            if (isWaiting && !isServed)
            {
                // Update patience bar
                UpdatePatienceBar();
            }
        }

        public void StartWaiting()
        {
            isWaiting = true;
            currentPatience = patienceTime;

            if (patienceBar != null)
            {
                patienceBar.SetActive(true);
            }

            if (patienceCoroutine != null)
            {
                StopCoroutine(patienceCoroutine);
            }
            patienceCoroutine = StartCoroutine(PatienceCountdown());
        }

        IEnumerator PatienceCountdown()
        {
            while (currentPatience > 0 && !isServed)
            {
                currentPatience -= Time.deltaTime;
                yield return null;
            }

            if (!isServed)
            {
                // Customer left due to impatience
                LeaveCustomer();
            }
        }

        public void Interact(PlayerController player)
        {
            if (isServed || !isWaiting)
            {
                return;
            }

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasCoffee())
            {
                ServeCustomer(player);
            }
        }

        public string GetInteractionText()
        {
            if (isServed)
            {
                return "Customer served!";
            }
            else if (isWaiting)
            {
                return "Serving coffee...";
            }
            else
            {
                return "Customer is not ready";
            }
        }

        public bool CanInteract(PlayerController player)
        {
            if (isServed || !isWaiting)
            {
                return false;
            }

            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            return inventory != null && inventory.HasCoffee();
        }

        void ServeCustomer(PlayerController player)
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.RemoveCoffee())
            {
                isServed = true;
                isWaiting = false;

                if (patienceCoroutine != null)
                {
                    StopCoroutine(patienceCoroutine);
                }

                if (patienceBar != null)
                {
                    patienceBar.SetActive(false);
                }

                // Give reward
                GameManager.Instance?.AddScore(rewardAmount, transform.position);

                // Trigger events
                OnCustomerServed?.Invoke(this);

                // Start leave animation
                StartCoroutine(LeaveAnimation());
            }
        }

        IEnumerator LeaveAnimation()
        {
            // Simple leave animation
            yield return new WaitForSeconds(0.5f);
            LeaveCustomer();
        }

        void LeaveCustomer()
        {
            OnCustomerLeft?.Invoke(this);
            Destroy(gameObject);
        }

        void UpdatePatienceBar()
        {
            if (patienceBarFill != null)
            {
                float patiencePercent = currentPatience / patienceTime;
                Vector3 scale = patienceBarFill.localScale;
                scale.x = patiencePercent;
                patienceBarFill.localScale = scale;

                // Change color based on patience
                Renderer fillRenderer = patienceBarFill.GetComponent<Renderer>();
                if (fillRenderer != null)
                {
                    Color color = Color.Lerp(Color.red, Color.green, patiencePercent);
                    fillRenderer.material.color = color;
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = isWaiting ? Color.yellow : Color.gray;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
