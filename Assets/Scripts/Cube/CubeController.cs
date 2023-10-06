using System.Threading.Tasks;
using UnityEngine;

public class CubeController : MonoBehaviour
{
	[SerializeField] private CountdownUI timerText;

	private int timer;
	private int timeToDisappear = 10;

	private void Start()
	{
		timer = timeToDisappear;
		DeactivateCubes();
	}

	private async void DeactivateCubes()
	{
		await Task.Delay(5000);
		var cubes = FindObjectsOfType<CubeBase>();
		var randomColorEnum = RandomEnum.GetRandomEnum<CubeColor>();
		var randomColor = randomColorEnum.GetColor();

		HandleTimer(randomColorEnum.ToString(), randomColor);
		await Task.Delay(10000);

		foreach(var cube in cubes)
		{
			if (cube.CubeCurrentColor != randomColor)
			{
				cube.gameObject.SetActive(false);
			}
		}

		await Task.Delay(3000);

		foreach (var cube in cubes)
		{
			if (cube.CubeCurrentColor != randomColor)
			{
				cube.gameObject.SetActive(true);
			}
		}
		await Task.Delay(3000);
		DeactivateCubes();
	}

	private async void HandleTimer(string colorText, Color32 color)
	{
		timerText.Activate();
		timerText.SetColor(color);

		while (timer > 0)
		{
			timer -= 1;
			timerText.UpdateText(timer, colorText);
			await Task.Delay(1000);
		}
		timer = timeToDisappear;
		timerText.Deactivate();
	}
}
