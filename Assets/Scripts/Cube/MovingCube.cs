using System.Collections;
using UnityEngine;

public class MovingCube : CubeBase
{
	private Vector3 direction;
	private Vector3 initialPosition;
	private float speed = 2.5f;
	[SerializeField] private LayerMask playerLayer;

	private void OnEnable()
	{
		//transform.position = initialPosition;
		StartCoroutine(ChangeDirection());
	}

	private void Start()
	{
		initialPosition = transform.position;
		CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
		SetColor(CubeCurrentColor);
	}

	private void Update()
	{
		Move();

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

		foreach (Collider collider in hitColliders)
		{
			if (collider.GetComponentInParent<PlayerMovement>() != null)
			{
				var player = collider.GetComponentInParent<PlayerMovement>();
				player.Follow(direction * speed * Time.deltaTime);
			}
		}
	}

	private IEnumerator ChangeDirection()
	{
		direction = transform.forward;
		yield return new WaitForSeconds(2);
		direction = -transform.forward;
		yield return new WaitForSeconds(2);
		StartCoroutine(ChangeDirection());
	}

	private void Move()
	{
		transform.position += direction * speed * Time.deltaTime;
	}
}
