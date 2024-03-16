using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HastyCube : CubeBase
{
    private Vector3 endPostion;
    private float speed = 30f;
    private float travelDistance = 6;
    private bool isMovingForward;
    private Vector3 direction;
    private Vector3 destination;
    private PlayerMovement playerMovement;

    private void Start()
    {
        SetDefaultPosition(transform.position);
        endPostion = transform.position + transform.forward * travelDistance;
        destination = endPostion;
        isMovingForward = true;
        CubeCurrentColor = RandomEnum.GetRandomEnum<CubeColor>().GetColor();
        SetColor(CubeCurrentColor);
        StartCoroutine(MovementRoutine());
    }

    private void FixedUpdate()
    {
        bool hasPlayer = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f, PlayerLayer);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponentInParent<PlayerMovement>() != null)
            {
                playerMovement = collider.GetComponentInParent<PlayerMovement>();
                hasPlayer = true;
            }
        }
        if(!hasPlayer)
        {
            playerMovement = null;
        }
    }

    private IEnumerator MovementRoutine()
    {
        while (transform.position.z != destination.z)
        {
            if (playerMovement!=null&& InputManager.CheckIfButtonIsDown())
            {
                //playerMovement.FollowTransform(direction * speed * Time.deltaTime);
                playerMovement.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            }
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        while (transform.position.z != DefaultPosition.z)
        {
            if (playerMovement != null && InputManager.CheckIfButtonIsDown())
            {
                //playerMovement.FollowTransform(direction * speed * Time.deltaTime);
                playerMovement.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            }
            transform.position = Vector3.MoveTowards(transform.position, DefaultPosition, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        StartCoroutine(MovementRoutine());

    }
}
