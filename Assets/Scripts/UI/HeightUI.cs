using TMPro;
using UnityEngine;

public class HeightUI : MonoBehaviour
{
	private TextMeshProUGUI text;
	[SerializeField] private HeightMeasurement heightMeasurement;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = $"{heightMeasurement.Height} m";
	}
}
