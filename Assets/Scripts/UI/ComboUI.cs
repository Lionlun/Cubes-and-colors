using TMPro;
using UnityEngine;

public class ComboUI : MonoBehaviour
{
	private TextMeshProUGUI text;
	[SerializeField] private OnCubeTrigger comboCount;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (comboCount.comboCounter <= 1)
		{
			text.text = "";
		}
		else
		{
			text.text = $"X{comboCount.comboCounter}";
		}
	}
}
