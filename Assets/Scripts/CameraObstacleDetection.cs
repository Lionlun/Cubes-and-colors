using UnityEngine;

public class CameraObstacleDetection : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    private Vector3 playerOffset = new Vector3(0, 1, 0);

	private void Update()
	{
        Detect();
	}
	private void Detect()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position + playerOffset - transform.position;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null) 
                {
                    return;
                }

                if (hit.collider.gameObject.GetComponent<PlayerMovement>() != null)
                {
		        
				}
                else if(hit.collider.gameObject.GetComponent<CubeBase>() != null)
                {
                    var cubeColor = hit.collider.gameObject.GetComponent<CubeBase>();
					cubeColor.GoTransparent();
                }

            }
        }
    }
}
