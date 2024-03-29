using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(SpriteRenderer))]
public class GateDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeState(bool isActive)
    {
        spriteRenderer.color = isActive ? Color.green : Color.white;
    }
}
