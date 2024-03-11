using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX = 400f;
    public float sensY = 400f;

    public Transform player;

    float xRotation;
    float yRotation;

    public Camera cam;
    public Rigidbody rb;
    public PlayerMovement playerMovement;

    private float minFov;
    public float maxFov = 90f;

    // Start is called before the first frame update
    void Start()
    {
        minFov = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        Rotation();
        DynamicFOV();
    }

    void Rotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        player.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void DynamicFOV()
    {
        if (!playerMovement.dashDone)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, minFov, 0.1f);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, maxFov, 0.1f);
        }
    }
}
