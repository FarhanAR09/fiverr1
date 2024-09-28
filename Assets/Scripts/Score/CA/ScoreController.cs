using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class ScoreController : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnCAEnemyDeath.Add(AddScore);
        }

        private void OnDisable()
        {
            GameEvents.OnCAEnemyDeath.Remove(AddScore);
        }

        private void AddScore(Enemy enemy)
        {
            Score.Instance.AddScore();
        }
    }
}