using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isTouchingGround;

    private Rigidbody2D _rb;
    private Animator _animator;
    private float _walkInput;
    private bool _jumpInput;
    private float _lastXValue;
    
    private void Start() {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _animator = gameObject.GetComponent<Animator>();
    } //end Start()

    private void Update() {
        Inputs();
        Jump();
    } //end Update()

    private void FixedUpdate() {
        Walk();
    } //end FixedUpdate()

    private void Inputs() {
        _walkInput = Input.GetAxisRaw("Horizontal");
        _jumpInput = Input.GetButtonDown("Jump");
    } //end Inputs()

    private void Walk() {
        _rb.velocity = new Vector2(_walkInput * speed, _rb.velocity.y);
        _lastXValue = _walkInput;
        _animator.SetFloat("WalkX", _walkInput);

        if (_rb.velocity.x != 0 && isTouchingGround) {
            _animator.SetBool("IsWalking", true); 
        } else {
            _animator.SetBool("IsWalking", false);
        }
    } //end Walk()

    private void Jump() {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_jumpInput && isTouchingGround) {
            _animator.SetBool("Jump", true);
            _animator.SetBool("IsTouchingGround", false);
            _rb.velocity = new Vector2(_rb.velocity.x, jumpSpeed);    
        } else if (_rb.velocity.y > 0) {
            _animator.SetBool("Jump", false);
            _animator.SetBool("IsFalling", true);            
        } else if (isTouchingGround) {
            _animator.SetBool("IsTouchingGround", true);
            _animator.SetBool("IsFalling", false);
        }
    } //end Jump()
}
