using UnityEngine;

public class HeightMeasurement : MonoBehaviour
{
	public int Height { get; private set; }

	[SerializeField] private Transform player;
	[SerializeField] private Transform groundPoint;

	private void Update()
	{
		MeasureHeight();
	}

	private void MeasureHeight()
	{
		transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.x);
		Height = (int)Vector3.Distance(transform.position, groundPoint.position) / 4;
	}
}
