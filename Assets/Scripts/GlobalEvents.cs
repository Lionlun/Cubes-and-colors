using System;
using UnityEngine;

public static class GlobalEvents
{
	public static event Action<AudioClip> OnSoundTriggered;
	public static event Action<AudioClip, float> OnPitchedSoundTriggered;
	public static event Action OnSpawnCubePressed;

	public static void SendOnSoundTriggered(AudioClip clip)
	{
		OnSoundTriggered?.Invoke(clip);
	}

	public static void SendOnPitchedSoundTriggered(AudioClip clip, float pitch)
	{
		OnPitchedSoundTriggered?.Invoke(clip, pitch);
	}

	public static void SendOnSpawnCubePressed()
	{
		OnSpawnCubePressed?.Invoke();
	}

}
