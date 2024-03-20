using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{


	[SerializeField] private float wallCheckRayDistance = 1f;
	[SerializeField] private float ledgeRayCorrectY = 0.5f;
	[SerializeField] private float offsetY;

	[SerializeField] private Transform wallCheckUp;
	[SerializeField] private Transform wallCheckDown;
	[SerializeField] private PlayerMovement player;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Animator animator;
	[SerializeField] private CharacterController characterController;
	private float grapCooldown = 0.5f;
	private float grapCooldownRefresh = 0.5f;

    private void FixedUpdate()
    {
        GrabLedge();
    }
    private void Update()
    {
       
        if (grapCooldown > 0)
        {
            grapCooldown -= Time.deltaTime;
        }
    }
    private void GrabLedge()
	{
		if (!player.IsHanging && grapCooldown <=0)
		{
			RaycastHit hit;
            if(!Physics.Raycast(wallCheckUp.position, transform.forward, 2f, groundLayer) && Physics.Raycast(wallCheckDown.position, transform.forward, out hit, 2f, groundLayer))
			{
                if (hit.collider.GetComponent<CubeBase>() != null)
                {
                    var cube = hit.collider.GetComponent<CubeBase>();
                    if (cube.IsInMovement)
                    {
                        //player.transform.position = cube.transform.position - player.transform.forward * 2;
                        ResetGrapCooldown();
                        return;
                    }
                }

                player.IsHanging = true;

                Vector3 hangPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                Vector3 offset = transform.forward * -0.5f + transform.up * -0.8f;
                hangPos += offset;
                player.transform.position = hangPos;
                transform.forward = -hit.normal;
                player.DisableGravity();
                player.Stop();
                animator.SetBool("OnLedge", true);


                //player.LedgeJump();
            }
        
        }
    }

	public void ResetGrapCooldown()
	{
		grapCooldown = grapCooldownRefresh;

    }
}
