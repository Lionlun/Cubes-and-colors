using UnityEngine;

public class CubeBase : MonoBehaviour
{
	public Color CubeCurrentColor { get; set; }
	private Renderer cubeRenderer;
	protected float TransparentTimer = 0.1f;
	[field:SerializeField] public CubeType CubeType { get; set; }
	protected Rigidbody Rb;
	private float getVelocityCooldown;
	private float getVelocityCooldownRefresh = 0.5f;


	private void Awake()
	{
		cubeRenderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		Rb = GetComponent<Rigidbody>();
		CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
		SetColor(CubeCurrentColor);
        getVelocityCooldown = getVelocityCooldownRefresh;
    }

	private void Update()
	{
		if (TransparentTimer < 0)
		{
			GoOpaque();
		}
		TransparentTimer -= Time.deltaTime;

		if(getVelocityCooldown > 0)
		{
			getVelocityCooldown -= Time.deltaTime;
		}

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
        if (getVelocityCooldown > 0)
        {
			return;
        }
        if (velocity.y < 0)
		{
			Debug.Log("received velocity is " +  velocity);
			var additionalVelocity = (velocity.y - Rb.velocity.y)*0.7f;
            Rb.AddForce(new Vector3(0, additionalVelocity, 0), ForceMode.Impulse);
			getVelocityCooldown = getVelocityCooldownRefresh;
        }
	}
}

public enum CubeType
{
	NormalCube,
	TurnOffCube,
	MovingCube,
}
