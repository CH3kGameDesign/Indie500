using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Dreamteck.Splines;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region References
    [Header("References")]
    public Rigidbody RB_playerPhysics;
    public Animator A_model;
    public CinemachineCamera CM_camera;
    public Transform T_playerModelHolder;
    public Transform T_playerModel;
    public Transform T_cameraHook;
    public Transform T_pickupHook;
    public TextMeshProUGUI TM_interactText;
    public SplineFollower SF_splineFollower;
    public ParticleSystem PS_chargeParticles;
    #endregion
    #region Input
    private inputClass Input = new inputClass();
    class inputClass
    {
        public Vector2 V2_Move = new Vector2();
        public Vector2 V2_Look = new Vector2();
        public bool B_jump = false;
        public bool B_jumpHeld = false;
        public bool B_interact = false;
        public bool B_sprint = false;
        public bool B_crouch = false;
        public bool B_fire = false;
    }
    private Vector3 v3_rotMove = new Vector3();
    private Vector3 q_rotLook = new Vector3();
    private Vector3 q_rotCameraHook = new Vector3();
    private Vector3 q_rotPlayerModel = new Vector3();
    [HideInInspector] public bool b_grounded = false;
    [HideInInspector] public bool b_groundPound = false;
    [HideInInspector] public bool b_crouching = false;
    [HideInInspector] public bool b_sprinting = false;
    [HideInInspector] public int i_chargeLevel = -1;
    [HideInInspector] public bool b_charging = false;
    [HideInInspector] public bool b_chargeBursting = false;
    [HideInInspector] public bool b_sliding = false;
    [HideInInspector] public float f_sprintHeld = 0;
    private bool b_canJump = true;
    private bool b_canDash = true;
    private bool b_wallRunning = false;
    private bool b_dashing = false;
    private bool b_canWallRun = true;
    private bool b_wallRunLeft = false;
    private bool b_wallRunRight = false;
    private bool b_grinding = false;
    private bool b_canGrind = true;
    #endregion
    #region Movement
    [Header("Movement")]
    public float F_moveSpeed = 10;
    public float F_sprintMultiplier = 1.5f;
    public float F_grindSpeedMultiplier = 3f;
    public float F_airMultiplier = 2f;
    public float F_groundDrag = 10;
    [Space(10)]
    public List<ChargeLevelClass> ChargeLevels = new List<ChargeLevelClass>();
    [Serializable]
    public class ChargeLevelClass
    {
        public float f_timeRequirement = 0f;
        public float f_speedMultiplier = 2f;
        public float f_duration = 1f;
        [Space(10)]
        public Color C_color = Color.whiteSmoke;
    }
    [Space(10)]
    public float F_wallRunCheckDistance = 0.5f;
    public float F_wallRunStickiness = 100f;
    public float F_wallRun_OutJumpForce = 5;
    private Collider wallHit_Last;
    private RaycastHit wallHit_Left;
    private RaycastHit wallHit_Right;
    [Space(10)]
    public float F_jumpForce = 5;
    public float F_crouchJumpForce = 7;
    public float F_groundPoundForce = 7;
    public float F_jumpTimer = 0.1f;
    public int I_midAirJumps = 1;
    private int _midAirJumpCounter = 0;
    private Vector3 v3_slideDirection = Vector3.zero;
    public float F_slideTimer = 1f;
    public float F_dashForce = 5;
    public float F_dashTimer = 0.2f;
    public int I_midAirDashes = 1;
    private int _midAirDashCounter = 0;
    public float F_playerHeight = 1f;
    public LayerMask LM_ground = new LayerMask();
    [Space(10)]
    public float F_throwForce = 10f;
    #endregion
    #region Camera
    [Header("Camera")]
    public float F_lookSpeed = 10;
    public Vector2 V2_xLookBounds = new Vector2(-80, 80);
    public cameraJuiceClass CameraJuice = new cameraJuiceClass();
    [System.Serializable]
    public class cameraJuiceClass
    {
        public float FOV_base = 60f;
        public float FOV_sprint = 70f;
        public float FOV_crouch = 50f;
        public float FOV_groundPound = 40f;
        public float Dutch_wallRun = 10f;
    }
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
        CameraJuiceHandler();
        AnimationUpdate();
        if (!_curVehicle)
            InteractInput();
    }
    Vector2 v2_animMove;
    void AnimationUpdate()
    {
        Vector2 _input = Input.V2_Move;
        if (b_sprinting)
            _input.y = 2;
        if (b_chargeBursting)
            _input.y = 3;
        v2_animMove = Vector2.Lerp(v2_animMove, _input, Time.deltaTime / 0.25f);
        A_model.SetFloat("posX", v2_animMove.x);
        A_model.SetFloat("posY", v2_animMove.y);
        A_model.SetBool("Crouching", b_crouching);
        A_model.SetBool("Moving", v2_animMove.magnitude > 0.1f);
        A_model.SetBool("InAir", !b_grounded);
        A_model.SetBool("WallRunLeft", b_wallRunLeft);
        A_model.SetBool("WallRunRight", b_wallRunRight);
        A_model.SetBool("Diving", b_sliding || b_dashing);
        A_model.SetBool("Charge", b_charging);
        A_model.SetBool("Grind", b_grinding);
    }

    void FixedUpdate()
    {
        if (!_curVehicle)
        {
            JumpHandler();
            SprintHandler();
            WallRunHandler();
            CrouchHandler();
            PhysicsMovement();
            GrindRail_Update();
            InteractHandler();
        }
        else
            CarHandler();
    }
    private void JumpHandler()
    {
        GroundCheck();
        if (Input.B_jump)
        {
            if (b_canJump && !b_charging)
            {
                if (b_grounded)
                    Jump();
                else if (!Input.B_jumpHeld)
                {
                    if (b_wallRunning)
                        Jump_WallRun();
                    else if (_midAirJumpCounter < I_midAirJumps && !b_groundPound)
                        Jump_MidAir();
                }
            }
            Input.B_jumpHeld = true;
        }
        else
            Input.B_jumpHeld = false;
    }
    private void SprintHandler()
    {
        if (b_sprinting)
        {
            if (Input.V2_Move.magnitude < 0.1f && !Input.B_sprint)
                b_sprinting = false;
        }
        if (Input.B_sprint)
        {
            b_sprinting = true;
            if (f_sprintHeld == 0)
            {
                if (b_canDash)
                {
                    if (!b_grounded && !b_wallRunning && _midAirDashCounter < I_midAirDashes)
                        Dash_MidAir();
                }
            }
            if (
                f_sprintHeld > 0.25f && f_sprintHeld < 0.35f &&
                b_grounded && Input.V2_Move.magnitude < 0.1f &&
                !b_charging && !b_crouching && !b_chargeBursting)
            {
                Charge_Start();
            }
            f_sprintHeld += Time.deltaTime;
            Charge_Update();
        }
        else
        {
            f_sprintHeld = 0;
            Charge_Activate();
        }
    }
    private void Charge_Start()
    {
        b_charging = true;
        Charge_SetLevel(0);
        PS_chargeParticles.Play();
    }
    private void Charge_Update()
    {
        if (!b_charging) return;
        if (i_chargeLevel + 1 >= ChargeLevels.Count) return;
        if (ChargeLevels[i_chargeLevel + 1].f_timeRequirement <= f_sprintHeld)
            Charge_SetLevel(i_chargeLevel + 1);
    }
    private void Charge_SetLevel(int _charge)
    {
        i_chargeLevel = _charge;
        if (i_chargeLevel < 0) return;
        var mainModule = PS_chargeParticles.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(ChargeLevels[i_chargeLevel].C_color);
    }
    private void Charge_Activate()
    {
        if (!b_charging) return;
        b_charging = false;
        b_chargeBursting = true;
        PS_chargeParticles.Stop();
        Invoke(nameof(Charge_End), ChargeLevels[i_chargeLevel].f_duration);
    }
    private void Charge_End()
    {
        b_chargeBursting = false;
        Charge_SetLevel(-1);
    }
    private void Dash_MidAir()
    {
        b_dashing = true;
        RB_playerPhysics.linearVelocity = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);
        RB_playerPhysics.useGravity = false;
        b_canDash = false;
        b_canJump = false;
        A_model.SetBool("Diving", true);
        A_model.Play("Slide");
        Vector3 _force = T_playerModelHolder.forward * F_dashForce;
        RB_playerPhysics.AddForce(_force, ForceMode.Impulse);
        Invoke(nameof(ResetDash), F_dashTimer);
        _midAirDashCounter++;
    }
    private void Jump()
    {
        if (b_grinding)
            GrindRail_End();
        A_model.Play("Jump");
        b_canJump = false;
        RB_playerPhysics.linearVelocity = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);
        Vector3 _force = transform.up * ((b_crouching && b_grounded) ? F_crouchJumpForce : F_jumpForce);
        RB_playerPhysics.AddForce(_force, ForceMode.Impulse);
        Invoke(nameof(ResetJump), F_jumpTimer);
    }
    private void Jump_MidAir()
    {
        Jump();
        _midAirJumpCounter++;
    }
    public void BouncePad()
    {
        b_canJump = false;
        Invoke(nameof(ResetJump), F_jumpTimer);
    }
    private void Jump_WallRun()
    {
        Jump();
        b_canWallRun = false;
        WallRunEnd();
        Vector3 wallNormal = b_wallRunRight ? wallHit_Right.normal : wallHit_Left.normal;
        RB_playerPhysics.AddForce(wallNormal * F_wallRun_OutJumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetWallRun), F_jumpTimer);
    }
    void ResetWallRun() { b_canWallRun = true; }
    Coroutine CrouchCoroutine = null;
    private void CrouchHandler()
    {
        if (Input.B_crouch)
        {
            if (!b_crouching && !b_charging)
            {
                //Start Crouching
                if (CrouchCoroutine != null)
                    StopCoroutine(CrouchCoroutine);
                CrouchCoroutine = StartCoroutine(Crouch(true));
                //Ground Pound
                if (b_grounded && b_sprinting)
                {
                    Slide();
                }
                if (!b_grounded && !b_wallRunning)
                {
                    GroundPound();
                }
            }
        }
        else if (b_crouching && !b_sliding)
        {
            if (CanStandUp())
            {
                //Stop Crouching
                if (CrouchCoroutine != null)
                    StopCoroutine(CrouchCoroutine);
                CrouchCoroutine = StartCoroutine(Crouch(false));
            }
        }
    }
    private void Slide()
    {
        if (!b_sliding)
        {
            b_sliding = true;
            A_model.SetBool("Diving", true);
            A_model.Play("Slide");
            Invoke(nameof(ResetSlide), F_slideTimer);
        }
    }
    private void GroundPound()
    {
        if (!b_groundPound)
        {
            b_groundPound = true;
            RB_playerPhysics.linearVelocity = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);
            Vector3 _force = -transform.up * F_groundPoundForce;
            RB_playerPhysics.AddForce(_force, ForceMode.Impulse);
        }
    }
    private bool CanStandUp()
    {
        RaycastHit hit;
        return !Physics.SphereCast(RB_playerPhysics.position + (Vector3.up / 2), 0.2f, Vector3.up, out hit, F_playerHeight * 0.5f + 0.1f, LM_ground);
    }
    IEnumerator Crouch(bool _crouching = false, float _time = 0.1f)
    {
        b_crouching = _crouching;
        float _timer = 0;
        Vector3 _startScale = RB_playerPhysics.transform.localScale;
        Vector3 _tarScale = _crouching ? new Vector3(1, 0.3f, 1) : new Vector3(1, 1, 1);
        while (_timer <= 1)
        {
            _timer += Time.deltaTime / _time;
            RB_playerPhysics.transform.localScale = Vector3.Lerp(_startScale, _tarScale, _timer);
            yield return new WaitForEndOfFrame();
        }
        RB_playerPhysics.transform.localScale = _tarScale;
    }
    private void WallRunHandler()
    {
        WallRunCheck();

        if (!b_grounded && b_canWallRun && Input.V2_Move.y > 0.5f)
        {
            if (b_wallRunRight || b_wallRunLeft)
            {
                if (!b_wallRunning)
                    WallRunStart();

                Vector3 wallNormal = b_wallRunRight ? wallHit_Right.normal : wallHit_Left.normal;
                RB_playerPhysics.AddForce(-wallNormal * F_wallRunStickiness, ForceMode.Force);
            }
            else if (b_wallRunning)
                WallRunEnd();
        }
        else if (b_wallRunning)
            WallRunEnd();
    }
    private void WallRunCheck()
    {
        //Left Check
        if (Physics.Raycast(RB_playerPhysics.transform.position + Vector3.up, -T_playerModelHolder.transform.right, out wallHit_Left, F_wallRunCheckDistance))
        {
            if (wallHit_Left.collider != wallHit_Last || b_wallRunLeft)
            {
                b_wallRunLeft = wallHit_Left.collider.tag == "WallRun";
                if (b_wallRunLeft)
                    wallHit_Last = wallHit_Left.collider;
            }
            else
                b_wallRunLeft = false;
        }
        else
            b_wallRunLeft = false;
        //Right Check
        if (Physics.Raycast(RB_playerPhysics.transform.position + Vector3.up, T_playerModelHolder.transform.right, out wallHit_Right, F_wallRunCheckDistance))
        {
            if (wallHit_Right.collider != wallHit_Last || b_wallRunRight)
            {
                b_wallRunRight = wallHit_Right.collider.tag == "WallRun";
                if (b_wallRunRight)
                    wallHit_Last = wallHit_Right.collider;
            }
            else
                b_wallRunLeft = false;
        }
        else
            b_wallRunRight = false;
    }
    private void WallRunStart()
    {
        b_wallRunning = true;
        RB_playerPhysics.useGravity = false;
        RB_playerPhysics.linearVelocity = new Vector3(RB_playerPhysics.linearVelocity.x, 0, RB_playerPhysics.linearVelocity.z);
        _midAirJumpCounter = 0;
        _midAirDashCounter = 0;
    }
    private void WallRunEnd()
    {
        b_wallRunning = false;
        b_wallRunLeft = false;
        b_wallRunRight = false;
        RB_playerPhysics.useGravity = true;
    }
    private void ResetJump() { b_canJump = true; }
    private void ResetDash() { b_dashing = false; b_canDash = true; b_canJump = true; RB_playerPhysics.useGravity = true; }
    private void ResetSlide() { b_sliding = false; }
    private void GroundCheck()
    {
        RaycastHit hit;
        b_grounded = Physics.SphereCast(RB_playerPhysics.position + Vector3.up, 0.15f, Vector3.down, out hit, F_playerHeight * 0.5f + 0.1f, LM_ground);
        if (b_grounded)
        {
            RB_playerPhysics.linearDamping = F_groundDrag;
            wallHit_Last = null;
            _midAirJumpCounter = 0;
            _midAirDashCounter = 0;
            GrindRail_Check(hit);
            Invoke(nameof(DisableGroundPound), 0.2f);
        }
        else
        {
            RB_playerPhysics.linearDamping = 0;
            ResetSlide();
        }
    }
    private void GrindRail_Check(RaycastHit hit)
    {
        if (!b_canGrind) return;
        if (hit.collider.tag == "GrindRail")
        {
            SplineComputer _spline;
            if (hit.collider.transform.parent.TryGetComponent<SplineComputer>(out _spline))
                GrindRail_Start(_spline, hit.point);
        }
    }
    private void GrindRail_Start(SplineComputer _sc, Vector3 _position)
    {
        b_grinding = true;
        b_canGrind = false;

        SF_splineFollower.spline = _sc;
        SF_splineFollower.follow = true;

        SplineSample _progress = _sc.Project(_position);
        //Set Position
        SF_splineFollower.SetPercent(_progress.percent);
        //Set Direction
        Vector3 _dir = v3_rotMove;
        float movementDot = Vector3.Dot(_dir, _progress.forward);
        curGrindSpeed = Mathf.Max(v3_rotMove.magnitude * F_grindSpeedMultiplier, 1f);
        if (movementDot >= 0)
        {
            SF_splineFollower.direction = Spline.Direction.Forward;
        }
        else
        {
            curGrindSpeed *= -1f;
            SF_splineFollower.direction = Spline.Direction.Backward;
        }
        SF_splineFollower.followSpeed = curGrindSpeed;
    }
    float curGrindSpeed = 3;
    private void GrindRail_Update()
    {
        if (!b_grinding) return;
        if (v3_rotMove.AlmostZero()) return;

        SplineComputer _sc = SF_splineFollower.spline;
        SplineSample _progress = _sc.Project(RB_playerPhysics.position);
        //Set Direction
        Vector3 _dir = v3_rotMove;
        float movementDot = Vector3.Dot(_dir, _progress.forward);
        if (SF_splineFollower.direction == Spline.Direction.Forward)
        {
            curGrindSpeed = Mathf.Lerp(curGrindSpeed, movementDot * v3_rotMove.magnitude * F_grindSpeedMultiplier, Time.fixedDeltaTime);
            if (movementDot < 0f && curGrindSpeed < 0.1f)
            {
                curGrindSpeed = -1f;
                SF_splineFollower.direction = Spline.Direction.Backward;
            }
        }
        if (SF_splineFollower.direction == Spline.Direction.Backward)
        {
            curGrindSpeed = Mathf.Lerp(curGrindSpeed, movementDot * v3_rotMove.magnitude * F_grindSpeedMultiplier, Time.fixedDeltaTime);
            if (movementDot > 0f && curGrindSpeed > -0.1f)
            {
                curGrindSpeed = 1f;
                SF_splineFollower.direction = Spline.Direction.Forward;
            }
        }
        SF_splineFollower.followSpeed = curGrindSpeed;
    }
    private void GrindRail_End()
    {
        b_grinding = false;
        SF_splineFollower.spline = null;
        SF_splineFollower.follow = false;
        Invoke(nameof(ResetGrind), F_jumpTimer);
    }
    public void GrindRail_ReachedEnd()
    {
        Jump();
        GrindRail_End();
    }
    void ResetGrind() { b_canGrind = true; }
    void DisableGroundPound() { b_groundPound = false; }
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
        if (b_chargeBursting)
            v3_rotMove = Quaternion.Euler(q_rotPlayerModel) * new Vector3(Input.V2_Move.x * 0.25f, 0, 1) * F_moveSpeed * ChargeLevels[i_chargeLevel].f_speedMultiplier;
        else
            v3_rotMove = Quaternion.Euler(q_rotPlayerModel) * new Vector3(Input.V2_Move.x, 0, Input.V2_Move.y) * 10 * F_moveSpeed * Time.fixedDeltaTime;
        v3_rotMove *= 10 * F_moveSpeed * Time.fixedDeltaTime;
        if (!b_charging)
        {
            if (!b_grounded) v3_rotMove *= F_airMultiplier;
            if (b_sprinting || b_wallRunning || b_chargeBursting) v3_rotMove *= F_sprintMultiplier;
            if (b_sliding) v3_rotMove = v3_slideDirection;
            else v3_slideDirection = v3_rotMove;
            RB_playerPhysics.AddForce(v3_rotMove, ForceMode.Impulse);
        }
        SpeedControl();
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(RB_playerPhysics.linearVelocity.x, 0f, RB_playerPhysics.linearVelocity.z);

        float _moveSpeed = F_moveSpeed;
        if (b_sprinting || b_wallRunning || b_chargeBursting) _moveSpeed *= F_sprintMultiplier;
        if (b_chargeBursting) _moveSpeed *= ChargeLevels[i_chargeLevel].f_speedMultiplier;
        if (!b_grounded) _moveSpeed *= F_airMultiplier;
        // limit velocity if needed
        if(flatVel.magnitude > _moveSpeed && !b_dashing && !b_sliding)
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
        if (b_chargeBursting)
            q_rotLook += new Vector3(-Input.V2_Look.y, 0, 0) * F_lookSpeed * 0.2f * Time.deltaTime;
        else
            q_rotLook += new Vector3(-Input.V2_Look.y, Input.V2_Look.x, 0) * F_lookSpeed * Time.deltaTime;
        q_rotLook.x = Mathf.Clamp(q_rotLook.x, V2_xLookBounds.x, V2_xLookBounds.y);
        q_rotPlayerModel.y = q_rotLook.y;
        q_rotCameraHook.x = q_rotLook.x;
        Quaternion _playerRot = Quaternion.Lerp(T_playerModelHolder.localRotation, Quaternion.Euler(q_rotPlayerModel), Time.deltaTime * 20);
        Quaternion _cameraRot = Quaternion.Lerp(T_cameraHook.localRotation, Quaternion.Euler(q_rotCameraHook), Time.deltaTime * 20);
        T_playerModelHolder.localRotation = _playerRot;
        T_cameraHook.localRotation = _cameraRot;
        //3rd Person Model Rot
        if (Input.V2_Move.magnitude > 0.05f)
        {
            Quaternion _playerModelRot = Quaternion.Lerp(T_playerModel.localRotation, Quaternion.LookRotation(new Vector3(Input.V2_Move.y, 0, -Input.V2_Move.x), Vector3.up), Time.deltaTime * 20);
            T_playerModel.localRotation = _playerModelRot;
        }
    }
    
    void CameraJuiceHandler()
    {
        float tarFieldOfView = CameraJuice.FOV_base;
        float tarDutch = 0;
        float lerpSpeed = Time.deltaTime * 10;
        if (b_sprinting || b_wallRunning) { tarFieldOfView = CameraJuice.FOV_sprint; }
        if (b_groundPound) { tarFieldOfView = CameraJuice.FOV_groundPound; }
        if (b_grounded && b_crouching) { tarFieldOfView = CameraJuice.FOV_crouch; }
        if (b_wallRunning)
        {
            if (b_wallRunRight) { tarDutch = CameraJuice.Dutch_wallRun; }
            else { tarDutch = -CameraJuice.Dutch_wallRun; }
        }
        CM_camera.Lens.FieldOfView = Mathf.Lerp(CM_camera.Lens.FieldOfView, tarFieldOfView, lerpSpeed);
        CM_camera.Lens.Dutch = Mathf.Lerp(CM_camera.Lens.Dutch, tarDutch, lerpSpeed);
    }
    public void AdjustCameraOffset(PhysicsDouble_Surface _surface)
    {
        Transform _old = T_playerModelHolder.parent;
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
    public void Input_Crouch(InputAction.CallbackContext cxt) { Input.B_crouch = cxt.performed; }
    public void Input_Fire(InputAction.CallbackContext cxt) { Input.B_fire = cxt.performed; }
}
