using UnityEngine;

public class ShadowCaster : MonoBehaviour
{
	private Vector3 hitPosition = Vector3.zero;
	[SerializeField] private GameObject shadowPrefab;
	private GameObject shadow;

	private void Start()
	{
		shadow = Instantiate (shadowPrefab, transform.position, Quaternion.identity);
	}

	private void Update()
	{
		shadow.transform.position = hitPosition;
		CastShadow();
	}
	private void CastShadow()
	{
		RaycastHit hit;

		if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hit, 4))
		{
			if (hit.collider.gameObject.GetComponent<CubeBase>() != null)
			{
				hitPosition = hit.point;
			}
			else
			{
				hitPosition = Vector3.zero;
			}
		}

	}
}
