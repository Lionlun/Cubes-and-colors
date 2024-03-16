using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CubeController : MonoBehaviour
{
	[SerializeField] private CountdownUI timerText;

	private int timer;
	private int timeToDisappear = 10;
	private CancellationTokenSource tokenSource = null;
	private IEnumerator activationRoutine;

	private void Start()
	{
		timer = timeToDisappear;

		StartCubeDeactivation();
	}

	private void StartCubeDeactivation()
	{
		tokenSource = new CancellationTokenSource();
		var token = tokenSource.Token;

		DeactivateCubes(token);
	}

	private async void DeactivateCubes(CancellationToken token)
	{
		await Task.Delay(10000);
		if (token.IsCancellationRequested)
		{
			tokenSource.Dispose();
			return;
		}

		var cubes = CubeSpawner.Cubes;
		var randomColorEnum = RandomEnum.GetRandomEnum<CubeColor>();
		var randomColor = randomColorEnum.GetColor();
		HandleTimer(randomColorEnum.ToString(), randomColor, token);

		await Task.Delay(10000);
		if (token.IsCancellationRequested)
		{
			tokenSource.Dispose();
			return;
		}

		foreach (var cube in cubes)
		{
			if (cube.CubeCurrentColor != randomColor)
			{
				cube.gameObject.SetActive(false);
			}
		}

		await Task.Delay(3000);
		if (token.IsCancellationRequested)
		{
			tokenSource.Dispose();
			return;
		}

		foreach (var cube in cubes)
		{
			if (cube.CubeCurrentColor != randomColor)
			{
				cube.gameObject.SetActive(true);
			}
		}
		await Task.Delay(3000);
		if (token.IsCancellationRequested)
		{
			tokenSource.Dispose();
			return;
		}

		StartCubeDeactivation();
	}

	private async void HandleTimer(string colorText, Color32 color, CancellationToken token)
	{
		timerText.Activate();
		timerText.SetColor(color);

		while (timer > 0)
		{
			if(token.IsCancellationRequested)
			{
				tokenSource.Dispose();
				return;
			}

			timer -= 1;
			timerText.UpdateText(timer, colorText);
			await Task.Delay(1000);
		}
		timer = timeToDisappear;
		timerText.Deactivate();
	}

	public void DeactivateOtherCubes()
	{
        if (activationRoutine != null)
        {
            StopCoroutine(activationRoutine);
            activationRoutine = null;
        }

		var cubes = CubeSpawner.Cubes;
		foreach (var cube in cubes)
		{
			if (cube.CubeType == CubeType.TurnOffCube)
			{
				continue;
			}
			cube.TurnOffCube();
		}
	}

	public void StartCubeActivationRoutine()
	{
        activationRoutine = ActivationRoutine();
        StartCoroutine(activationRoutine);
	}

	private IEnumerator ActivationRoutine()
	{
       yield return new WaitForSeconds(0.7f);
        var cubes = CubeSpawner.Cubes;
        foreach (var cube in cubes)
        {
            if (cube.CubeType == CubeType.TurnOffCube)
            {
                continue;
            }
            cube.TurnOnCube();
        }
    }


	private void OnApplicationQuit()
	{
		if (tokenSource != null)
		{
			tokenSource.Cancel();
		}
	}
}
