using System.Collections;
using TMPro;
using UnityEditor.Rendering;
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
    public Transform T_pickupHook;
    public TextMeshProUGUI TM_interactText;
    #endregion
    #region Input
    private inputClass Input = new inputClass();
    class inputClass
    {
        public Vector2 V2_Move = new Vector2();
        public Vector2 V2_Look = new Vector2();
        public bool B_jump = false;
        public bool B_interact = false;
        public bool B_sprint = false;
        public bool B_fire = false;
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
    public float F_sprintMultiplier = 1.5f;
    public float F_airMultiplier = 2f;
    public float F_groundDrag = 10;
    [Space(10)]
    public float F_jumpForce = 10;
    public float F_jumpTimer = 0.1f;
    public float F_playerHeight = 1f;
    public LayerMask LM_ground = new LayerMask();
    [Space(10)]
    public float F_throwForce = 10f;
    #endregion
    #region Camera
    [Header("Camera")]
    public float F_lookSpeed = 10;
    public Vector2 V2_xLookBounds = new Vector2(-80, 80);
    #endregion
    private VehicleSeat _curVehicle = null;
    private Pickup _curPickup = null;
    private Interact _curInteract = null;
    private Coroutine _interactCoyote = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCursorLock(true);
        InteractCheck();
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
        CameraMovement();
        if (!_curVehicle)
            InteractInput();
    }

    void FixedUpdate()
    {
        if (!_curVehicle)
        {
            JumpHandler();
            PhysicsMovement();
            InteractHandler();
        }
        else
            CarHandler();
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
    void InteractInput()
    {
        if (_curPickup != null)
        {
            ThrowHandler();
            return;
        }
        if (_curInteract != null && Input.B_interact)
        {
            Input.B_interact = false;
            _curInteract.PlayerInteract(this);
        }
    }
    private bool _aiming = false;
    void ThrowHandler()
    {
        if (Input.B_interact)
        {
            Input.B_interact = false;
            _curPickup.OnDropped(Vector3.zero);
            _curPickup = null;
            _aiming = false;
        }
        else if (Input.B_fire)
        {
            _aiming = true;
        }
        else if (_aiming)
        {
            _curPickup.OnDropped(T_cameraHook.transform.forward * F_throwForce);
            _curPickup = null;
            _aiming = false;
        }
    }
    public void SetVehicle(VehicleSeat _seat) { _curVehicle = _seat; }
    public void SetPickup(Pickup _pickup)
    {
        _curPickup = _pickup;
    }
    void InteractHandler()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hit, 5))
        {
            Interact temp;
            if (hit.collider.TryGetComponent<Interact>(out temp))
            {
                if (_interactCoyote != null)
                { StopCoroutine(_interactCoyote); _interactCoyote = null; }
                InteractCheck(temp);
            }
            else InteractCoyoteTimeCheck();
        }
        else InteractCoyoteTimeCheck();
    }
    void InteractCheck(Interact _interact = null)
    {
        bool _valid = _interact != null;
        if (_valid) _valid = _interact.B_canInteract;
        if (!_valid)
        {
            _curInteract = null;
            TM_interactText.text = "";
        }
        else if (_interact != _curInteract)
        {
            _curInteract = _interact;
            TM_interactText.text = _curInteract.GetInteractString();
        }
    }
    void InteractCoyoteTimeCheck()
    {
        if (_interactCoyote == null)
            _interactCoyote = StartCoroutine(InteractCoyoteTime());
    }
    IEnumerator InteractCoyoteTime()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        InteractCheck(null);
         _interactCoyote = null; 
    }
    void PhysicsMovement()
    {
        v3_rotMove = Quaternion.Euler(q_rotPlayerModel) * new Vector3(Input.V2_Move.x, 0, Input.V2_Move.y) * 10 * F_moveSpeed * Time.fixedDeltaTime;
        if (!b_grounded) v3_rotMove *= F_airMultiplier;
        else if (Input.B_sprint) v3_rotMove *= F_sprintMultiplier;
        RB_playerPhysics.AddForce(v3_rotMove, ForceMode.Impulse);
        SpeedControl();
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);

        float _moveSpeed = F_moveSpeed;
        if (Input.B_sprint) _moveSpeed *= F_sprintMultiplier;
        // limit velocity if needed
        if(flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * F_moveSpeed;
            RB_playerPhysics.linearVelocity = new Vector3(limitedVel.x, RB_playerPhysics.linearVelocity.y, limitedVel.z);
        }
    }
    void CarHandler()
    {
        _curVehicle.V_vehicle.Input_Move(Input.V2_Move);
        if (Input.B_interact)
        {
            _curVehicle.V_vehicle.Input_Move(Vector2.zero);
            _curVehicle.PlayerExit();
            Input.B_interact = false;
            _curVehicle = null;
        }
    }
    void CameraMovement()
    {
        q_rotLook += new Vector3(-Input.V2_Look.y, Input.V2_Look.x, 0) * F_lookSpeed * Time.deltaTime;
        q_rotLook.x = Mathf.Clamp(q_rotLook.x, V2_xLookBounds.x, V2_xLookBounds.y);
        q_rotPlayerModel.y = q_rotLook.y;
        q_rotCameraHook.x = q_rotLook.x;
        Quaternion _playerRot = Quaternion.Lerp(T_playerModel.localRotation, Quaternion.Euler(q_rotPlayerModel), Time.deltaTime * 20);
        Quaternion _cameraRot = Quaternion.Lerp(T_cameraHook.localRotation, Quaternion.Euler(q_rotCameraHook), Time.deltaTime * 20);
        T_playerModel.localRotation = _playerRot;
        T_cameraHook.localRotation = _cameraRot;
    }
    public void AdjustCameraOffset(PhysicsDouble_Surface _surface)
    {
        Transform _old = T_playerModel.parent;
        Transform _new;
        if (_surface == null) _new = transform;
        else _new = _surface.T_visualModel;
        
        Quaternion _offset = _old.rotation * Quaternion.Inverse(_new.rotation);

        Vector3 _euler = _offset.eulerAngles;
        q_rotLook.y += _euler.y;
    }

    public void Input_Move(InputAction.CallbackContext cxt) { Input.V2_Move = cxt.ReadValue<Vector2>(); }
    public void Input_Look(InputAction.CallbackContext cxt) { Input.V2_Look = cxt.ReadValue<Vector2>(); }
    public void Input_Jump(InputAction.CallbackContext cxt) { Input.B_jump = cxt.performed; }
    public void Input_Interact(InputAction.CallbackContext cxt) { Input.B_interact = cxt.performed; }
    public void Input_Sprint(InputAction.CallbackContext cxt) { Input.B_sprint = cxt.performed; }
    public void Input_Fire(InputAction.CallbackContext cxt) { Input.B_fire = cxt.performed; }
}
