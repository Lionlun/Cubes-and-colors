using UnityEngine;

public class CubeBase : MonoBehaviour
{
	public Color CubeCurrentColor { get; private set; }
	private Renderer cubeRenderer;
	private Renderer cubeRender;
	private float transparentTimer = 0.1f;

	private void Start()
	{
		CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
		cubeRenderer = GetComponent<Renderer>();
		SetColor();
		cubeRender = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (transparentTimer < 0)
		{
			GoOpaque();
		}
		transparentTimer -= Time.deltaTime;
	}

	private void SetColor()
	{
		cubeRenderer.material.SetColor("_Color", CubeCurrentColor);
	}

	public void GoTransparent()
	{
		transparentTimer = 0.5f;
		Material uniqueMaterial = cubeRenderer.material;
		uniqueMaterial.color = new Color(cubeRender.material.color.r, cubeRender.material.color.g, cubeRender.material.color.b, 0.1f);
	}

	public void GoOpaque()
	{
		Material uniqueMaterial = cubeRenderer.material;
		uniqueMaterial.color = new Color(cubeRender.material.color.r, cubeRender.material.color.g, cubeRender.material.color.b, 1f);
	}
}
