using System.Collections.Generic;
using UnityEngine;

public class PhysicsDouble_Object : MonoBehaviour
{
    public Transform T_model;
    public Rigidbody RB_physics;
    public Transform T_parent;
    public PlayerController PC_player;

    PhysicsDouble_Surface _curSurface = null;
    List<PhysicsDouble_Surface> _allSurfaces = new List<PhysicsDouble_Surface>();

    float _timerCheck = 0;
    public void FixedUpdate()
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
    
    public void EnterSurface(PhysicsDouble_Surface _surface)
    {
        Debug.Log("Enter");
        if (!_allSurfaces.Contains(_surface))
            _allSurfaces.Add(_surface);
        SetSurface();
    }
    public void ExitSurface(PhysicsDouble_Surface _surface)
    {
        Debug.Log("Exit");
        if (_allSurfaces.Contains(_surface))
            _allSurfaces.Remove(_surface);
        SetSurface();
    }

    void SetSurface()
    {
        if (_allSurfaces.Count > 0)
            SetSurface(_allSurfaces[0]);
        else
            SetSurface(null);
    }
    void SetSurface(PhysicsDouble_Surface _surface = null)
    {
        //Ignore if target surface == current
        if (_curSurface == _surface)
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
