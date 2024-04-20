using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class GatePellet : MonoBehaviour
{
    private enum State
    {
        DontCollect,
        Available,
        Activated
    }
    private State state = State.DontCollect;

    public UnityEvent<int> OnCollected { get; private set; } = new();

    [SerializeField]
    private AudioClip pickedSFX;

    private bool beenSetUp = false;
    private int order = -1;
    private GateDisplay gateDisplay;

    private void Awake()
    {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.25f;
    }

    private void OnEnable()
    {
        GameEvents.OnGatesSequenceUpdate.Add(UpdateGateState);
    }

    private void OnDisable()
    {
        GameEvents.OnGatesSequenceUpdate.Remove(UpdateGateState);
    }

    private void Update()
    {
        switch (order)
        {
            case 0:
                Debug.DrawRay(transform.position, Vector3.up, Color.red);
                break;
            case 1:
                Debug.DrawRay(transform.position, Vector3.up, Color.yellow);
                break;
            case 2:
                Debug.DrawRay(transform.position, Vector3.up, Color.green);
                break;
        }

        switch (state)
        {
            case State.DontCollect:
                Debug.DrawRay(transform.position, Vector3.left, Color.red);
                break;
            case State.Available:
                Debug.DrawRay(transform.position, Vector3.left, Color.yellow);
                break;
            case State.Activated:
                Debug.DrawRay(transform.position, Vector3.left, Color.green);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (beenSetUp && state != State.Activated)
        {
            if (collision.gameObject.Equals(PlayerInput.GOInstance))
            {
                if (pickedSFX != null && SFXController.Instance != null)
                {
                    SFXController.Instance.RequestPlay(pickedSFX, 15000);
                }

                OnCollected.Invoke(order);
                //OnCollected.RemoveAllListeners();
                //Destroy(gameObject);
            }
        }
    }

    public void Setup(int order, GateDisplay gateDisplay)
    {
        this.order = order;
        this.gateDisplay = gateDisplay;

        beenSetUp = true;
    }

    private void UpdateGateState(int currentOrder)
    {
        if (currentOrder > order)
        {
            state = State.Activated;
        }
        else if (currentOrder == order)
        {
            state = State.Available;
        }
        else
        {
            state = State.DontCollect;
        }
    }
}
