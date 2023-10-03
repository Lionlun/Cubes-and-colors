using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UITouchChecker : MonoBehaviour
{
	private GraphicRaycaster graphicRaycaster;

	private void Start()
	{
		graphicRaycaster = GetComponent<GraphicRaycaster>();
	}

	public bool IsPointerOverUI()
	{
		var pointerPosition = Touchscreen.current.position.ReadValue();

		var pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = pointerPosition;

		var results = new List<RaycastResult>();
		graphicRaycaster.Raycast(pointerEventData, results);
		return results.Count > 0;
	}
}
