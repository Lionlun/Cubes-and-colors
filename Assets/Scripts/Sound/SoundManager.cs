using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[SerializeField] private AudioSource musicSource, effectsSource;

	private void OnEnable()
	{
		GlobalEvents.OnSoundTriggered += PlaySound;
		GlobalEvents.OnPitchedSoundTriggered += PlayPitchedSound;
	}

	private void OnDisable()
	{
		GlobalEvents.OnSoundTriggered -= PlaySound;
		GlobalEvents.OnPitchedSoundTriggered -= PlayPitchedSound;
	}

	private void PlaySound(AudioClip clip)
	{
		effectsSource.PlayOneShot(clip);
	}

	public void PlayPitchedSound(AudioClip clip, float pitch)
	{
		effectsSource.pitch = pitch;
		effectsSource.PlayOneShot(clip);
	}

	public void ChangeMasterVolume(float value)
	{
		AudioListener.volume = value;
	}

	

	public void ToggleEffects()
	{
		effectsSource.mute = !effectsSource.mute;
	}

	public void ToggleMusic()
	{
		musicSource.mute = !musicSource.mute;
	}

	public void SendOnCubeJump()
	{
	
	}
}
