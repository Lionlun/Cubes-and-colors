using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
	public static List<CubeBase> Cubes = new List<CubeBase>();

	[SerializeField] private CubeBase cube;
	[SerializeField] private SpawnCube spawnCube;
	[SerializeField] private MovingCube movingCube;
	[SerializeField] private TurnOffCube turnOffCube;
	[SerializeField] private Floor floor;
	[SerializeField] private CubeBase stairsCube;
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

			var random = Random.Range(0, 101);

			if(random <= 70)
			{
                var newNormalCube = Instantiate(cube, newPosition, Quaternion.identity);
                lastPosition = newNormalCube.transform.position;
                Cubes.Add(newNormalCube);
            }
			if(random >70 && random <= 80)
			{
                var newTurnOffCube = Instantiate(turnOffCube, newPosition, Quaternion.identity);
                lastPosition = newTurnOffCube.transform.position;
                Cubes.Add(newTurnOffCube);
            }
			if (random > 80 && random <= 90)
			{
                var newMovingCube = Instantiate(movingCube, newPosition, Quaternion.identity);
                lastPosition = newMovingCube.transform.position;
                Cubes.Add(newMovingCube);
            }
			if(random >90 && random <= 100)
			{
				for(int j = 0; j < 4; j++)
                {
                    var newStairsCube = Instantiate(stairsCube, newPosition, Quaternion.identity);
                    lastPosition = newStairsCube.transform.position;
					newStairsCube.SetAmplitudeReducer(10);
                    Cubes.Add(newStairsCube);
                    newPosition += new Vector3(2.5f, 2.5f, 0);
                }
            }
		}

		var spawnCubePosition = lastPosition + GetRandomOffset();
		var newSpawnCube = Instantiate(spawnCube, spawnCubePosition, Quaternion.identity);
		lastPosition = newSpawnCube.transform.position;
		var floor = Instantiate(this.floor, lastPosition + new Vector3(13, -2, 0), Quaternion.identity);
    }

	private Vector3 GetRandomOffset()
	{
		var randomX = Random.Range(-4,4);
		while (randomX == previousRandomX)
		{
			randomX = Random.Range(-4, 4);
		}
		previousRandomX = randomX;

		var randomY = Random.Range(6,8f);
		var randomZ = Random.Range(-4,4);
		return new Vector3(randomX,randomY,randomZ);
	}
}
