using UnityEngine;

public class RoverController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform driverSeat; 
    [SerializeField] private Camera roverCamera; 
    [SerializeField] private AudioClip SFXEngine;
    
    [Header("Movimiento del Rover")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("UI")]
    [SerializeField] private GameObject enterPrompt; 
    
    private bool isPlayerInside = false;
    private GameObject player;
    private AstronautPlayer.AstronautPlayer playerController;
    private CharacterController characterController;
    private Camera playerCamera;
    
    private bool playerNearby = false;
    private Rigidbody rb;

    void Start()
    {
        if (enterPrompt) enterPrompt.SetActive(false);
        
        if (roverCamera) roverCamera.gameObject.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPlayerInside && playerNearby)
            {
                EnterRover();
            }
            else if (isPlayerInside)
            {
                ExitRover();
            }
        }

        if (isPlayerInside)
        {
            ControlRover();
        }
    }

    void ControlRover()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Rotación
        float turn = h * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, turn, 0f);

        Vector3 moveDirection = transform.forward * v * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        
        AlignToGround();
    }

    void AlignToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void EnterRover()
    {
        AudioManager.instance.PlayDrivingMusic();
        AudioManager.instance.PlaySFX(SFXEngine);

        if (player == null) return;

        isPlayerInside = true;

        // cámara  jugador
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = player.GetComponentInChildren<Camera>();
        }

        //  cámara  rover
        if (playerCamera) playerCamera.gameObject.SetActive(false);
        if (roverCamera) roverCamera.gameObject.SetActive(true);

        if (characterController) characterController.enabled = false;
        
        if (playerController) playerController.enabled = false;

        //  parentación
        player.transform.SetParent(transform);
        
        if (driverSeat)
        {
            player.transform.position = driverSeat.position;
            player.transform.rotation = driverSeat.rotation;
        }
        
        if (enterPrompt) enterPrompt.SetActive(false);

    }

    void ExitRover()
    {
        AudioManager.instance.PlayLevelMusic();
        AudioManager.instance.StopSFX(SFXEngine);

        if (player == null) return;

        isPlayerInside = false;

        if (roverCamera) roverCamera.gameObject.SetActive(false);
        if (playerCamera) playerCamera.gameObject.SetActive(true);

        player.transform.SetParent(null);

        Vector3 exitPosition = transform.position + transform.right * 5f;
        player.transform.position = exitPosition;

        if (rb) rb.linearVelocity = Vector3.zero;

        if (characterController) characterController.enabled = true;
        
        if (playerController) playerController.enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerController = player.GetComponent<AstronautPlayer.AstronautPlayer>();
            characterController = player.GetComponent<CharacterController>();
            
            playerNearby = true;
            
            if (!isPlayerInside && enterPrompt)
            {
                enterPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            playerNearby = false;
            if (enterPrompt) enterPrompt.SetActive(false);
        }
    }
}