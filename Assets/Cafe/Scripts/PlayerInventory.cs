using UnityEngine;
using System.Collections.Generic;

namespace Cafe
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Bean Stacking")]
        public Transform beanStackParent;
        public GameObject beanPrefab;
        public int maxBeanStack = 3;

        [Header("Coffee Stacking")]
        public Transform coffeeStackParent;
        public GameObject coffeeCupPrefab;
        public int maxCoffeeStack = 1;

        [Header("Events")]
        public System.Action<int> OnBeanStackChanged;
        public System.Action<int> OnCoffeeStackChanged;

        private List<GameObject> beanStack = new List<GameObject>();
        private List<GameObject> coffeeStack = new List<GameObject>();

        void Start()
        {
            // Create bean stack parent if not assigned
            if (beanStackParent == null)
            {
                GameObject stackParent = new GameObject("BeanStack");
                stackParent.transform.SetParent(transform);
                stackParent.transform.localPosition = Vector3.up * 0.5f;
                beanStackParent = stackParent.transform;

            }

            // Create coffee stack parent if not assigned
            if (coffeeStackParent == null)
            {
                GameObject stackParent = new GameObject("CoffeeStack");
                stackParent.transform.SetParent(transform);
                stackParent.transform.localPosition = Vector3.up * 0.8f;
                coffeeStackParent = stackParent.transform;
            }
        }

        void Update()
        {
            UpdateBeanStackVisual();
            UpdateCoffeeStackVisual();
        }

        // Bean management
        public bool CanCollectBean()
        {
            return beanStack.Count < maxBeanStack;
        }

        public bool AddBean()
        {
            if (CanCollectBean())
            {
                GameObject newBean = Instantiate(beanPrefab, beanStackParent);
                newBean.transform.localPosition = Vector3.up * (beanStack.Count * 0.3f);
                beanStack.Add(newBean);
                OnBeanStackChanged?.Invoke(beanStack.Count);
                return true;
            }
            return false;
        }

        public bool RemoveBean()
        {
            if (beanStack.Count > 0)
            {
                GameObject beanToRemove = beanStack[beanStack.Count - 1];
                beanStack.RemoveAt(beanStack.Count - 1);
                Destroy(beanToRemove);
                OnBeanStackChanged?.Invoke(beanStack.Count);
                return true;
            }
            return false;
        }

        public bool HasBeans()
        {
            return beanStack.Count > 0;
        }

        public int GetBeanCount()
        {
            return beanStack.Count;
        }

        // Coffee management
        public bool CanAddCoffee()
        {
            return coffeeStack.Count < maxCoffeeStack;
        }

        public bool AddCoffee()
        {
            if (CanAddCoffee())
            {
                GameObject newCoffee = Instantiate(coffeeCupPrefab, coffeeStackParent);
                newCoffee.transform.localPosition = Vector3.up * (coffeeStack.Count * 0.3f);
                coffeeStack.Add(newCoffee);
                OnCoffeeStackChanged?.Invoke(coffeeStack.Count);
                return true;
            }
            return false;
        }

        public bool RemoveCoffee()
        {
            if (coffeeStack.Count > 0)
            {
                GameObject coffeeToRemove = coffeeStack[coffeeStack.Count - 1];
                coffeeStack.RemoveAt(coffeeStack.Count - 1);
                Destroy(coffeeToRemove);
                OnCoffeeStackChanged?.Invoke(coffeeStack.Count);
                return true;
            }
            return false;
        }

        public bool HasCoffee()
        {
            return coffeeStack.Count > 0;
        }

        public int GetCoffeeCount()
        {
            return coffeeStack.Count;
        }

        void UpdateBeanStackVisual()
        {
            for (int i = 0; i < beanStack.Count; i++)
            {
                if (beanStack[i] != null)
                {
                    beanStack[i].transform.localPosition = Vector3.up * (i * 0.3f);
                }
            }
        }

        void UpdateCoffeeStackVisual()
        {
            for (int i = 0; i < coffeeStack.Count; i++)
            {
                if (coffeeStack[i] != null)
                {
                    coffeeStack[i].transform.localPosition = Vector3.up * (i * 0.3f);
                }
            }
        }
    }
}
