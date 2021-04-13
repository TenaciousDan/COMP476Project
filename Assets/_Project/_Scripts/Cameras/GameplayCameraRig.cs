using UnityEngine;

public class GameplayCameraRig : MonoBehaviour
{
    public float moveSpeed = 50;
    public float fastMoveSpeed = 150;
    public float rotationAmount = 50;
    public float zoomAmount = 100;
    public float smoothing = 5;
    public Camera rigCamera;
    public Transform target;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private void Awake()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        rigCamera.transform.rotation = Quaternion.LookRotation(transform.position - rigCamera.transform.position);
        newZoom = rigCamera.transform.localPosition;
    }

    private void Update()
    {
        if (target != null)
        {
            if (!Mathf.Approximately(Input.GetAxisRaw("Vertical"), 0) || !Mathf.Approximately(Input.GetAxisRaw("Horizontal"), 0))
                target = null;
            else
                newPosition = target.transform.position;
        }
        else
        {
            HandleTranslationInput();
        }

        HandleRotationInput();
        HandleZoomInput();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothing * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, smoothing * Time.deltaTime);
        rigCamera.transform.localPosition = Vector3.Lerp(rigCamera.transform.localPosition, newZoom, smoothing * Time.deltaTime);
    }

    private void HandleTranslationInput()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        newPosition += (transform.forward * vertical * speed * Time.deltaTime);
        newPosition += (transform.right * horizontal * speed * Time.deltaTime);
    }

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            newRotation *= Quaternion.Euler(Vector3.up * Input.GetAxis("Mouse X") * rotationAmount * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void HandleZoomInput()
    {
        newZoom += rigCamera.transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomAmount;
    }
}
