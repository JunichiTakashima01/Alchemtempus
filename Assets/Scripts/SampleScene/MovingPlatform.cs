using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    public Vector3 nextPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextPosition = pointA.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if (this.transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointB.position) ? pointA.position : pointB.position;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(collider.gameObject.tag);
        if (collider.gameObject.CompareTag("PlayerGroundCheck"))
        {
            collider.gameObject.transform.parent.parent = this.transform;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("PlayerGroundCheck"))
        {
            collider.gameObject.transform.parent.parent = null;
        }
    }
}
