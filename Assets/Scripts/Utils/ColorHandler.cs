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
				return new Color(1.0f, 0.50f, 0.0f, 1);

			default:
				return Color.white;
		}
	}
}
