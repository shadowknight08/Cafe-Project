using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cafe
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float interactionRange = 2f;
        public Camera playerCamera;


        [Header("Visual References")]
        public Transform playerBack;
        public LayerMask interactableLayer = 1;

        [Header("Interaction Settings")]
        public float interactionCooldown = 0.5f;

        private Rigidbody rb;
        private Vector3 movement;
        private GameObject currentInteractable;
        private float lastInteractionTime;
        private PlayerInventory playerInventory;

        // Events
        public System.Action<GameObject> OnInteractableChanged;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.freezeRotation = true;
            }

            // Get or add PlayerInventory component
            playerInventory = GetComponent<PlayerInventory>();
            if (playerInventory == null)
            {
                playerInventory = gameObject.AddComponent<PlayerInventory>();
            }

            // Find camera if not assigned
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
                if (playerCamera == null)
                {
                    playerCamera = FindObjectOfType<Camera>();
                }
            }
        }

        void Update()
        {
            HandleMovement();
            HandleInteraction();
        }

        void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Check if there's actual input (not just noise)
            bool hasInput = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;

            if (hasInput && playerCamera != null)
            {
                // Get camera's forward and right vectors (flattened to ground plane)
                Vector3 cameraForward = playerCamera.transform.forward;
                Vector3 cameraRight = playerCamera.transform.right;

                // Remove Y component to keep movement on ground plane
                cameraForward.y = 0f;
                cameraRight.y = 0f;

                // Normalize to ensure consistent movement speed
                cameraForward.Normalize();
                cameraRight.Normalize();

                // Calculate movement direction relative to camera
                movement = (cameraRight * horizontal + cameraForward * vertical).normalized;

                // Smooth rotation towards movement direction
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
            else
            {
                movement = Vector3.zero;
            }
        }

        void FixedUpdate()
        {
            if (rb != null)
            {
                rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
            }
        }

        void HandleInteraction()
        {
            // Find nearest interactable
            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
            GameObject nearestInteractable = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider col in colliders)
            {
                if (col.GetComponent<IInteractable>() != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestInteractable = col.gameObject;
                    }
                }
            }

            // Update current interactable
            if (nearestInteractable != currentInteractable)
            {
                currentInteractable = nearestInteractable;
                OnInteractableChanged?.Invoke(currentInteractable);
            }

            // Auto-interact when in range (with cooldown)
            if (currentInteractable != null && Time.time - lastInteractionTime >= interactionCooldown)
            {
                IInteractable interactable = currentInteractable.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(this))
                {
                    interactable.Interact(this);
                    lastInteractionTime = Time.time;
                }
            }
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
