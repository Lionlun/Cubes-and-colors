using System.Collections;
using UnityEngine;

public class MovingCube : CubeBase
{
	private Vector3 initialPosition;
	private Vector3 endPostion;
	private float speed = 2.5f;
	private float travelDistance = 5;
	private bool isMovingForward;
	private Vector3 direction;
	private Vector3 destination;

	private void Start()
	{
		initialPosition = transform.position;
		endPostion = transform.position + transform.forward*travelDistance;
		destination = endPostion;
		isMovingForward = true;
        CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
		SetColor(CubeCurrentColor);
	}

	private void Update()
	{
        Move();
		SetDirection();
		

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, PlayerLayer);

		foreach (Collider collider in hitColliders)
		{
			if (collider.GetComponentInParent<PlayerMovement>() != null)
			{
				var player = collider.GetComponentInParent<PlayerMovement>();
				player.Follow(direction * speed * Time.deltaTime);
			}
		}

        if (TransparentTimer < 0)
        {
            GoOpaque();
        }
        TransparentTimer -= Time.deltaTime;

    }

	private void Move()
	{
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
		
	}
	private void SetDirection()
	{
        if (transform.position == destination)
        {
            if (destination == endPostion)
            {
                destination = initialPosition;
            }
            else
            {
                destination = endPostion;
            }
        }

        direction = (destination - transform.position).normalized;
    }
}
