using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class LedgeClimb : MonoBehaviour
{
	[SerializeField] private LedgeDetection ledgeDetection;
	private Vector3 offset1 = new Vector3 (0, 1, 1);
	private Vector3 offset2 = new Vector3 (0, 1, 1);
	private Vector3 climbStartPosition;
	private Vector3 climbEndPosition;
	private bool canClimb;
	private bool canGrabLedge = true;
	private PlayerMovement playerMovement;

	private void Start()
	{
		playerMovement = GetComponent<PlayerMovement>();
	}

	private void Update()
	{
		Climb();
	}

	private void Climb()
	{
		if (ledgeDetection.IsDetected && canGrabLedge)
		{
			canGrabLedge = false;
			Vector2 ledgePosition = ledgeDetection.transform.position;
			climbStartPosition = ledgeDetection.transform.position + offset1;
			climbEndPosition = ledgeDetection.transform.position + offset2;
			canClimb = true;
			
		}

		if (canClimb)
		{
			transform.position = climbStartPosition;
			EndClimb();
		}
	}

	private async void EndClimb()
	{
		canClimb = false;
		playerMovement.Stop();
		var moveDirection = ledgeDetection.Position + new Vector3(0, 0.5f, 0);
		transform.position = moveDirection;
		await Task.Delay(500);
		canGrabLedge = true;
		playerMovement.ContinueMovement();
	}
}
