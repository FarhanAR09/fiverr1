using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSpriteTargeter : MonoBehaviour
{
    public enum ToggleGraphicsMode
    {
        Sudden
    }

    private Toggle toggle;

    [SerializeField]
    private SpriteRenderer targetSpriteRenderer;
    [SerializeField]
    private ToggleGraphicsMode mode = ToggleGraphicsMode.Sudden;

    private void OnEnable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(UpdateGraphics);
        }
    }

    private void OnDisable()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(UpdateGraphics);
        }
    }

    private void Start()
    {
        TryGetComponent(out toggle);
    }

    private void UpdateGraphics(bool isOn)
    {
        if (targetSpriteRenderer != null)
        {
            switch (mode)
            {
                case ToggleGraphicsMode.Sudden:
                    targetSpriteRenderer.enabled = isOn;
                    break;
                default:
                    targetSpriteRenderer.enabled = isOn;
                    break;
            }
        }
    }
}
