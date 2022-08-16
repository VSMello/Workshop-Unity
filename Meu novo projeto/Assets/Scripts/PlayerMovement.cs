using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private CharacterController2D _controller;
	private Animator _animator;

	[SerializeField] private float runSpeed = 40f;

	private float _horizontalMove = 0f;
	private bool _jump = false;
	private bool _crouch = false;

    private void Start(){
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
    }

	// Update is called once per frame
	private void Update () {

		_horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		_animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			_jump = true;
			_animator.SetBool("IsJumping", true);
		}

		if (Input.GetButtonDown("Crouch"))
		{
			_crouch = true;
		} else if (Input.GetButtonUp("Crouch"))
		{
			_crouch = false;
		}

	}

	public void OnLanding ()
	{
		_animator.SetBool("IsJumping", false);
	}

	public void OnCrouching (bool isCrouching)
	{
		_animator.SetBool("IsCrouching", isCrouching);
	}

	private void FixedUpdate ()
	{
		// Move o personagem
		_controller.Move(_horizontalMove * Time.fixedDeltaTime, _crouch, _jump);
		_jump = false;
	}
}