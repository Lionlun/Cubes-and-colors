
using UnityEngine;

public class SpawnCube : CubeBase
{
	private bool isAlreadyTriggered;
	[SerializeField] private LayerMask playerLayer;

	private void Update()
	{
		if (isAlreadyTriggered)
		{
			return;
		}

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

		foreach (Collider collider in hitColliders)
		{
			if (collider.GetComponentInParent<PlayerMovement>() != null)
			{
				isAlreadyTriggered = true;
				GlobalEvents.SendOnSpawnCubePressed();
			}
		}
	}
}
