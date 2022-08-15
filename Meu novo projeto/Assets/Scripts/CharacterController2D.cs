using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 250f;							// Quantidade de força adicionada ao pulo.
	[Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;			// Velocidade máxima ao abaixar. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// Quantidade de suavização do movimento.
	[SerializeField] private LayerMask groundMask;								// Máscara que define o que é chão.
	[SerializeField] private Transform groundCheck;								// Marcador de posição para checar quando o player está no chão.
	[SerializeField] private Transform ceilingCheck;							// Marcador de posição para checar se há um teto.
	[SerializeField] private Collider2D crouchDisableCollider;					// Colisor que será desabilitado quando abaixar.

	private const float _groundedRadius = .2f;	// Raio do OverlapCircle para saber se está no chão.
	private bool _grounded;						// Se player está ou não no chão.
	private const float _ceilingRadius = .2f;	// Raio do OverlapCircle para saber se player pode levantar.
	private Rigidbody2D _rb;
	private bool _facingRight = true;			// Para determinar qual lado o player está olhando.
	private Vector3 _velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool _wasCrouching = false;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = _grounded;
		_grounded = false;

		// O player está no chão se o circlecast utilizando a posição do groundCheck escosta qualquer objeto designado como chão.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, _groundedRadius, groundMask);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				_grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	public void Move(float move, bool crouch, bool jump)
	{
		// Se abaixado, checar se personagem pode se levantar
		if (!crouch)
		{
			// Se há um colisor bloqueando o player de levantar, mantém abaixado
			if (Physics2D.OverlapCircle(ceilingCheck.position, _ceilingRadius, groundMask))
			{
				crouch = true;
			}
		}

		// Só controla o personagem se está no chão.
		if (_grounded)
		{

			// Se agaixado
			if (crouch)
			{
				if (!_wasCrouching)
				{
					_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduz a velocidade atravéz do multiplicador crouchSpeed.
				move *= crouchSpeed;

				// Desativa um dos colisores quando abaixado.
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			} else
			{
				// Ativa o colisor quando não está mais abaixado
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = true;

				if (_wasCrouching)
				{
					_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, _rb.velocity.y);
			// And then smoothing it out and applying it to the character
			_rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !_facingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && _facingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (_grounded && jump)
		{
			// Add a vertical force to the player.
			_grounded = false;
			_rb.AddForce(new Vector2(0f, jumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}