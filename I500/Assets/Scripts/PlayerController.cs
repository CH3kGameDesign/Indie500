using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region References
    [Header("References")]
    public Rigidbody RB_playerPhysics;
    public Transform T_playerModel;
    public Transform T_cameraHook;
    #endregion
    #region Input
    private inputClass Input = new inputClass();
    class inputClass
    {
        public Vector2 V2_Move = new Vector2();
        public Vector2 V2_Look = new Vector2();
        public bool B_jump = false;
    }
    private Vector3 v3_rotMove = new Vector3();
    private Vector3 q_rotLook = new Vector3();
    private Vector3 q_rotCameraHook = new Vector3();
    private Vector3 q_rotPlayerModel = new Vector3();
    private bool b_grounded = false;
    private bool b_canJump = true;
    #endregion
    #region Movement
    [Header("Movement")]
    public float F_moveSpeed = 10;
    public float F_airMultiplier = 2f;
    public float F_groundDrag = 10;
    public float F_jumpForce = 10;
    public float F_jumpTimer = 0.1f;
    public float F_playerHeight = 1f;
    public LayerMask LM_ground = new LayerMask();
    #endregion
    #region Camera
    [Header("Camera")]
    public float F_lookSpeed = 10;
    public Vector2 V2_xLookBounds = new Vector2(-80, 80);
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCursorLock(true);
    }

    void SetCursorLock(bool _locked)
    {
        Cursor.visible = !_locked;

        if (_locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        ModelMovement();
        CameraMovement();
    }

    void FixedUpdate()
    {
        JumpHandler();
        PhysicsMovement();
    }
    private void JumpHandler()
    {
        GroundCheck();
        if(b_canJump && b_grounded)
        {
            if (Input.B_jump)
            {
                b_canJump = false;
                RB_playerPhysics.linearVelocity = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);
                RB_playerPhysics.AddForce(transform.up * F_jumpForce, ForceMode.Impulse);
                Invoke(nameof(ResetJump), F_jumpTimer);
            }
        }
    }
    private void ResetJump() { b_canJump = true; }
    private void GroundCheck()
    {
        b_grounded = Physics.Raycast(RB_playerPhysics.position, Vector3.down, F_playerHeight * 0.5f + 0.1f, LM_ground);
        if (b_grounded)
            RB_playerPhysics.linearDamping = F_groundDrag;
        else
            RB_playerPhysics.linearDamping = 0;
    }

    void PhysicsMovement()
    {
        v3_rotMove = Quaternion.Euler(q_rotPlayerModel) * new Vector3(Input.V2_Move.x, 0, Input.V2_Move.y) * 10 * F_moveSpeed * Time.fixedDeltaTime;
        if (!b_grounded) v3_rotMove *= F_airMultiplier;
        RB_playerPhysics.AddForce(v3_rotMove, ForceMode.Impulse);
        SpeedControl();
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > F_moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * F_moveSpeed;
            RB_playerPhysics.linearVelocity = new Vector3(limitedVel.x, RB_playerPhysics.linearVelocity.y, limitedVel.z);
        }
    }
    void ModelMovement()
    {
        T_playerModel.position = RB_playerPhysics.position;
    }
    void CameraMovement()
    {
        q_rotLook += new Vector3(-Input.V2_Look.y, Input.V2_Look.x, 0) * F_lookSpeed * Time.deltaTime;
        q_rotLook.x = Mathf.Clamp(q_rotLook.x, V2_xLookBounds.x, V2_xLookBounds.y);

        q_rotPlayerModel.y = q_rotLook.y;
        q_rotCameraHook.x = q_rotLook.x;

        T_playerModel.localEulerAngles = q_rotPlayerModel;
        T_cameraHook.localEulerAngles = q_rotCameraHook;
    }

    public void Input_Move(InputAction.CallbackContext cxt) { Input.V2_Move = cxt.ReadValue<Vector2>(); }
    public void Input_Look(InputAction.CallbackContext cxt) { Input.V2_Look = cxt.ReadValue<Vector2>(); }
    public void Input_Jump(InputAction.CallbackContext cxt) { Input.B_jump = cxt.performed; }
}
