using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class DebugHealthBar : MonoBehaviour
    {
        //Health
        [Tooltip("GameObject with IHealthOwner")]
        [SerializeField]
        private GameObject goHealthOwner;
        private IHealthOwner _healthOwner;
        private IHealthOwner HealthOwner
        {
            get
            {
                if (_healthOwner == null)
                    Debug.LogWarning("HealthOwner is null");
                return _healthOwner;
            }
            set
            {
                _healthOwner = value;
            }
        }

        //Amount
        private float Amount
        {
            get { 
                if (HealthOwner == null)
                {
                    return 0f;
                }
                return HealthOwner.CurrentHealth / HealthOwner.MaxHealth;
            }
        }
        private float initScale = 1f;

        //Health Bar
        [SerializeField]
        private GameObject filledBar;

        private void OnEnable()
        {
            if (HealthOwner != null)
            {
                HealthOwner.OnHealthUpdated += UpdateBar;
            }
        }

        private void OnDisable()
        {
            if (HealthOwner != null)
            {
                HealthOwner.OnHealthUpdated -= UpdateBar;
            }
        }

        private void Awake()
        {
            goHealthOwner.TryGetComponent(out _healthOwner);
        }

        private void Start()
        {
            if (filledBar != null)
            {
                initScale = filledBar.transform.localScale.x;
            }
        }

        public void Setup(IHealthOwner healthOwner = null)
        {
            if (healthOwner != null)
            {
                HealthOwner = healthOwner;
            }
        }

        private void UpdateBar(float _)
        {
            if (filledBar != null)
            {
                filledBar.transform.localScale = new Vector3(Amount * initScale, filledBar.transform.localScale.y, filledBar.transform.localScale.z);
            }
        }
    }
}