using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace CoreAttack
{
    public class EnemyPrefabsProvider : MonoBehaviour
    {
        private static EnemyPrefabsProvider _instance;
        public static EnemyPrefabsProvider Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogWarning("EnemyPrefabsProvider is NULL");
                return _instance;
            }
            private set => _instance = value;
        }

        private readonly Dictionary<EnemyType, GameObject> prefabs = new();
        [SerializeField]
        private List<EnemyType> prefabsKeys = new();
        [SerializeField]
        private List<GameObject> prefabsGameObjects = new();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            UpdateDictionary();
        }

        public GameObject TryGetPrefab(EnemyType key)
        {
            if (prefabs == null)
            {
                Debug.LogWarning("EnemyPrefabsProvider prefabs is NULL");
                return null;
            }
            if (!prefabs.ContainsKey(key))
            {
                Debug.LogWarning("EnemyPrefabsProvider prefabs does not contain key " + key);
                return null;
            }
            return prefabs[key];
        }

        public void UpdateDictionary()
        {
            if (prefabsKeys.Count == prefabsGameObjects.Count)
            {
                prefabs.Clear();
                for (int i = 0; i < prefabsKeys.Count && i < prefabsGameObjects.Count; i++)
                {
                    prefabs[prefabsKeys[i]] = prefabsGameObjects[i];
                }
            }
            else Debug.LogWarning("EnemyPrefabsProvider prefabsKeys.Count != prefabsGameObjects.Count");
        }
    }
}