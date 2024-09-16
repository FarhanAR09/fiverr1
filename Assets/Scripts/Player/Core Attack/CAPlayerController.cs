using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    [RequireComponent(typeof(CAMovementController))]
    public class CAPlayerController : MonoBehaviour
    {
        private CAMovementController movementController;

        [Tooltip("Gun GameObject with ICAGun interface")]
        [SerializeField]
        private GameObject gun;
        private ICAGun iGun;

        private Camera mainCam;

        private void Awake()
        {
            TryGetComponent(out movementController);

            if (gun != null && gun.TryGetComponent(out ICAGun _iGun))
            {
                iGun = _iGun;
            }
            else Debug.LogWarning("ICAGun is null in " + name);

            mainCam = Camera.main;
        }

        private void Update()
        {
            if (iGun != null && gun != null)
            {
                Vector2 aimDirection = ((Vector2)(mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position)).normalized;
                gun.transform.position = 1f * aimDirection + (Vector2)transform.position;
                gun.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg));

                if (Input.GetMouseButtonDown(0))
                {
                    iGun.Shoot();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    iGun.ToggleShoot(!iGun.ShootToggled);
                }
            }
            else Debug.LogWarning("Gun is null in " + name);
        }

        private void FixedUpdate()
        {
            if (movementController != null)
            {
                Vector2 input = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (input != Vector2.zero)
                {
                    print(input);
                    movementController.MoveTo(input + (Vector2)transform.position);
                }
            }
            else Debug.LogWarning("Movement Controller is null in " + name);
        }
    }
}
