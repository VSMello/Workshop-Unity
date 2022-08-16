using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 250f;							// Quantidade de for�a adicionada ao pulo.
	[Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;			// Velocidade m�xima ao abaixar. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// Quantidade de suaviza��o do movimento.
	[SerializeField] private LayerMask groundMask;								// M�scara que define o que � ch�o.
	[SerializeField] private Transform groundCheck;								// Marcador de posi��o para checar quando o player est� no ch�o.
	[SerializeField] private Transform ceilingCheck;							// Marcador de posi��o para checar se h� um teto.
	[SerializeField] private Collider2D crouchDisableCollider;					// Colisor que ser� desabilitado quando abaixar.
	[SerializeField] private bool airControl = false;
	
	private const float GroundedRadius = .2f;	// Raio do OverlapCircle para saber se est� no ch�o.
	private bool _grounded;						// Se player est� ou n�o no ch�o.
	private const float CeilingRadius = .2f;	// Raio do OverlapCircle para saber se player pode levantar.
	private Rigidbody2D _rb;
	private bool _facingRight = true;			// Para determinar qual lado o player est� olhando.
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

		// O player est� no ch�o se o circlecast utilizando a posi��o do groundCheck escosta qualquer objeto designado como ch�o.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedRadius, groundMask);
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
			// Se h� um colisor bloqueando o player de levantar, mant�m abaixado
			if (Physics2D.OverlapCircle(ceilingCheck.position, CeilingRadius, groundMask))
			{
				crouch = true;
			}
		}

		// S� controla o personagem se est� no ch�o ou airControl == true.
		if (_grounded || airControl)
		{
			// Se agaixado
			if (crouch)
			{
				if (!_wasCrouching)
				{
					_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduz a velocidade atrav�z do multiplicador crouchSpeed.
				move *= crouchSpeed;

				// Desativa um dos colisores quando abaixado.
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			} else
			{
				// Ativa o colisor quando n�o est� mais abaixado
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = true;

				if (_wasCrouching)
				{
					_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move o personagem encontrando a velocidade alvo
			Vector3 targetVelocity = new Vector2(move * 10f, _rb.velocity.y);
			// Depois suaviza o movimento e aplica ao personagem
			_rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, movementSmoothing);

			// Se o input está movendo o player para a direita e o personagem está olhando para esquerda...
			if (move > 0 && !_facingRight)
			{
				// ... vira o player.
				Flip();
			}
			// Se o input está movendo o player para a esquerda e o personagem está olhando para direita...
			else if (move < 0 && _facingRight)
			{
				// ... vira o player.
				Flip();
			}
		}
		// Se o player deve pular...
		if (_grounded && jump)
		{
			// Adiciona força vertical.
			_grounded = false;
			_rb.AddForce(new Vector2(0f, jumpForce));
		}
	}


	private void Flip()
	{
		// Altera a rotulação do player, se está ou não olhando para a direita.
		_facingRight = !_facingRight;

		// Multiplica o localScale do player por -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}