using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static event Action OnTouchStarted = delegate { };
	public static event Action OnTouchEnded = delegate { };
	private TouchControls touchControls;
	[SerializeField] private UITouchChecker uIChecker;

	private void Awake()
	{
		touchControls = new TouchControls();
	}

	private void OnEnable()
	{
		touchControls.Enable();
		touchControls.Touch.FirstTouchPress.started += ctx => StartTouch(ctx);
		touchControls.Touch.SecondTouch.started += ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled += ctx => EndTouch(ctx);
		touchControls.Touch.SecondTouch.canceled += ctx => EndTouch(ctx);
	}

	private void OnDisable()
	{
		touchControls.Disable();
		touchControls.Touch.FirstTouchPress.started -= ctx => StartTouch(ctx);
		touchControls.Touch.SecondTouch.started -= ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled -= ctx => EndTouch(ctx);
		touchControls.Touch.SecondTouch.canceled -= ctx => EndTouch(ctx);
	}

	private async void StartTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later
		
		if (!uIChecker.IsPointerOverUI())
		{
			OnTouchStarted?.Invoke();
		}
	}
	private async void SecondTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later
		OnTouchStarted?.Invoke();
	}

	private void EndTouch(InputAction.CallbackContext context)
	{
		OnTouchEnded?.Invoke();
	}

	private void CheckUI()
	{
		
	}
}
