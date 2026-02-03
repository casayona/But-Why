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
    public float rotationSpeed = 180f; // Saniyedeki dönüþ açýsý
    public float rotationSmoothTime = 0.15f; // Dönüþün yumuþama süresi
    public float jumpHeight = 1.5f;

    [Header("Sliding Settings")]
    public float normalHeight = 1.8f;
    public float slideHeight = 0.6f;
    public Vector3 normalCenter = new Vector3(0, 0.9f, 0);
    public Vector3 slideCenter = new Vector3(0, 0.3f, 0);

    [Header("Visual Juice (Hissiyat)")]
    public Transform visualModel; // Karakterin modelini (grafiðini) buraya sürükle
    public float leanAmount = 15f; // Dönüþlerdeki yatma miktarý
    public float leanSpeed = 5f;

    [Header("Physics")]
    public float gravity = -19.62f;

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isSprinting;
    private bool isSliding;
    private float smoothMoveAmount;

    // Yumuþatma için yardýmcý deðiþkenler
    private float currentRotationVelocity;
    private float rotationVelocitySmooth;
    private float currentLeanAngle;

    private void Awake()
    {
        inputActions = new PenguinInputs();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

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

        ApplyRotation();
        MovePlayer();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        UpdateAnimations();
    }

    private void ApplyRotation()
    {
        // 1. DÖNÜÞ YUMUÞATMA
        // moveInput.x (-1, 0, 1) deðerini yumuþak bir hýza çeviriyoruz
        float targetRotationVelocity = moveInput.x * rotationSpeed;
        currentRotationVelocity = Mathf.SmoothDamp(currentRotationVelocity, targetRotationVelocity, ref rotationVelocitySmooth, rotationSmoothTime);

        transform.Rotate(0, currentRotationVelocity * Time.deltaTime, 0);

        // 2. GÖRSEL YATMA EFEKTÝ (Lean)
        // Eðer visualModel atanmýþsa, dönüþ yönüne göre modeli Z ekseninde yatýrýr
        if (visualModel != null)
        {
            float targetLean = -moveInput.x * leanAmount;
            currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, Time.deltaTime * leanSpeed);

            // Modelin mevcut rotasyonunu bozmadan sadece Z (yatma) eksenini deðiþtiriyoruz
            visualModel.localRotation = Quaternion.Euler(0, 0, currentLeanAngle);
        }
    }

    private void MovePlayer()
    {
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