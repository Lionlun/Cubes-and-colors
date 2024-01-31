using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
	private bool onWall;
	private bool onWallUp;
	private bool onWallDown;
	private bool onLedge;
	private bool isAlreadyMoved;

	[SerializeField] private float wallCheckRayDistance = 1f;
	[SerializeField] private float ledgeRayCorrectY = 0.5f;
	[SerializeField] private float offsetY;
	private float minCorrectionY = 0.01f;

	[SerializeField] private Transform wallCheckUp;
	[SerializeField] private Transform wallCheckDown;
	[SerializeField] private PlayerMovement player;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Animator animator;
	[SerializeField] private CharacterController characterController;
	public Collider CurrentCubeToClimb { get; private set; }

    private void FixedUpdate()
	{
		CheckLedge();
    }

	private void Climb()
	{
		if (onLedge)
		{
			animator.SetTrigger("Climb");
        }
	}

	private void CheckLedge()
	{
		onWallUp = Physics.Raycast(wallCheckUp.position, transform.forward, wallCheckRayDistance, groundLayer);

		if (onWallUp)
		{
			onLedge = !Physics.Raycast(wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0), transform.forward, wallCheckRayDistance, groundLayer);
			MoveTowardsLedge();
		}
		else
		{
			onLedge = false;
		}

		animator.SetBool("OnLedge", onLedge);

		if (onLedge)
		{	
			player.Stop();

			SetCurrentCubeToClimb();
           
            CalculateCorrectOffset();
            Climb();
        }
		else
		{
			//CurrentCubeToClimb = null;
        }
	}

    private void SetCurrentCubeToClimb()
	{
        Ray ray = new Ray(wallCheckUp.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, wallCheckRayDistance, groundLayer))
        {
            CurrentCubeToClimb = hit.transform.GetComponent<Collider>();
            if (CurrentCubeToClimb != null)
            {
                Physics.IgnoreCollision(characterController, CurrentCubeToClimb);
            }
        }
    }

    private void MoveTowardsLedge()
	{
		if (isAlreadyMoved)
		{
			return;
		}
		isAlreadyMoved = true;
		Ray ray = new Ray(wallCheckUp.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, wallCheckRayDistance, groundLayer))
		{
			CurrentCubeToClimb = hit.transform.GetComponent<Collider>();
            var direction = hit.point - player.transform.position;
			player.transform.position += new Vector3(direction.x, 0, direction.z).normalized * hit.distance/1.5f;

            if (CurrentCubeToClimb != null)
            {
                Physics.IgnoreCollision(characterController, CurrentCubeToClimb);
            }
        }
	}

	private  void CalculateCorrectOffset()
	{
		Ray ray = new Ray(wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0) + transform.forward * wallCheckRayDistance, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, ledgeRayCorrectY, groundLayer))
		{
			offsetY = hit.distance;

			if(offsetY > minCorrectionY * 1.5f)
			{
				player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - offsetY + minCorrectionY, player.transform.position.z);
				isAlreadyMoved = false;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(wallCheckUp.position, wallCheckUp.position + wallCheckUp.forward * wallCheckRayDistance);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0),
			wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0) + wallCheckUp.forward * wallCheckRayDistance);
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine(wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0) + transform.forward * wallCheckRayDistance,
			wallCheckUp.position + new Vector3(0, ledgeRayCorrectY, 0) + transform.forward * wallCheckRayDistance + Vector3.down * ledgeRayCorrectY);
	}
}
