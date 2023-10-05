using System;

public static class RandomEnum
{
	public static T GetRandomEnum<T>()
	{
		var values = Enum.GetValues(typeof(T));
		int random = UnityEngine.Random.Range(0, values.Length);
		return (T)values.GetValue(random);
	}
}

