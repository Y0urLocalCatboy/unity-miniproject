using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CatController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform leftPaw;
    public Transform rightPaw;

    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float lookSensitivity = 0.05f;
    public float attackDistance = 0.6f;
    public float attackSpeed = 12f;

    private Rigidbody rb;
    private float verticalRotation = 0f;
    private bool isGrounded;
    private bool isLeftAttacking = false;
    private bool isRightAttacking = false;

    private Vector3 originalLeftPawPos;
    private Vector3 originalRightPawPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform) cameraTransform.localRotation = Quaternion.identity;

        if (leftPaw) originalLeftPawPos = leftPaw.localPosition;
        if (rightPaw) originalRightPawPos = rightPaw.localPosition;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * lookSensitivity;

            transform.Rotate(Vector3.up, mouseDelta.x, Space.World);

            verticalRotation -= mouseDelta.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

            if (cameraTransform)
            {
                cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            if (Keyboard.current.qKey.wasPressedThisFrame && !isLeftAttacking && leftPaw)
            {
                StartCoroutine(AttackRoutine(leftPaw, originalLeftPawPos, true));
            }

            if (Keyboard.current.eKey.wasPressedThisFrame && !isRightAttacking && rightPaw)
            {
                StartCoroutine(AttackRoutine(rightPaw, originalRightPawPos, false));
            }
        }
    }

    void FixedUpdate()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;
        }

        Vector3 moveDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;
    }

    IEnumerator AttackRoutine(Transform paw, Vector3 originalPos, bool isLeft)
    {
        if (isLeft) isLeftAttacking = true;
        else isRightAttacking = true;

        Vector3 targetPos = originalPos + Vector3.forward * attackDistance;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            paw.localPosition = Vector3.Lerp(originalPos, targetPos, elapsed);
            elapsed += Time.deltaTime * attackSpeed;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 1f)
        {
            paw.localPosition = Vector3.Lerp(targetPos, originalPos, elapsed);
            elapsed += Time.deltaTime * attackSpeed;
            yield return null;
        }

        paw.localPosition = originalPos;

        if (isLeft) isLeftAttacking = false;
        else isRightAttacking = false;
    }
}