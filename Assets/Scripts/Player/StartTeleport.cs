using UnityEngine;

public class StartTeleport : MonoBehaviour
{
    private void FixedUpdate()
    {
		ReturnToInitialPosition();

	}

    private void ReturnToInitialPosition()
    {
		if (transform.position.y < -5)
		{
			transform.position = Vector3.zero;
		}
	}
}
