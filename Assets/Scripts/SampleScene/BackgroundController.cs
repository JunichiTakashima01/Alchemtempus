using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPositionX;
    private float length;

    public Camera cam;
    public float parallaxEffect; //the speed at which the background should move with the camera. 0 = not moving, 1 = move with camera
    public float verticalOffset = 0; //offset correspond to camera

    void Awake()
    {
        startPositionX = this.transform.position.x;
        length = this.GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(length);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float movementOfBackground = cam.transform.position.x * parallaxEffect;
        float distanceToCam = cam.transform.position.x - movementOfBackground;

        this.transform.position = new Vector2(startPositionX + movementOfBackground, cam.transform.position.y + verticalOffset);

        if (distanceToCam > startPositionX + length)
        {
            startPositionX += length;
        }
        else if (distanceToCam < startPositionX - length)
        {
            startPositionX -= length;
        }
    }
}
