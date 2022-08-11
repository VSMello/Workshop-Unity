using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private CharacterController2D controller;
	private Animator animator;

	[SerializeField] private float runSpeed = 40f;

	private float horizontalMove = 0f;
	private bool jump = false;
	private bool crouch = false;

    private void Start(){
        controller = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
    }

	// Update is called once per frame
	private void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			animator.SetBool("IsJumping", true);
		}

		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		} else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}

	}

	public void OnLanding ()
	{
		animator.SetBool("IsJumping", false);
	}

	public void OnCrouching (bool isCrouching)
	{
		animator.SetBool("IsCrouching", isCrouching);
	}

	private void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}
}