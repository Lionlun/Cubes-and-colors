using TMPro;
using UnityEngine;

public class HeightUI : MonoBehaviour
{
	private TextMeshProUGUI text;
	[SerializeField] HeightMeasurement HeightMeasurement;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		text.text = $"{HeightMeasurement.Height} m";
	}
}
