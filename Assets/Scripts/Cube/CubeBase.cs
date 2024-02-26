using System.Collections;
using TMPro;
using UnityEngine;

public class CubeBase : MonoBehaviour
{
	public Color CubeCurrentColor { get; set; }
	private Renderer cubeRenderer;
	protected float TransparentTimer = 0.1f;
	[field:SerializeField] public CubeType CubeType { get; set; }
	public Rigidbody Rb { get; private set; }
	private float getVelocityCooldown;
	private float getVelocityCooldownRefresh = 0.5f;
	private IEnumerator currentRoutine;
	private float springSpeed = 10f;
	[SerializeField] protected LayerMask PlayerLayer;
	private bool isFollowing;
    public float damping = 0.5f;



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
    private void LateUpdate()
    {
		isFollowing = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, PlayerLayer);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponentInParent<PlayerMovement>() != null)
            {
                var player = collider.GetComponentInParent<PlayerMovement>();
                isFollowing = true;
            }
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

	public void GetVelocity(Vector3 velocity, PlayerMovement playerMovement)
	{
	Debug.Log("Velocity y is  " +  velocity.y);
        if (getVelocityCooldown > 0)
        {
            return;
        }
        if (velocity.y > -5)
		{
			return;
		}
		var downPosition = transform.position + velocity/4;
		var upPosition  = transform.position - velocity /4;
        Debug.Log("Try to spring movement");
        if (currentRoutine == null)
		{
            currentRoutine = SpringMovement(downPosition, upPosition, playerMovement);
            StartCoroutine(currentRoutine);
        }
	
        /*
        if (velocity.y < 0)
		{
			Debug.Log("Velocity y is " +  velocity.y);	
			Debug.Log("received velocity is " +  velocity);
			var additionalVelocity = (velocity.y - Rb.velocity.y)*0.7f;
            Rb.AddForce(new Vector3(0, additionalVelocity, 0), ForceMode.Impulse);
			getVelocityCooldown = getVelocityCooldownRefresh;
        }*/
	}

	public IEnumerator SpringMovement(Vector3 downPosition, Vector3 upPosition, PlayerMovement playerMovement)
	{
        Debug.Log("Spring movement started");
		var defaultPosition = transform.position;


        Vector3 currentPosition = transform.position;
        while (Vector3.Distance(currentPosition, downPosition) > 1f)
        {
            currentPosition = Vector3.Lerp(currentPosition, downPosition, Time.deltaTime * springSpeed);
            // Apply damping to simulate spring effect

            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;

            if (isFollowing)
            {
                playerMovement.Follow(Vector3.down * Time.deltaTime * springSpeed);
            }

            yield return null;
        }
        while (Vector3.Distance(currentPosition, upPosition) > 1.5f)
        {
            currentPosition = Vector3.Lerp(currentPosition, upPosition, Time.deltaTime * springSpeed*2);
            // Apply damping to simulate spring effect
            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;

            if (isFollowing)
            {
                playerMovement.Follow(Vector3.down * Time.deltaTime * springSpeed*2);
            }


            yield return null;
        }

        var deltaSpeed = springSpeed;
        while (deltaSpeed > 10)
        {
            currentPosition = Vector3.Lerp(currentPosition, defaultPosition, Time.deltaTime * deltaSpeed);
            // Apply damping to simulate spring effect
            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;
            deltaSpeed -= Time.deltaTime;
            Debug.Log("DeltaSpeed is " + deltaSpeed);

            if (isFollowing)
            {
                playerMovement.Follow(Vector3.down * Time.deltaTime * deltaSpeed);
            }

            yield return null;
        }
        while(!Mathf.Approximately(transform.position.y, defaultPosition.y))
        {
            transform.position = Vector3.MoveTowards(transform.position, defaultPosition, Time.deltaTime * deltaSpeed);
            yield return null;
        }
        currentRoutine = null;
        getVelocityCooldown = getVelocityCooldownRefresh;
        yield return null;
	}



    /*public IEnumerator SpringMovement(Vector3 downPosition, Vector3 upPosition, PlayerMovement playerMovement)
    {

        Debug.Log("Spring movement started");
        var defaultPosition = transform.position;
        var deltaSpeed = springSpeed;
        while (transform.position.y > downPosition.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPosition, Time.deltaTime * deltaSpeed);

            if (isFollowing)
            {
                playerMovement.Follow(Vector3.down * Time.deltaTime * deltaSpeed);
            }
            if (deltaSpeed > 0.1f)
            {
                deltaSpeed -= Time.deltaTime * 10;
            }

            yield return null;
        }
        while (transform.position.y < upPosition.y)
        {
            deltaSpeed = springSpeed;
            transform.position = Vector3.MoveTowards(transform.position, upPosition, Time.deltaTime * deltaSpeed);

            if (isFollowing)
            {
                playerMovement.Follow(Vector3.up * Time.deltaTime * deltaSpeed);
            }
            if (deltaSpeed > 0.1f)
            {
                deltaSpeed -= Time.deltaTime * 10;
            }
            yield return null;
        }
        while (transform.position.y > defaultPosition.y)
        {
            deltaSpeed = springSpeed;
            transform.position = Vector3.MoveTowards(transform.position, defaultPosition, Time.deltaTime * deltaSpeed);
            if (isFollowing)
            {
                playerMovement.Follow(Vector3.down * Time.deltaTime * deltaSpeed);
            }
            if (deltaSpeed > 0.1f)
            {
                deltaSpeed -= Time.deltaTime * 10;
            }
            yield return null;
        }
        currentRoutine = null;
        getVelocityCooldown = getVelocityCooldownRefresh;
        yield return null;
    }*/

}

public enum CubeType
{
	NormalCube,
	TurnOffCube,
	MovingCube,
}
