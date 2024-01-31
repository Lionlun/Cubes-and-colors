using UnityEngine;

public class CubeBase : MonoBehaviour
{
	public Color CubeCurrentColor { get; set; }
	private Renderer cubeRenderer;
	protected float TransparentTimer = 0.1f;
	[field:SerializeField] public CubeType CubeType { get; set; }
	protected Rigidbody Rb;


	private void Awake()
	{
		cubeRenderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
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

	public void GetVelocity(Vector3 velocity)
	{
		if(velocity.y < 0)
		{
			Debug.Log("received velocity is " +  velocity);
            Rb.AddForce(new Vector3(0, velocity.y, 0), ForceMode.Impulse);
        }
	}
}

public enum CubeType
{
	NormalCube,
	TurnOffCube,
	MovingCube,
}
