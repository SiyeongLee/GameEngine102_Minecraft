using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 재시작을 위해 필요

public class PlayerControoller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sneakSpeed = 2.5f; // 웅크리기 속도 (보통 절반)
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 3f;

    [Header("Fall Settings")]
    public float fallThreshold = -30f; // 이 높이 아래로 떨어지면 재시작

    float xRotation = 0f;
    CharacterController controller;
    Transform cam;
    Vector3 velocity;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cam == null)
        {
            cam = GetComponentInChildren<Camera>()?.transform;
        }
    }

    void Update()
    {
        // 1. 낙사 체크 (맵 밖으로 떨어지면 재시작)
        if (transform.position.y < fallThreshold)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        HandleMove();
        HandleLook();
    }

    void HandleMove()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 2. 웅크리기(Shift) 입력 확인
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSneaking ? sneakSpeed : moveSpeed;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 및 거리 계산
        Vector3 moveDir = transform.right * h + transform.forward * v;
        Vector3 moveDelta = moveDir * currentSpeed * Time.deltaTime;

        // 3. 웅크리기 시 낙하 방지 (Safe Walk) 로직
        if (isSneaking && isGrounded)
        {
            // 갈려고 하는 위치(현재위치 + 이동량) 아래에 땅이 있는지 체크
            if (!CheckGround(transform.position + moveDelta))
            {
                // 땅이 없다면(떨어지는 곳이라면), X축 이동만 시도해본다 (모서리 타기 허용)
                Vector3 moveX = transform.right * h * currentSpeed * Time.deltaTime;
                if (CheckGround(transform.position + moveX))
                {
                    moveDelta = moveX; // X축으로는 이동 가능
                }
                else
                {
                    // 그것도 안되면 Z축 이동만 시도해본다
                    Vector3 moveZ = transform.forward * v * currentSpeed * Time.deltaTime;
                    if (CheckGround(transform.position + moveZ))
                    {
                        moveDelta = moveZ; // Z축으로는 이동 가능
                    }
                    else
                    {
                        // 둘 다 떨어지는 길이라면 아예 멈춤
                        moveDelta = Vector3.zero;
                    }
                }
            }
        }

        controller.Move(moveDelta);

        // 점프 및 중력 처리
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

    // 웅크리기용 바닥 체크 함수
    bool CheckGround(Vector3 targetPos)
    {
        // 캐릭터의 발 위치(targetPos)에서 아래로 레이저를 쏴서 땅이 있는지 확인
        // RayOrigin: targetPos에서 약간 위(0.5f)
        // Direction: 아래(Vector3.down)
        // Distance: 1.0f (0.5f 위에서 쏘니까 발 밑 0.5f까지 검사)
        return Physics.Raycast(targetPos + Vector3.up * 0.5f, Vector3.down, 1.0f);
    }
}