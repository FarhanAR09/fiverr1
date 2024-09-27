using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CoreAttack
{
    public class SpawnerController : MonoBehaviour
    {
        [SerializeField]
        private float spawnInterval = 1f;
        private float spawnTimer = 0f;

        private Dictionary<EnemyType, float> enemyTypeProbabilities = new();
        [SerializeField]
        private List<EnemyType> enemyTypes = new();
        [Tooltip("Will be normalized later")]
        [SerializeField]
        private List<float> probabilities = new();
        private List<float> cumulativeProbabilities = new();

        private void Awake()
        {
            if (enemyTypes.Count > 0 && enemyTypes.Count != probabilities.Count)
            {
                Debug.LogWarning("enemyTypes.Count != probabilities.Count");
            }
            else
            {
                //Normalize probabilities
                float total = 0f;
                for (int i = 0; i < probabilities.Count; i++)
                {
                    total += probabilities[i];
                }
                for (int i = 0; i < probabilities.Count; i++)
                {
                    probabilities[i] = probabilities[i] / total;
                }

                //Add cumulative probabilities
                total = 0f;
                for (int i = 0; i < probabilities.Count; i++)
                {
                    total += probabilities[i];
                    cumulativeProbabilities.Add(total);
                }

                //Add to dictionary
                for (int i = 0; i < enemyTypes.Count; i++)
                {
                    enemyTypeProbabilities.Add(enemyTypes[i], probabilities[i]);
                }
            }
        }

        private void FixedUpdate()
        {
            if (spawnTimer < spawnInterval)
            {
                spawnTimer += Time.fixedDeltaTime;
            }
            else
            {
                spawnTimer = 0f;
                if (EnemyPrefabsProvider.Instance != null)
                {
                    EnemyType spawningType = GetTypeRandomly();
                    print("Spawning " + spawningType.ToString());
                    GameObject prefab = EnemyPrefabsProvider.Instance.TryGetPrefab(spawningType);
                    if (prefab != null)
                    {
                        Instantiate(prefab, transform.position, new Quaternion());
                    }
                    else Debug.LogWarning(spawningType.ToString() + " is null");
                }
                else Debug.LogWarning("EnemyPrefabsProvider is null");
            }
        }

        private EnemyType GetTypeRandomly()
        {
            float spawnProbability = Random.Range(0f, 1f);
            for (int i = 0; i < cumulativeProbabilities.Count; i++)
            {
                if (spawnProbability <= cumulativeProbabilities[i])
                {
                    return enemyTypes[i];
                }
            }
            return 0;
        }
    }
}