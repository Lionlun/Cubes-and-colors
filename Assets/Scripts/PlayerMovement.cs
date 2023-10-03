using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private Joystick joystick;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundChecker;
	[SerializeField] private float checkGroundRadius = 0.4f;
	[SerializeField] private float jumpHeight = 4f;

	private CharacterController character;

	private float verticalVelocity;
	private float speed = 4;
	private float gravity = -9.81f;
	private bool isGrounded;

	private void OnEnable()
	{
		InputManager.OnTouchStarted += Jump;
	}

	private void OnDisable()
	{
		InputManager.OnTouchStarted -= Jump;
	}

	private void Start()
	{
		character = GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{
		HandleVelocityWhenGrounded();
		Move();
		DoGravity();
	}

	private void Move()
	{
		var velocity = new Vector3(joystick.Horizontal, 0, joystick.Vertical) * speed;
		character.Move(velocity * Time.fixedDeltaTime);
	}

	private void Jump()
	{
		if (isGrounded)
		{
			verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
		}
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
}
