using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static event Action OnTouchStarted = delegate { };
	public static event Action OnTouchEnded = delegate { };
	public static event Action<Vector2, float> OnStartSwipe;
    public static event Action<Vector2, float> OnEndSwipe;
    private TouchControls touchControls;
	[SerializeField] private UITouchChecker uIChecker;
	private Camera mainCamera;

	private void Awake()
	{
		touchControls = new TouchControls();
        mainCamera = Camera.main;

    }

	private void OnEnable()
	{
		touchControls.Enable();
		touchControls.Touch.FirstTouchPress.started += ctx => StartTouch(ctx);
		touchControls.Touch.SecondTouchPress.started += ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled += ctx => EndFirstTouch(ctx);
		touchControls.Touch.SecondTouchPress.canceled += ctx => EndSecondTouch(ctx);
	}

	private void OnDisable()
	{
		touchControls.Disable();
		touchControls.Touch.FirstTouchPress.started -= ctx => StartTouch(ctx);
		touchControls.Touch.SecondTouchPress.started -= ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled -= ctx => EndFirstTouch(ctx);
		touchControls.Touch.SecondTouchPosition.canceled -= ctx => EndSecondTouch(ctx);
	}

	private async void StartTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later
		
        OnStartSwipe?.Invoke(touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>(), (float)context.startTime);

        if (!uIChecker.IsPointerOverUI())
		{
			OnTouchStarted?.Invoke();
			Debug.Log("Touch started");
		}
	}
	private async void SecondTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later

        OnStartSwipe?.Invoke(touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>(), (float)context.startTime);

        if (touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>().x < Screen.width / 2)
		{
			return;
		}
        OnTouchStarted?.Invoke();
	
        
       // OnTouchStarted?.Invoke();
	}

	private void EndFirstTouch(InputAction.CallbackContext context)
	{
		OnTouchEnded?.Invoke();
        OnEndSwipe?.Invoke(touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>(), (float)context.time);
    }

	private void EndSecondTouch(InputAction.CallbackContext context)
	{
        OnEndSwipe?.Invoke(touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>(), (float)context.time);
    }

	public Vector2 PrimaryPosition()
	{
		return CoordinateConverter.ScreenToWorld(mainCamera, touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>());
	}
	private void CheckUI()
	{
		
	}
}
