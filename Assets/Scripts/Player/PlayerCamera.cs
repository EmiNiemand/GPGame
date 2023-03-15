using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // External components
    // -------------------
    private Camera playerCamera;
    private Rigidbody2D playerRigidbody;

    // SmoothDamp variables
    // --------------------
    private Vector3 smoothDampVelocity;

    // Follow calculation variables
    private Vector2 playerDirection;
    private Vector3 targetPos;

    // Follow control variables
    // -----------------
    // Offset sets automatically, based on placement on scene
    private Vector3 cameraOffset;
    [Header("Settings")]
    [Range(0.0f, 2.0f)]
    [Tooltip("How much time can camera take before reaching target point; bigger value = more smoothing")]
    [SerializeField] private float followDuration;
    [Range(0, 50)]
    [Tooltip("Bigger value = less smoothing")]
    [SerializeField] private int targetPointSpeed;
    [SerializeField] private float horizontalOffset = 5;
    // [SerializeField] private float runningOffset = 1;
    // [SerializeField] private float standingOffset = 4;
    [SerializeField] private float verticalOffset = 10;

    // Debug variables
    // ---------------
    [Header("Debug")]
    [SerializeField] private GameObject debugCircle;


    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCamera = Camera.main;
        cameraOffset = playerCamera.transform.position - transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Get player direction based on velocity,
        // with "deadzone" (for very small values assign zero)
        Vector2 speedDirection = playerRigidbody.velocity;

        if(Mathf.Abs(speedDirection.x) < 0.001f) speedDirection.x = 0;
        if(Mathf.Abs(speedDirection.y) < 0.001f) speedDirection.y = 0;
        speedDirection = new Vector2(System.Math.Sign(speedDirection.x), System.Math.Sign(speedDirection.y));
        // informs of player rotation
        playerDirection = new Vector2(speedDirection.x != 0 ? speedDirection.x : playerDirection.x,
                                      speedDirection.y != 0 ? speedDirection.y : playerDirection.y);
        
        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 playerPos = transform.position;
        float speedScalarX = horizontalOffset;
        // TODO: needs improvement to make it work like in Dead Cells
        // - for x axis, throw camera forward on stay
        // - for y axis, follow player closely
        //speedScalarX -= Mathf.Clamp(Mathf.Abs(playerRigidbody.velocity.x), runningOffset, standingOffset);

        float speedScalarY = Mathf.Abs(playerRigidbody.velocity.y) > verticalOffset ? Mathf.Clamp(Mathf.Abs(playerRigidbody.velocity.y), verticalOffset, verticalOffset*1.2f) : 0;

        Vector3 newTargetPos = cameraOffset;
        newTargetPos.x += playerPos.x + speedScalarX * playerDirection.x;
        newTargetPos.y += playerPos.y + speedScalarY * speedDirection.y;

        if(targetPos == Vector3.zero) targetPos = newTargetPos;
        // Additional smoothing of target point
        targetPos = Vector3.MoveTowards(targetPos, newTargetPos, targetPointSpeed * Time.deltaTime);

        if(debugCircle)
            debugCircle.transform.position = new Vector3(targetPos.x, targetPos.y, 0);

        playerCamera.transform.position = Vector3.SmoothDamp(cameraPos, targetPos, ref smoothDampVelocity, followDuration);
    }

}
