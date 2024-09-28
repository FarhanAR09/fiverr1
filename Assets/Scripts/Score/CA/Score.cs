using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class Score : MonoBehaviour
    {
        //Singleton
        private static Score _instance;
        public static Score Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("Score Instance is null");
                }
                return _instance;
            }
            set => _instance = value;
        }

        public int Amount { get; private set; }

        public void AddScore(int amount)
        {
            Amount += amount;
        }

        public void RemoveScore(int amount)
        { 
            Amount -= amount;
        }
    }
}