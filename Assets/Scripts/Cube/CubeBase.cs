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
	public IEnumerator CurrentRoutine { get; private set; }
	private float springSpeed = 9f;
	[SerializeField] protected LayerMask PlayerLayer;
	private bool isFollowing;
    private float cubeCoyoteTime;
    public float damping = 0.5f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private float downAmplitudeReducer = 4f;
    [SerializeField] private float pushForce = 30f;

    [SerializeField] private bool IsSetColor;
    [SerializeField] private Color setColor;
    [SerializeField] private ParticleSystem turnOffFX;
    protected Vector3 DefaultPosition;
    private bool isCollided;
    public bool IsInMovement { get; private set; }

    private void Awake()
	{
		cubeRenderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		Rb = GetComponent<Rigidbody>();

        if (IsSetColor)
        {
            CubeCurrentColor = setColor;
            SetColor(CubeCurrentColor);
        }
        else
        {
            CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
            SetColor(CubeCurrentColor);
        }
		
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

        if (!isFollowing)
        {
            cubeCoyoteTime -= Time.deltaTime;
        }

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponentInParent<PlayerMovement>() != null)
            {
                var player = collider.GetComponentInParent<PlayerMovement>();
                isFollowing = true;
                cubeCoyoteTime = 0.2f;
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
        /*if (getVelocityCooldown > 0)
        {
            Debug.Log("Velocity cooldown is not refreshed");
            return;
        }*/
        if (velocity.y > -10) //-5
		{
            Debug.Log("Velocity is too low");
			return;
		}
		
        if (CurrentRoutine == null)
		{
            var downPosition = transform.position + velocity / 4;
            var upPosition = transform.position - velocity / 4;
            CurrentRoutine = SpringMovement(downPosition, upPosition, playerMovement);
            StartCoroutine(CurrentRoutine);
        }
        else
        {
            var downPosition = DefaultPosition + velocity / 4;
            var upPosition = DefaultPosition - velocity / 4;
            StopCoroutine(CurrentRoutine);
            CurrentRoutine = SpringMovement(downPosition, upPosition, playerMovement);
            StartCoroutine(CurrentRoutine);
        }
	}

    public IEnumerator SpringMovement(Vector3 downPosition, Vector3 upPosition, PlayerMovement playerMovement)
    {
        IsInMovement = true;
        var amplitude = Vector3.Distance(upPosition, downPosition);

        Vector3 currentPosition = transform.position;
        while (Vector3.Distance(currentPosition, downPosition) > downAmplitudeReducer)
        {
            playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, currentPosition.y + 1, playerMovement.transform.position.z);
            currentPosition = Vector3.Lerp(currentPosition, downPosition, Time.deltaTime * springSpeed);

            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;

            if (isFollowing)
            {
                playerMovement.FollowTransform(Vector3.down * Time.deltaTime * springSpeed);
            }

            if (isCollided)
            {
                isCollided = false;
                break;
            }

            yield return null;
        }

        while (Vector3.Distance(currentPosition, upPosition) > 3f)
        {
            playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, currentPosition.y + 1, playerMovement.transform.position.z);
            currentPosition = Vector3.Lerp(currentPosition, upPosition, Time.deltaTime * springSpeed*2);
            // Apply damping to simulate spring effect
            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;

            if (isCollided)
            {
                isCollided = false;
                break;
            }

            yield return null;
        }

        var hasJumped = false;

        if (cubeCoyoteTime > 0 && amplitude > 4)
        {
            if (!InputManager.CheckIfButtonIsDown())
            {
                playerMovement.TriggerJump(pushForce);
                hasJumped = true;
            }
      
        }
        else if (cubeCoyoteTime > 0 && amplitude <= 4)
        {
            if (!InputManager.CheckIfButtonIsDown())
            {
                playerMovement.TriggerJump(pushForce / 2);
                hasJumped = true;
            }
        }

        while (Vector3.Distance(currentPosition, upPosition) > 1.5f)
        {
            if (!hasJumped)
            {
               playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, currentPosition.y + 1, playerMovement.transform.position.z);
            }

            currentPosition = Vector3.Lerp(currentPosition, upPosition, Time.deltaTime * springSpeed * 1.9f);
            // Apply damping to simulate spring effect
            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;

            if (isCollided)
            {
                isCollided = false;
                break;
            }

            IsInMovement = false;
            yield return null;
        }


        var deltaSpeed = springSpeed;
        while (deltaSpeed > 15)
        {
            if (!hasJumped && playerMovement.IsGrounded)
            {
                //playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, currentPosition.y + 1, playerMovement.transform.position.z);
                var direction = (DefaultPosition - transform.position).normalized;
                playerMovement.transform.position += direction * Time.deltaTime * deltaSpeed;
            }

            currentPosition = Vector3.Lerp(currentPosition, DefaultPosition, Time.deltaTime * deltaSpeed);
            // Apply damping to simulate spring effect
            currentPosition += (transform.position - currentPosition) * damping;
            transform.position = currentPosition;
            deltaSpeed -= Time.deltaTime;

            yield return null;
        }

        while(!Mathf.Approximately(transform.position.y, DefaultPosition.y))
        {
            if (!hasJumped && playerMovement.IsGrounded)
            {
                 //playerMovement.transform.position = new Vector3(playerMovement.transform.position.x, transform.position.y + 1, playerMovement.transform.position.z);
                var direction = (DefaultPosition - transform.position).normalized;
                playerMovement.transform.position += direction* Time.deltaTime * deltaSpeed;
            }
            transform.position = Vector3.MoveTowards(transform.position, DefaultPosition, Time.deltaTime * deltaSpeed);
            yield return null;
        }

        transform.position = DefaultPosition;
        CurrentRoutine = null;
        getVelocityCooldown = getVelocityCooldownRefresh;
        isCollided = false;
        yield return null;
	}

    public void TurnOffCube()
    {
        if(turnOffFX != null)
        {
            var main = turnOffFX.main;
            main.startColor = CubeCurrentColor;
            turnOffFX.Play();
        }

        meshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
    public void TurnOnCube()
    {
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }

    public void SetAmplitudeReducer(float value)
    {
        downAmplitudeReducer = value;
    }

    public void SetDefaultPosition(Vector3 position)
    {
        DefaultPosition = position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.GetComponent<NormalCube>())
        {
            isCollided = true;
        }
    }

}

public enum CubeType
{
	NormalCube,
	TurnOffCube,
	MovingCube,
}
