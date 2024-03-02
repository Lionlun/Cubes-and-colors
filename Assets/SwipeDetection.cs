using System;
using System.Collections;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private float minimumDistance = 0.2f;
    [SerializeField] private float maximumTime = 1f;
    [SerializeField, Range(0,1f)] private float directionThreshold = 0.5f;


    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endtime;
    public static Action<Vector3> OnSwipeDetected;

    private void OnEnable()
    {
        InputManager.OnStartSwipe += SwipeStart;
        InputManager.OnEndSwipe += SwipeEnd;

    }
    private void OnDisable()
    {
        InputManager.OnStartSwipe -= SwipeStart;
        InputManager.OnEndSwipe -= SwipeEnd;
    }

    public void SwipeStart(Vector2 position, float startTime)
    {
        startPosition = position;
        this.startTime = startTime;
    }
    public void SwipeEnd(Vector2 position, float endTime)
    {
        endPosition = position;
        this.endtime = endTime;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endtime - startTime) <= maximumTime)
        {
            Vector3 direction = endPosition - startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
        } 
    }

    public void SwipeDirection(Vector2 direction)
    {
        OnSwipeDetected?.Invoke(direction);

        Debug.Log("Try to detect swipe direction");
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {

            Debug.Log("Swipe Up");
        }
        if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("Swipe Down");
        }
        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            Debug.Log("Swipe Left");
        }
        if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            Debug.Log("Swipe Right");
        }
    }
}
