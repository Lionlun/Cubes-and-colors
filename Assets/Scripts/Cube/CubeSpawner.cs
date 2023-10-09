using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
	[SerializeField] private CubeBase cube;
	private Vector3 lastPosition;
	private float previousRandomX = 0;

	private void Start()
	{
		lastPosition = Vector3.zero;
		SpawnCubes();
	}
	private void SpawnCubes()
	{
		for(int i = 0; i < 100; i++) 
		{
			var newPosition = lastPosition + GetRandomOffset();
			var newCube = Instantiate(cube, newPosition, Quaternion.identity);
			lastPosition = newCube.transform.position;
		}
	}

	private Vector3 GetRandomOffset()
	{
		var randomX = Random.Range(-4,4);
		while (randomX == previousRandomX)
		{
			randomX = Random.Range(-4, 4);
		}
		previousRandomX = randomX;

		var randomY = Random.Range(2,5);
		var randomZ = Random.Range(-4,4);
		return new Vector3(randomX,randomY,randomZ);
	}
}
