using UnityEngine;

public class TurnOffCube : CubeBase
{
	[SerializeField] private LayerMask playerLayer;
	private CubeController cubeController;
	private bool isAlreadyTriggered;

	private void Start()
	{
		cubeController = FindObjectOfType<CubeController>();
	}

	private void FixedUpdate()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

		if (hitColliders.Length > 0 ) 
		{
			if (isAlreadyTriggered) 
			{
				return;
			}
			foreach (Collider collider in hitColliders)
			{
				if (collider.GetComponentInParent<PlayerMovement>() != null)
				{
					isAlreadyTriggered = true;
					cubeController.DeactivateOtherCubes();
				}
			}
		}
		else
		{
			if(isAlreadyTriggered)
			{
                cubeController.ActivateOtherCubes();
                isAlreadyTriggered = false;
            }
		}
	}
}
