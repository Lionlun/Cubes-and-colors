using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCube : CubeBase
{
    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, PlayerLayer);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponentInParent<PlayerMovement>() != null)
            {
                var player = collider.GetComponentInParent<PlayerMovement>();
                player.Follow(Rb.velocity*Time.fixedDeltaTime);
            }

        }
    }
}
