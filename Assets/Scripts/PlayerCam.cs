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
    private float minFov;
    public float maxFov = 100f;

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

    private readonly float maxVelocity = 18f;
    void DynamicFOV()
    {
        float t = Mathf.SmoothStep(0, 1, rb.velocity.magnitude / maxVelocity);
        cam.fieldOfView = Mathf.Lerp(minFov, maxFov, t);
    }
}
