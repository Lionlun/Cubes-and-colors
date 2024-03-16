using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private LineRenderer lineRenderer;

	[SerializeField] private Joystick joystick;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundChecker;
	[SerializeField] private Transform diveGroundChecker;
	[SerializeField] private float checkGroundRadius = 0.3f;
	[SerializeField] private float jumpHeight = 6f;

	[SerializeField] private Transform finishLedgePosition;

	private CharacterController characterController;

	private float maxSpeed = 9.5f; //8/5
	private float acceleration = 30f;
	private float deceleration = 20f;
	float verticalSpeed = 0;
	float horizontalSpeed = 0;

	private float verticalVelocity;
	private float gravity = -40f;
	public bool IsGrounded { get; private set; }
	private bool canJump;

	private float jumpBufferTime = 0.1f;
	private float jumpBufferCounter;

	private float coyoteTime = 0.15f;
	private float coyoteTimeCounter;
	private float downVelocityMultiplier = 1f;

	private bool canMove = true;

	[SerializeField] private AudioClip jumpSound;

	[SerializeField] private Animator animator;
	[SerializeField] private LedgeDetection ledgeDetection;
	private float delayAfterJump = 0.5f;
	private float delayForSecondJump = 0.3f;
	[SerializeField] private OnCubeTrigger onCubeTrigger;
	private bool isGravityOn = true;
	[SerializeField] private LayerMask cubeLayer;
	public bool IsDivingDown { get; set; }
	public bool IsButtonDown { get; set; }
	[SerializeField] private ParticleSystem fallingDownFX;

	private void OnEnable()
	{
		InputManager.OnTouchStarted += HandleJumpBuffer;
		InputManager.OnTouchStarted += Jump;
		InputManager.OnTouchEnded += GoDown;
		InputManager.OnSecondEnded += CancelDive;
		InputManager.OnTouchEnded += CancelDive;
		SwipeDetection.OnSwipeDetected += StartDashCoroutine;
	}

	private void OnDisable()
	{
		InputManager.OnTouchStarted -= HandleJumpBuffer;
		InputManager.OnTouchStarted -= Jump;
		InputManager.OnTouchEnded -= GoDown;
		InputManager.OnSecondEnded -= CancelDive;
		InputManager.OnTouchEnded -= CancelDive;
		SwipeDetection.OnSwipeDetected -= StartDashCoroutine;
	}

	private void Start()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{

	}

	private void Update()
	{
		/*if (CheckCloseToGround())
		{
            animator.SetBool("IsDiving", false);
        }*/

		if (!canMove)
		{
			horizontalSpeed = 0;
			verticalSpeed = 0;
			verticalVelocity = 0;
		}

		if (IsGrounded && delayAfterJump <= 0)
		{
			coyoteTimeCounter = coyoteTime;
			downVelocityMultiplier = 1f;
			canJump = true;

		}
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
			delayAfterJump -= Time.deltaTime;
		}

		jumpBufferCounter -= Time.deltaTime;

		if (delayForSecondJump >= 0)
		{
			delayForSecondJump -= Time.deltaTime;
		}
		

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
		else
		{
            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                Rotation();
            }
        }

		if (characterController.velocity.y < -100 && !IsGrounded)
		{
			animator.SetTrigger("JumpApex");
		}
        HandleAnimation();
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

		if (!IsGrounded)
		{
			animator.SetBool("IsJumping", true);
			animator.SetBool("IsRunning", false);
		}
		else
		{
			animator.SetBool("IsJumping", false);
		}

		if (IsGrounded && InputManager.CheckIfButtonIsDown() && IsDivingDown)
		{
			animator.SetBool("IsGrappling", true);
			fallingDownFX.Stop();
			canMove = false;
		}

		if (!IsGrounded || !InputManager.CheckIfButtonIsDown())
        {
            animator.SetBool("IsGrappling", false);
			canMove = true;
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
		if(coyoteTimeCounter > 0)
		{
            if (canJump)
            {
                if (onCubeTrigger.CurrentCube != null)
                {
                    if (onCubeTrigger.CurrentCube.Rb?.velocity.y > 0)
                    {
                        verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity) + (onCubeTrigger.CurrentCube.Rb.velocity.y) / 2;
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
        }
		else
		{
			Dive();
        }
	}

	public void Dive()
	{

        RaycastHit hit;

        var points = new Vector3[4] 
		{
			Vector3.forward,
			Vector3.right,
			Vector3.left,
           -Vector3.forward,
        };

        foreach (Vector3 point in points)
        {
            Vector3 position = transform.position + point; // Adjust point position relative to the object
            if (Physics.Raycast(position, Vector3.down, out hit))
            {

                if (hit.collider.gameObject.GetComponent<CubeBase>() != null)
                {

                    var cube = hit.collider.gameObject.GetComponent<CubeBase>();
                    var direction = characterController.transform.position - cube.transform.position;
                    var modifiedDirection = new Vector3(direction.x, 0, direction.z);
                    StartCoroutine(CorrectPositionToCubeCenter(cube.transform));
                    //characterController.Move(modifiedDirection);


                    var requieredPosition = new Vector3(cube.transform.position.x, transform.position.y, cube.transform.position.z);
                    //transform.position = requieredPosition;

                    Debug.Log("DiveTowardsCube");
                    animator.SetBool("IsDiving", true);
                    IsDivingDown = true;
					fallingDownFX.Play();
                    isGravityOn = true;
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
                    verticalVelocity = -verticalVelocity * 2;
                    coyoteTimeCounter = coyoteTime;
                    downVelocityMultiplier = 1f;
                    jumpBufferCounter = jumpBufferTime;
                    return;
                }
            }
        }

        Debug.Log("Dive down");
        animator.SetBool("IsDiving", true);
		IsDivingDown = true;
        fallingDownFX.Play();

        isGravityOn = true;
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * gravity);
        verticalVelocity = -verticalVelocity * 2;
        coyoteTimeCounter = coyoteTime;
        downVelocityMultiplier = 1f;
        jumpBufferCounter = jumpBufferTime;
    }

	private IEnumerator CorrectPositionToCubeCenter(Transform cubeTransform)
	{
		while (transform.position.x != cubeTransform.position.x && transform.position.z != cubeTransform.position.z)
		{
			if(IsGrounded)
			{
				break;
			}
			if(transform.position.y <= cubeTransform.position.y)
			{
				break;
			}
			transform.position = Vector3.Lerp(transform.position, new Vector3(cubeTransform.position.x, transform.position.y, cubeTransform.position.z), Time.deltaTime*5);
			yield return null;
		}
		yield return null;
    }


    private void HandleJumpBuffer()
	{
		jumpBufferCounter = jumpBufferTime;
	}

	private void DoGravity()
	{
		if (isGravityOn)
		{
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * downVelocityMultiplier * Time.deltaTime);
        }
	
	}

	private bool CheckIsGrounded()
	{
		bool result = Physics.CheckSphere(groundChecker.position, checkGroundRadius, groundLayer);
		return result;
	}

	private bool CheckCloseToGround()
	{
		bool result = Physics.CheckSphere(diveGroundChecker.position, checkGroundRadius, groundLayer);
		
		return result;
    }

	private void HandleVelocityWhenGrounded()
	{
		IsGrounded = CheckIsGrounded();

		if (IsGrounded && verticalVelocity < 0)
		{
			verticalVelocity = -2;
            //IsDivingDown = false;
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
		this.IsGrounded = isGrounded;
	}
	public void TriggerJump(float force)
	{
		canJump = true;
        verticalVelocity = force;
    }

	public void SetCanJump(bool canJump)
	{
		this.canJump = canJump;
	}

	public void StartDashCoroutine(Vector3 direction)
	{
		/*if(isGrounded)
		{
			return;
		}
		if(dashRoutine !=null)
		{
			StopCoroutine(dashRoutine);
            StartCoroutine(dashRoutine);
			Debug.Log("Dash routine wasnt null");
        }
		else
		{
            dashRoutine = DashRoutine(direction);
            StartCoroutine(dashRoutine);
            Debug.Log("Dash routine was null");
        }*/
    }
	
	public IEnumerator DashRoutine(Vector3 direction)
	{
		Debug.Log("Dash");
		var modifiedDirection = new Vector3(direction.x, 0, direction.y);
		var dashDistance = 15;
		var dashSpeed = 30f;
		var destination = transform.position + modifiedDirection * dashDistance;
		isGravityOn = false;

        for (int i = 0; i < dashDistance; i++)
		{
            characterController.Move(modifiedDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
		isGravityOn = true;
    }

	private void CancelDive()
	{
		IsDivingDown = false;
        fallingDownFX.Stop();
        animator.SetBool("IsDiving", false);
    }

	private void SetButtonDownTrue()
	{
		IsButtonDown = true;
	}
    private void SetButtonDownFalse()
    {
		IsButtonDown = false;
    }
}
