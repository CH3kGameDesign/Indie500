using System.Collections.Generic;
using UnityEngine;

public class PhysicsDouble_Object : MonoBehaviour
{
    public Transform T_model;
    public Rigidbody RB_physics;
    public Transform T_parent;
    public PlayerController PC_player;

    private bool _updateSurface = true;
    PhysicsDouble_Surface _curSurface = null;
    List<PhysicsDouble_Surface> _allSurfaces = new List<PhysicsDouble_Surface>();

    public void FixedUpdate()
    {
        BoundsCheck();
    }

    float _timerCheck = 0;
    void BoundsCheck()
    {
        if (_curSurface != null)
        {
            if (_timerCheck > 0.1f)
            {
                _timerCheck = 0;
                if (!_curSurface.C_physicsBounds.bounds.Contains(RB_physics.transform.position))
                    ExitSurface(_curSurface);
            }
            _timerCheck += Time.fixedDeltaTime;
        }
        else
            _timerCheck = 0;
    }
    public void OnPickedUp(Transform _transform)
    {
        _updateSurface = false;
        RB_physics.isKinematic = true;
        T_model.parent = _transform;
        T_model.localPosition = Vector3.zero;
        T_model.localRotation = Quaternion.identity;
        RB_physics.transform.parent = _transform;
        RB_physics.transform.localPosition = Vector3.zero;
        RB_physics.transform.localRotation = Quaternion.identity;
    }
    public void OnDropped()
    {
        _updateSurface = true;
        RB_physics.isKinematic = false;
        SetSurface(true);
    }
    public void EnterSurface(PhysicsDouble_Surface _surface)
    {
        if (!_allSurfaces.Contains(_surface))
            _allSurfaces.Add(_surface);
        SetSurface();
    }
    public void ExitSurface(PhysicsDouble_Surface _surface)
    {
        if (_allSurfaces.Contains(_surface))
            _allSurfaces.Remove(_surface);
        SetSurface();
    }

    void SetSurface(bool _override = false)
    {
        if (_updateSurface == false && _override == false) return;
        if (_allSurfaces.Count > 0)
            SetSurfaceSpecific(_allSurfaces[0], _override);
        else
            SetSurfaceSpecific(null, _override);
    }
    void SetSurfaceSpecific(PhysicsDouble_Surface _surface = null, bool _override = false)
    {
        //Ignore if target surface == current
        if (_curSurface == _surface && _override == false)
            return;
        //Save Old Transform Parent
        Transform _oldVisualP = T_model.parent;
        Transform _newVisualP;
        //Adjust Player Controller rotation offset to maintain consistent look position
        if (PC_player != null)
            PC_player.AdjustCameraOffset(_surface);
        //Update Surface Parent
        if (_surface == null)
        {
            T_model.parent = T_parent;
            RB_physics.transform.parent = T_parent;
            _newVisualP = T_parent;
        }
        else
        {
            T_model.parent = _surface.T_visualModel;
            RB_physics.transform.parent = _surface.T_physicsModel;
            _newVisualP = _surface.T_visualModel;
        }
        //Adjust Rigidbody Force Direction
        Quaternion _offset = _oldVisualP.rotation * Quaternion.Inverse(_newVisualP.rotation);
        RB_physics.linearVelocity = _offset * RB_physics.linearVelocity;

        //Adjust Physics Position to remain consistent
        RB_physics.transform.localPosition = T_model.localPosition;
        //Set Current Surface for future checks
        _curSurface = _surface;
    }

    void Update()
    {
        SurfaceUpdate();
    }
    void SurfaceUpdate()
    {
        T_model.localPosition = RB_physics.transform.localPosition;
    }
}
