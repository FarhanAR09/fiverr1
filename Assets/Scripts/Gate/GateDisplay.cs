using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(SpriteRenderer))]
public class GateDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem psAbsorb, psExplode;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (psAbsorb != null)
        {
            ParticleSystem.EmissionModule psAbsorbEmission = psAbsorb.emission;
            psAbsorbEmission.enabled = false;
        }
    }

    public void ChangeState(bool isActive)
    {
        spriteRenderer.color = isActive ? Color.green : Color.white;

        //Particle System
        if (psAbsorb != null)
        {
            ParticleSystem.EmissionModule psAbsorbEmission = psAbsorb.emission;
            if (isActive)
            {
                psAbsorb.Play();
                psAbsorbEmission.enabled = true;
            }
            else
            {
                psAbsorbEmission.enabled = false;
            }
        }

        if (psExplode != null)
        {
            psExplode.Emit(10);
        }
    }
}
