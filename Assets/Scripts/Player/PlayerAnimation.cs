using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerPowerUpManager))]
public class PlayerAnimation : MonoBehaviour
{
    private PlayerInput input;
    [SerializeField]
    private Transform spriteTransform;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private PlayerPowerUpManager powerManager;
    [SerializeField]
    private ParticleSystem trail;
    private ParticleSystem.MainModule trailMain;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        if (trail != null)
        {
            trailMain = trail.main;
        }
        else Debug.Log("Trail particle system is null");

        powerManager = GetComponent<PlayerPowerUpManager>();
    }

    private void Update()
    {
        //Sprite rotation
        if (spriteTransform != null)
        {
            spriteTransform.localScale = new(1, 1, 1);
            spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            switch (input.Direction)
            {
                case MovementDirection.Up:
                    spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case MovementDirection.Down:
                    spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    break;
                case MovementDirection.Left:
                    spriteTransform.localScale = new(-1, 1, 1);
                    break;
                case MovementDirection.Right:
                    spriteTransform.localScale = new(1, 1, 1);
                    break;
            }
        }

        if (spriteRenderer)
        {
            Color playerGlowColor = powerManager.AvailableCharge switch
            {
                0 => Color.red,
                1 => Color.yellow,
                2 => Color.green,
                3 => Color.cyan,
                _ => Color.red
            };
            spriteRenderer.material.SetColor("_Color", playerGlowColor);
        }

        if (trail != null)
        {
            trailMain.startColor = powerManager.AvailableCharge switch
            {
                0 => (ParticleSystem.MinMaxGradient)Color.red,
                1 => (ParticleSystem.MinMaxGradient)Color.yellow,
                2 => (ParticleSystem.MinMaxGradient)Color.green,
                3 => (ParticleSystem.MinMaxGradient)Color.cyan,
                _ => (ParticleSystem.MinMaxGradient)Color.red,
            };
        }
    }
}
