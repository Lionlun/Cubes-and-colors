using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
	public bool IsDetected { get; set; }
	public Vector3 Position { get; set; }

	[SerializeField] private float radius;
	[SerializeField] private LayerMask mask;

	private bool canDetect = true;
	private float detectTime = 0.1f;
	private float detectTimer;


	private void Update()
	{
		if (canDetect)
		{
			DetectLedge();
		}
		HandleDetectTimer();
		if (detectTimer < 0) 
		{
			IsDetected = false;
		}
	}

	private  void DetectLedge()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, mask);
		foreach (Collider collider in hitColliders)
		{
			if (collider.gameObject.GetComponent<CubeBase>() != null)
			{
				detectTimer = detectTime;
				IsDetected = true;
				Position = collider.transform.position;
			}
		}
	}

	private void HandleDetectTimer()
	{
		detectTimer -= Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<CubeBase>() != null)
		{
			canDetect = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<CubeBase>() != null)
		{
			canDetect = true;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
