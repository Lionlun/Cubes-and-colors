using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    private Vector3 cameraOffset = new Vector3(0 , 3, -7);
	private float cameraSpeed = 80f;
	private float modifiedSpeed;

	private void Update()
	{
		Follow();
	}

	private void Follow()
    {
		if (player.GetVerticalVelocity() < -10) 
		{
			modifiedSpeed = cameraSpeed * 10;
		}
        else
        {
			modifiedSpeed = cameraSpeed;
        }

        transform.position = Vector3.MoveTowards(transform.position, player.transform.position + cameraOffset, modifiedSpeed * Time.deltaTime);
	}
}
