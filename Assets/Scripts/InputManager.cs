using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static event Action OnTouchStarted = delegate { };
	public static event Action<Vector2, float> OnTouchEnded = delegate { };
	private TouchControls touchControls;
	[SerializeField] private UITouchChecker uIChecker;

	private void Awake()
	{
		touchControls = new TouchControls();
	}

	private void OnEnable()
	{
		touchControls.Enable();
		touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
		touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
	}

	private void OnDisable()
	{
		touchControls.Disable();
	}

	private async void StartTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later

		if (!uIChecker.IsPointerOverUI())
		{
			OnTouchStarted?.Invoke();
		}
	}

	private void EndTouch(InputAction.CallbackContext context)
	{
		OnTouchEnded?.Invoke(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
	}
}
