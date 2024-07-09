using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private Rigidbody rb;

    private Vector2 direction;

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new(direction.x, 0, direction.y);
        rb.velocity = moveSpeed * move;
    }

    void OnMove(InputValue input)
    {
        Vector2 dir = input.Get<Vector2>();
        Debug.Log(dir);

        direction = dir;
    }
}
