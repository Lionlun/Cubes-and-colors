using TMPro;
using UnityEngine;

public class CountdownUI: MonoBehaviour
{
    TextMeshProUGUI text;

	private void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		Deactivate();
	}

	public void UpdateText(float time, string colorName)
	{
		text.text = $"{colorName}: <br> {time}";
	}

	public void SetColor(Color32 color)
	{
		text.color = color;
	}

	public void Activate()
	{
		this.gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		this.gameObject.SetActive(false);
	}
}
