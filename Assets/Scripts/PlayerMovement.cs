using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private Joystick joystick;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundChecker;
	[SerializeField] private float checkGroundRadius = 0.4f;
	[SerializeField] private float jumpHeight = 6f;

	private CharacterController character;

	private float maxSpeed = 8f;
	private float acceleration = 30f;
	private float deceleration = 20f;
	float verticalSpeed = 0;
	float horizontalSpeed = 0;

	private float verticalVelocity;
	private float gravity = -40f;
	private bool isGrounded;

	private float jumpBufferTime = 0.1f; //Buffer time is working wrong when the second jump is too soon, it still makes jump
	private float jumpBufferCounter;

	private float coyoteTime = 0.2f;
	private float coyoteTimeCounter;

	private bool canMove = true;

	[SerializeField] private Animator animator;

	private void OnEnable()
	{
		InputManager.OnTouchStarted += HandleJumpBuffer;
		InputManager.OnTouchEnded += GoDown;
	}

	private void OnDisable()
	{
		InputManager.OnTouchStarted -= HandleJumpBuffer;
		InputManager.OnTouchEnded -= GoDown;
	}

	private void Start()
	{
		character = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (!canMove)
		{
			horizontalSpeed = 0;
			verticalSpeed = 0;
			verticalVelocity = 0;
		}
		if (isGrounded)
		{
			coyoteTimeCounter = coyoteTime;
		}
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
		}

		jumpBufferCounter -= Time.deltaTime;
		Jump();

		if (joystick.Horizontal != 0 ||  joystick.Vertical != 0)
		{
			Rotation();
		}
		HandleAnimation();
	}

	private void FixedUpdate()
	{
		HandleVelocityWhenGrounded();
		Move();
		DoGravity();
	}

	private void HandleAnimation()
	{
		if (joystick.Horizontal != 0 || joystick.Vertical != 0)
		{
			animator.SetBool("IsRunning", true);
		}
		else
		{
			animator.SetBool("IsRunning", false);
		}

		if (!isGrounded)
		{
			animator.SetBool("IsJumping", true);
			animator.SetBool("IsRunning", false);
		}
		else
		{
			animator.SetBool("IsJumping", false);
		}
	}

	private void Move()
	{
		float horizontalSpeedDif = maxSpeed - horizontalSpeed;
		float verticalSpeedDif = maxSpeed - verticalSpeed;
		float maxHorSpeed = joystick.Horizontal * maxSpeed;
		float maxVertSpeed = joystick.Vertical * maxSpeed;


		if (joystick.Horizontal != 0)
		{
			horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, maxHorSpeed, acceleration*Time.deltaTime);
		}

		if (joystick.Vertical != 0)
		{
			verticalSpeed = Mathf.MoveTowards(verticalSpeed, maxVertSpeed, acceleration * Time.deltaTime);
		}

		if (joystick.Horizontal == 0)
		{
			horizontalSpeed = Mathf.MoveTowards(horizontalSpeed, 0, deceleration * Time.deltaTime);
		}

		if (joystick.Vertical == 0)
		{
			verticalSpeed = Mathf.MoveTowards(verticalSpeed, 0, deceleration * Time.deltaTime);
		}

		//verticalSpeed = Mathf.Clamp(verticalSpeed, -maxSpeed, maxSpeed);
		//horizontalSpeed = Mathf.Clamp(horizontalSpeed, -maxSpeed, maxSpeed);

		var velocity = new Vector3(horizontalSpeed, 0, verticalSpeed);
		character.Move(velocity * Time.fixedDeltaTime);
	}

	private void Rotation()
	{
		var direction = new Vector3 (joystick.Horizontal, 0, joystick.Vertical);
		Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
		transform.rotation = rotation;
	}

	public void Jump()
	{
		if (coyoteTimeCounter > 0 && jumpBufferCounter > 0)
		{
			verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
			coyoteTimeCounter = 0f;
			jumpBufferCounter = 0f;
		}
	}

	private void HandleJumpBuffer()
	{
		jumpBufferCounter = jumpBufferTime;
	}

	private void DoGravity()
	{
		verticalVelocity += gravity * Time.fixedDeltaTime;
		character.Move(Vector3.up * verticalVelocity * Time.fixedDeltaTime);
	}

	private bool CheckIsGrounded()
	{
		bool result = Physics.CheckSphere(groundChecker.position, checkGroundRadius, groundLayer);
		return result;
	}

	private void HandleVelocityWhenGrounded()
	{
		isGrounded = CheckIsGrounded();

		if (isGrounded && verticalVelocity < 0)
		{
			verticalVelocity = -2;
		}
	}

	private void GoDown()
	{
		if (verticalVelocity > 0)
		{
			verticalVelocity *= 0.2f;
		}
	}

	public void Stop()
	{
		horizontalSpeed = 0f;
		verticalSpeed = 0f;
		verticalVelocity = 0f;
		canMove = false;
	}
	public void ContinueMovement()
	{
		canMove = true;
	}
}
