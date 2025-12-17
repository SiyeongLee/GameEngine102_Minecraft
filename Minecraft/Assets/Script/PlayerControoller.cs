using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControoller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sneakSpeed = 2.5f; // 웅크리기 속도
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 3f;

    [Header("Crouch Settings")]
    public float crouchYOffset = 0.4f;      // 웅크릴 때 카메라가 내려가는 정도
    public float crouchTransitionSpeed = 10f; // 카메라 이동 속도 (부드러움)

    [Header("Fall Settings")]
    public float fallThreshold = -30f; // 낙사 높이

    private CharacterController controller;
    private Transform cam;
    private Vector3 velocity;
    private bool isGrounded;

    private float xRotation = 0f;
    private float defaultCamY; // 원래 카메라 높이 저장용

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // 카메라 찾기
        if (cam == null)
        {
            cam = GetComponentInChildren<Camera>()?.transform;
        }

        // 시작할 때 원래 카메라의 Y 위치를 저장해둡니다.
        if (cam != null)
        {
            defaultCamY = cam.localPosition.y;
        }
    }

    void Update()
    {
        // 1. 낙사 체크
        if (transform.position.y < fallThreshold)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        HandleMove();
        HandleLook();
        HandleCrouchView(); // 4. 카메라 높이 조절 함수 호출
    }

    void HandleMove()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 2. 웅크리기 입력 확인
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSneaking ? sneakSpeed : moveSpeed;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = transform.right * h + transform.forward * v;
        Vector3 moveDelta = moveDir * currentSpeed * Time.deltaTime;

        // 3. 웅크리기 시 낙하 방지 (Safe Walk)
        if (isSneaking && isGrounded)
        {
            if (!CheckGround(transform.position + moveDelta))
            {
                Vector3 moveX = transform.right * h * currentSpeed * Time.deltaTime;
                if (CheckGround(transform.position + moveX))
                {
                    moveDelta = moveX;
                }
                else
                {
                    Vector3 moveZ = transform.forward * v * currentSpeed * Time.deltaTime;
                    if (CheckGround(transform.position + moveZ))
                    {
                        moveDelta = moveZ;
                    }
                    else
                    {
                        moveDelta = Vector3.zero;
                    }
                }
            }
        }

        controller.Move(moveDelta);

        // 점프 (웅크린 상태에서는 점프 불가하게 하려면 && !isSneaking 추가)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (cam != null)
            cam.localRotation = Quaternion.Euler(xRotation, 0f, 0);
    }

    // 4. 카메라 시점 높이 조절 로직
    void HandleCrouchView()
    {
        if (cam == null) return;

        float targetY = defaultCamY; // 기본은 원래 높이

        // 쉬프트를 누르고 있으면 목표 높이를 낮춤
        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetY = defaultCamY - crouchYOffset;
        }

        // 현재 높이에서 목표 높이까지 부드럽게 이동 (Lerp)
        float newY = Mathf.Lerp(cam.localPosition.y, targetY, Time.deltaTime * crouchTransitionSpeed);

        // 카메라 위치 적용
        cam.localPosition = new Vector3(cam.localPosition.x, newY, cam.localPosition.z);
    }

    bool CheckGround(Vector3 targetPos)
    {
        return Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1.0f);
    }
}