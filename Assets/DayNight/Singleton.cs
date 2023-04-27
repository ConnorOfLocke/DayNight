using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CRAB.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            Instance = this as T;
            Initialise();
        }

        protected abstract void Initialise();

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
