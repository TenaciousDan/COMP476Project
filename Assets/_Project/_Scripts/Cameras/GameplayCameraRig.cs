using UnityEngine;

using Tenacious.Scenes;

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

    [SerializeField] private Vector3 cameraEndGamePosition;
    [SerializeField] private Vector3 cameraEndGameRotationEuler;
    [SerializeField, Min(0)] private float endGameTransitionTime = 5;
    private float endTransitionTimer;

    private bool IsMouseOverGameWindow 
    { 
        get { return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y); } 
    }

    private void Awake()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        rigCamera.transform.rotation = Quaternion.LookRotation(transform.position - rigCamera.transform.position);
        newZoom = rigCamera.transform.localPosition;
        endTransitionTimer = endGameTransitionTime;
    }

    private void Update()
    {
        if (!GameplayManager.Instance.gameIsOver)
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
        else
        {
            endTransitionTimer -= Time.deltaTime;
            if (endTransitionTimer <= 0)
            {
                SceneLoader.Instance.LoadScene("Credits", SceneLoader.RANDOM_TRANSITION);
            }
        }
    }

    private void LateUpdate()
    {
        if (!GameplayManager.Instance.gameIsOver)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothing * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, smoothing * Time.deltaTime);
            rigCamera.transform.localPosition = Vector3.Lerp(rigCamera.transform.localPosition, newZoom, smoothing * Time.deltaTime);
            rigCamera.transform.rotation = Quaternion.LookRotation(transform.position - rigCamera.transform.position);
        }
        else
        {
            rigCamera.transform.position = Vector3.Lerp(rigCamera.transform.position, cameraEndGamePosition, smoothing * Time.deltaTime);
            rigCamera.transform.rotation = Quaternion.Lerp(rigCamera.transform.rotation, Quaternion.Euler(cameraEndGameRotationEuler), smoothing * Time.deltaTime);
        }
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
        if (IsMouseOverGameWindow)
        {
            Vector3 zoom = (-rigCamera.transform.localPosition).normalized * Input.GetAxis("Mouse ScrollWheel") * zoomAmount;
            newZoom += zoom;
        }
    }

    public void SetPosition(Vector3 position)
    {
        newPosition = position;
    }
}
