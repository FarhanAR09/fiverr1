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
    [SerializeField]
    private Sprite inactiveSprite;
    public Sprite activeSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (inactiveSprite == null)
            inactiveSprite = spriteRenderer.sprite;
        if (activeSprite == null)
            activeSprite = spriteRenderer.sprite;
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
        spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

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
                psAbsorb.Emit(15);
                IEnumerator DelayDeactivate()
                {
                    yield return new WaitForSeconds(1f);
                    psAbsorbEmission.enabled = false;
                }
                StopCoroutine(DelayDeactivate());
                StartCoroutine(DelayDeactivate());
            }
        }

        if (isActive && psExplode != null)
        {
            psExplode.Emit(10);
        }
    }
}
