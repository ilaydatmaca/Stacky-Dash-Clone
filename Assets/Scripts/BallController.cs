using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15f;

    private bool isTraveling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity);

        int i = 0;
        while (i < hitColliders.Length)
        {
            Stack pickup = hitColliders[i].transform.GetComponent<Stack>();
            if (pickup)
            {
                pickup.Remove();
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 0.3f, transform.localScale.z);
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero)
        {

            Vector3 pos = transform.position;
            pos.y = 0.5f;
            if (Vector3.Distance(pos, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }
        if (isTraveling)
            return;

        if (Input.GetMouseButton(0))

        {
            int X = (int)Input.mousePosition.x;
            int Y = (int)Input.mousePosition.y;

            swipePosCurrentFrame = new Vector2(X, Y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                // Make sure it was a legit swipe, not a tap
                {
                    return;
                }

                currentSwipe.Normalize();

                //Up/down

                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //Go up/down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    //Go Left/Right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            swipePosLastFrame = swipePosCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;

        }
    }

    private void SetDestination(Vector3 direction)
    {
        Vector3 pos = transform.position;
        pos.y = 0.5f;
        travelDirection = direction;

        RaycastHit hit;

        if (Physics.Raycast(pos, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
            Debug.Log(nextCollisionPosition);
        }

        isTraveling = true;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")

        {
            transform.localPosition = new Vector3(Mathf.Round(transform.localPosition.x), transform.localPosition.y, Mathf.Round(transform.localPosition.z));

        }

    }
}
