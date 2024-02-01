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

	CharacterController characterController;

    private void Start()
    {
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

		if (Physics.Raycast(ray, out hit, triggerDistance, groundLayer))
		{
            if (characterController.velocity.y < -1)
            {
                hit.transform.GetComponent<CubeBase>().GetVelocity(characterController.velocity);
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
		}
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
