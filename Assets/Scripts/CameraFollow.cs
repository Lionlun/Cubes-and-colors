using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 cameraOffset = new Vector3(0 , 3, -7);

	private void Update()
	{
		Follow();
	}

	private void Follow()
    {
		transform.position = target.position + cameraOffset;
	}
}
