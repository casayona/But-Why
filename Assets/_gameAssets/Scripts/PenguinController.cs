using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PenguinController : MonoBehaviour
{
    private PenguinInputs inputActions;
    private CharacterController controller;
    private Animator anim;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float slideSpeed = 15f;
    public float rotationSpeed = 100f; // Karakterin A/D ile dönme hýzý
    public float jumpHeight = 1.5f;

    [Header("Sliding Settings")]
    public float normalHeight = 1.8f;
    public float slideHeight = 0.6f;
    public Vector3 normalCenter = new Vector3(0, 0.9f, 0);
    public Vector3 slideCenter = new Vector3(0, 0.3f, 0);

    [Header("Physics")]
    public float gravity = -19.62f;

    [Header("References")]
    public Transform mainCamera; // Kamera artýk sadece görsel takip için, hareket yönünü etkilemez

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isSprinting;
    private bool isSliding;
    private float smoothMoveAmount;

    private void Awake()
    {
        inputActions = new PenguinInputs();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Input Atamalarý
        inputActions.PenguinActions.Jump.performed += ctx => Jump();
        inputActions.PenguinActions.Sprint.started += ctx => isSprinting = true;
        inputActions.PenguinActions.Sprint.canceled += ctx => isSprinting = false;
        inputActions.PenguinActions.Slide.started += ctx => StartSlide();
        inputActions.PenguinActions.Slide.canceled += ctx => StopSlide();
    }

    private void OnEnable() => inputActions.PenguinActions.Enable();
    private void OnDisable() => inputActions.PenguinActions.Disable();

    private void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        moveInput = inputActions.PenguinActions.Move.ReadValue<Vector2>();

        // 1. DÖNME: A ve D tuþlarý karakteri kendi ekseninde döndürür
        float rotation = moveInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);

        // 2. HAREKET: W ve S tuþlarý karakterin baktýðý yöne (forward) gitmesini saðlar
        MovePlayer();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        UpdateAnimations();
    }
    private void FixedUpdate()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void MovePlayer()
    {
        // Sadece W ve S (Dikey eksen) hareket yönünü belirler
        // Karakterin transform.forward deðerini kullanarak "baktýðý yöne" gitmesini saðlýyoruz
        Vector3 moveDirection = transform.forward * moveInput.y;

        if (moveDirection.magnitude >= 0.1f || isSliding)
        {
            float currentSpeed = walkSpeed;
            if (isSliding) currentSpeed = slideSpeed;
            else if (isSprinting) currentSpeed = sprintSpeed;

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
    }

    private void StartSlide()
    {
        if (!controller.isGrounded) return;
        isSliding = true;
        controller.height = slideHeight;
        controller.center = slideCenter;
    }

    private void StopSlide()
    {
        isSliding = false;
        controller.height = normalHeight;
        controller.center = normalCenter;
    }

    private void UpdateAnimations()
    {
        // Animasyon için sadece ileri-geri (W/S) hareketine bakýyoruz
        float targetMoveAmount = Mathf.Abs(moveInput.y);
        smoothMoveAmount = Mathf.SmoothStep(smoothMoveAmount, targetMoveAmount, Time.deltaTime * 5f);

        anim.SetFloat("Vert", smoothMoveAmount, 0.3f, Time.deltaTime);
        float targetState = isSprinting ? 1f : 0f;
        anim.SetFloat("State", targetState, 0.2f, Time.deltaTime);
        anim.SetBool("IsSliding", isSliding);
    }

    private void Jump()
    {
        if (controller.isGrounded && !isSliding)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}