using UnityEngine;

public class CubeBase : MonoBehaviour
{
	public Color CubeCurrentColor { get; set; }
	private Renderer cubeRenderer;
	protected float TransparentTimer = 0.1f;
	[field:SerializeField] public CubeType CubeType { get; set; }


	private void Awake()
	{
		cubeRenderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
		SetColor(CubeCurrentColor);
	}

	private void Update()
	{
		if (TransparentTimer < 0)
		{
			GoOpaque();
		}
		TransparentTimer -= Time.deltaTime;
	}

	protected void SetColor(Color color)
	{
		cubeRenderer.material.SetColor("_Color", color);
	}

	public void GoTransparent()
	{
		TransparentTimer = 0.5f;
		Material uniqueMaterial = cubeRenderer.material;
		uniqueMaterial.color = new Color(cubeRenderer.material.color.r, cubeRenderer.material.color.g, cubeRenderer.material.color.b, 0.1f);
	}

	public void GoOpaque()
	{
		Material uniqueMaterial = cubeRenderer.material;
		uniqueMaterial.color = new Color(cubeRenderer.material.color.r, cubeRenderer.material.color.g, cubeRenderer.material.color.b, 1f);
	}
}

public enum CubeType
{
	NormalCube,
	TurnOffCube,
	MovingCube,
}
