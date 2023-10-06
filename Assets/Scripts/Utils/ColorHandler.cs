using UnityEngine;
using Color = UnityEngine.Color;

public static class ColorHandler
{
	public static Color GetColor(this CubeColor cubeColor)
	{
		switch (cubeColor)
		{
			case CubeColor.Yellow:
				return Color.yellow;

			case CubeColor.Green:
				return Color.green;

			case CubeColor.Blue:
				return Color.blue;

			case CubeColor.Violet:
				return Color.magenta;

			case CubeColor.Red:
				return Color.red;

			case CubeColor.Orange:
				return new Color32(255, 127, 80, 1);

			default:
				return Color.white;
		}
	}
}
