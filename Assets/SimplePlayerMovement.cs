using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float movementSpeed;

    private float vertical;
    private float horizontal;

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void GetInput()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    private void ApplyMovement()
    {
        Vector3 moveDir = transform.up * vertical + transform.right * horizontal;
        rb.velocity = (moveDir * movementSpeed);
    }

    private void OnParticleCollision(GameObject other)
    {
        BulletHellManager.Instance.EndBattle(true);
    }
}
