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

    public static event Action OnSecondStarted = delegate { };
    public static event Action OnSecondEnded = delegate { };

    public static event Action<Vector2, float> OnStartSwipe;
    public static event Action<Vector2, float> OnEndSwipe;
    private TouchControls touchControls;
	[SerializeField] private UITouchChecker uIChecker;
	private Camera mainCamera;
	private bool isSwipeStarted;

    private static bool IsHoldingFirstTouchDown;
    private static bool IsHoldingSecondTouchDown;


	private void Awake()
	{
		touchControls = new TouchControls();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Debug.Log("Is holding first is " + IsHoldingFirstTouchDown);
        Debug.Log("Is holding second is " + IsHoldingSecondTouchDown);
    }

    private void OnEnable()
	{
		touchControls.Enable();
		touchControls.Touch.FirstTouchPress.started += ctx => FirstTouch(ctx);
		touchControls.Touch.SecondTouchPress.started += ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled += ctx => EndFirstTouch(ctx);
		touchControls.Touch.SecondTouchPress.canceled += ctx => EndSecondTouch(ctx);
	}

	private void OnDisable()
	{
		touchControls.Disable();
		touchControls.Touch.FirstTouchPress.started -= ctx => FirstTouch(ctx);
		touchControls.Touch.SecondTouchPress.started -= ctx => SecondTouch(ctx);
		touchControls.Touch.FirstTouchPress.canceled -= ctx => EndFirstTouch(ctx);
		touchControls.Touch.SecondTouchPosition.canceled -= ctx => EndSecondTouch(ctx);
	}

	private async void FirstTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later
        

        if (touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>().x < Screen.width / 2)
        {
            OnStartSwipe?.Invoke(touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>(), (float)context.startTime);
            isSwipeStarted = true;
        }
		else
		{
            IsHoldingFirstTouchDown = true;
            OnTouchStarted?.Invoke();
            Debug.Log("Touch started");
        }
    }

    private void EndFirstTouch(InputAction.CallbackContext context)
    {
        OnTouchEnded?.Invoke();
        IsHoldingFirstTouchDown = false;
        if (isSwipeStarted)
        {
            if (touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>().x > Screen.width / 2)
            {
                return;
            }
            if (touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>().x > Screen.width / 2)
            {
                return;
            }
            Debug.Log("invoke On end swipe");
            OnEndSwipe?.Invoke(touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>(), (float)context.time);
            isSwipeStarted = false;
        }
    }

    private async void SecondTouch(InputAction.CallbackContext context)
	{
		await Task.Delay(10); //ToDo Implement the appropriate solution later
       
        OnSecondStarted?.Invoke();
        if (touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>().x < Screen.width / 2)
		{
            OnStartSwipe?.Invoke(touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>(), (float)context.startTime);
            isSwipeStarted = true;
        }
		else
		{
            IsHoldingSecondTouchDown = true;
            OnTouchStarted?.Invoke();
        }
  
        // OnTouchStarted?.Invoke();
    }

	private void EndSecondTouch(InputAction.CallbackContext context)
	{
        IsHoldingSecondTouchDown = false;
        OnSecondEnded?.Invoke();
        if (isSwipeStarted)
		{
            isSwipeStarted = false;
            if (touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>().x > Screen.width / 2)
            {
                return;
            }
			Debug.Log("End second touch");
            OnEndSwipe?.Invoke(touchControls.Touch.SecondTouchPosition.ReadValue<Vector2>(), (float)context.time);
            
        }
    
    }

	public Vector2 PrimaryPosition()
	{
		return CoordinateConverter.ScreenToWorld(mainCamera, touchControls.Touch.FirstTouchPosition.ReadValue<Vector2>());
	}
    public static bool CheckIfButtonIsDown()
    {
        if (IsHoldingFirstTouchDown || IsHoldingSecondTouchDown)
        {
            return true;
        }
        else 
        {
            return false; 
        }
    }
}
