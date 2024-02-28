using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private Joystick joystick;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundChecker;
	[SerializeField] private float checkGroundRadius = 0.3f;
	[SerializeField] private float jumpHeight = 6f;

	[SerializeField] private Transform finishLedgePosition;

	private CharacterController characterController;

	private float maxSpeed = 8.5f;
	private float acceleration = 30f;
	private float deceleration = 20f;
	float verticalSpeed = 0;
	float horizontalSpeed = 0;

	private float verticalVelocity;
	private float gravity = -40f;
	private bool isGrounded;
	private bool canJump;
	private bool canDoubleJump;

	private float jumpBufferTime = 0.1f;
	private float jumpBufferCounter;

	private float coyoteTime = 0.2f;
	private float coyoteTimeCounter;
	private float downVelocityMultiplier = 1f;

	private bool canMove = true;

	[SerializeField] private AudioClip jumpSound;

	[SerializeField] private Animator animator;
	[SerializeField] private LedgeDetection ledgeDetection;
	private float delayAfterJump =0.5f;
	private float delayForSecondJump = 0.3f;
	[SerializeField] private OnCubeTrigger onCubeTrigger;

	private void OnEnable()
	{
		InputManager.OnTouchStarted += HandleJumpBuffer;
        InputManager.OnTouchStarted += Jump;
        InputManager.OnTouchEnded += GoDown;
	}

	private void OnDisable()
	{
		InputManager.OnTouchStarted -= HandleJumpBuffer;
        InputManager.OnTouchStarted -= Jump;
        InputManager.OnTouchEnded -= GoDown;
	}

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (!canMove)
		{
			horizontalSpeed = 0;
			verticalSpeed = 0;
			verticalVelocity = 0;
		}

		if (isGrounded && delayAfterJump <=0)
		{
			coyoteTimeCounter = coyoteTime;
			downVelocityMultiplier = 1f;
			canDoubleJump = true;
			canJump = true;

        }
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
			delayAfterJump -= Time.deltaTime;
		}

		jumpBufferCounter -= Time.deltaTime;
		//Jump();

		if(delayForSecondJump >= 0)
		{
			delayForSecondJump -= Time.deltaTime;
		}
		HandleAnimation();

		if (canMove)
		{
			HandleVelocityWhenGrounded();
			Move();
			DoGravity();

			if (joystick.Horizontal != 0 || joystick.Vertical != 0)
			{
				Rotation();
			}
		}

        if (characterController.velocity.y < -100 && !isGrounded)
        {
			animator.SetTrigger("JumpApex");
        }
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
		//float horizontalSpeedDif = maxSpeed - horizontalSpeed;
		//float verticalSpeedDif = maxSpeed - verticalSpeed;
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

		var velocity = new Vector3(horizontalSpeed, 0, verticalSpeed);
		characterController.Move(velocity * Time.deltaTime);
	}

	private void Rotation()
	{
		var direction = new Vector3 (joystick.Horizontal, 0, joystick.Vertical);
		Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
		transform.rotation = rotation;
	}

	public void Jump()
	{
		if (canJump)
		{
			if(onCubeTrigger.CurrentCube != null)
			{
				if(onCubeTrigger.CurrentCube.Rb?.velocity.y > 0)
				{
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity) + (onCubeTrigger.CurrentCube.Rb.velocity.y)/2;
				}
				else
				{
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
                }
			}
			else
			{
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }

			
			coyoteTimeCounter = 0f;
			jumpBufferCounter = 0f;
			delayAfterJump = 0.5f;
			delayForSecondJump = 0.3f;
            canJump = false;
        
        }
		else if(canDoubleJump && delayForSecondJump <= 0)
		{
			animator.SetTrigger("DoubleJump");
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
            coyoteTimeCounter = coyoteTime;
            downVelocityMultiplier = 1f;
            jumpBufferCounter = jumpBufferTime;
            canDoubleJump = false;

        }
	}
	public void DoubleJump()
	{
        /*if (canDoubleJump)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
            coyoteTimeCounter = coyoteTime;
            downVelocityMultiplier = 1f;
            jumpBufferCounter = jumpBufferTime;
            canDoubleJump = false;
        }*/
    }

	private void HandleJumpBuffer()
	{
		jumpBufferCounter = jumpBufferTime;
	}

	private void DoGravity()
	{
		verticalVelocity += gravity * Time.deltaTime;
		characterController.Move(Vector3.up * verticalVelocity * downVelocityMultiplier * Time.deltaTime);
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
		if(verticalVelocity > 0)
		{
			verticalVelocity = 0f;
			downVelocityMultiplier = 1.6f;
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
	public void FinishClimb()
	{
		transform.position = finishLedgePosition.position;
        canMove = true;

    }

	public void LedgeJump()
	{
        canMove = true;
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    public float GetVerticalVelocity()
	{
		return verticalVelocity;
	}

	public void PushUp()
	{
		verticalVelocity = 30;
	}
	public void FollowTransform(Vector3 direction)
	{
		transform.position += direction;
    }
    public void FollowSimpleMove(Vector3 direction)
    {
        characterController.Move(direction);
    }
    public void Follow(Transform transform)
	{
		var offset = 1f;
		this.transform.position = new Vector3(this.transform.position.x, transform.position.y + offset, this.transform.position.z);
	}

	public void SetIsGrounded(bool isGrounded)
	{
		this.isGrounded = isGrounded;
	}
	public void TriggerJump(float force)
	{
		canJump = true;
        verticalVelocity = force;
    }
}
