using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
	public static List<CubeBase> Cubes = new List<CubeBase>();

	[SerializeField] private CubeBase cube;
	[SerializeField] private SpawnCube spawnCube;
	[SerializeField] private MovingCube movingCube;
	private Vector3 lastPosition;
	private float previousRandomX = 0;
	[SerializeField] private HeightMeasurement heightMeasurement;


	private void OnEnable()
	{
		GlobalEvents.OnSpawnCubePressed += SpawnCubes;
	}
	private void OnDisable()
	{
		GlobalEvents.OnSpawnCubePressed -= SpawnCubes;
	}

	private void Start()
	{
		lastPosition = Vector3.zero;
		SpawnCubes();
	}

	private void SpawnCubes()
	{
		for (int i = 0; i < 100; i++) 
		{
			var newPosition = lastPosition + GetRandomOffset();

			if (i % 10 == 0)
			{
				var newMovingCube = Instantiate(movingCube, newPosition, Quaternion.identity);
				lastPosition = newMovingCube.transform.position;
				Cubes.Add(newMovingCube);

			}
			else
			{
				var newNormalCube = Instantiate(cube, newPosition, Quaternion.identity);
				lastPosition = newNormalCube.transform.position;
				Cubes.Add(newNormalCube);
			}
		}

		var spawnCubePosition = lastPosition + GetRandomOffset();
		var newSpawnCube = Instantiate(spawnCube, spawnCubePosition, Quaternion.identity);
		lastPosition = newSpawnCube.transform.position;
	}

	private Vector3 GetRandomOffset()
	{
		var randomX = Random.Range(-4,4);
		while (randomX == previousRandomX)
		{
			randomX = Random.Range(-4, 4);
		}
		previousRandomX = randomX;

		var randomY = Random.Range(4f,6);
		var randomZ = Random.Range(-4,4);
		return new Vector3(randomX,randomY,randomZ);
	}
}
