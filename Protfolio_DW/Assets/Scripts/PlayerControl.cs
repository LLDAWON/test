using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed = 5f;
    [SerializeField]
    private float _runSpeed = 10f;
    [SerializeField]
    private float _jumpPower = 100.0f;

    private float _moveSpeed;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");

        _moveSpeed = 0.0f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _moveSpeed = _runSpeed;
            _animator.SetBool("Dash", true);
        }
        else if (horizontal != 0)
        {
            _moveSpeed = _walkSpeed;
            _animator.SetBool("Dash", false);
        }

        transform.Translate(Vector2.right * horizontal * _moveSpeed * Time.deltaTime);

        _animator.SetFloat("Speed", Mathf.Abs(_moveSpeed));

        if (horizontal < 0.0f)
            _spriteRenderer.flipX = true;
        else if (horizontal > 0.0f)
            _spriteRenderer.flipX = false;
    }

    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);
            _animator.SetBool("IsAir", true);
        }
        _animator.SetFloat("Velocity", _rigidbody2D.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Tile")
        {
            _animator.SetBool("IsAir", false);
        }
    }

}
