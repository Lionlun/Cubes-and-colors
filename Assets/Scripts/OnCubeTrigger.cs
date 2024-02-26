using UnityEngine;

public class OnCubeTrigger : MonoBehaviour
{
	private bool isTriggered;
	private float triggerDistance = 1.5f;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private AudioClip onCubeJumpSound;
	private int lastId;
	private float comboPitch = 1;
	public int comboCounter { get; private set; } = 0;
	
	private float comboTimer;
	private float comboTime = 1.2f;

	private CharacterController characterController;
	private PlayerMovement playerMovement;
	public CubeBase CurrentCube { get; private set; }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();	
    }

    private void Update()
	{
		DetectOnCubeJump();
		HandleTimer();
	}

	private void DetectOnCubeJump()
	{
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;

        Vector3 p1 = transform.position + characterController.center;

        if (Physics.SphereCast(p1, characterController.height/2, Vector3.down, out hit, 0.5f))
        {
            CurrentCube = hit.transform.GetComponent<CubeBase>();

            if (characterController.velocity.y < -1)
            {
                CurrentCube?.GetVelocity(characterController.velocity, playerMovement);
            }

            if (isTriggered || lastId == hit.transform.GetInstanceID())
            {
                return;
            }

            lastId = hit.transform.GetInstanceID();
            isTriggered = true;
            playerMovement.SetIsGrounded(true);
            GlobalEvents.SendOnPitchedSoundTriggered(onCubeJumpSound, comboPitch);
            comboPitch += 0.05f;
            comboCounter++;
            comboTimer = comboTime;
        }
        else
        {
            isTriggered = false;
        }

		/*
		if (Physics.Raycast(ray, out hit, triggerDistance, groundLayer))
		{
            if (characterController.velocity.y < -1)
            {
                hit.transform.GetComponent<CubeBase>()?.GetVelocity(characterController.velocity);
                Debug.Log("Transfered velocity is " + characterController.velocity);
            }

            if (isTriggered || lastId == hit.transform.GetInstanceID())
			{
				return;
			}

			lastId = hit.transform.GetInstanceID();
			isTriggered = true;
			GlobalEvents.SendOnPitchedSoundTriggered(onCubeJumpSound , comboPitch);
			comboPitch+=0.05f;
			comboCounter++;
			comboTimer = comboTime;
		}
		else
		{
			isTriggered = false;
		}*/
	}

	private void HandleTimer()
	{
		if(comboTimer > 0)
		{
			comboTimer -= Time.deltaTime;
		}
		else
		{
			comboPitch = 1;
			comboCounter = 0;
		}
	}
}
