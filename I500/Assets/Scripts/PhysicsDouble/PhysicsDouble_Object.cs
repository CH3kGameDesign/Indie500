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
        Transform _oldP = RB_physics.transform.parent;
        Transform _newP;
        //Adjust Player Controller rotation offset to maintain consistent look position
        if (PC_player != null)
            PC_player.AdjustCameraOffset(_surface);
        //Update Surface Parent
        if (_surface == null)
        {
            T_model.parent = T_parent;
            RB_physics.transform.parent = T_parent;
            _newP = T_parent;
        }
        else
        {
            T_model.parent = _surface.T_visualModel;
            RB_physics.transform.parent = _surface.T_physicsModel;
            _newP = _surface.T_physicsModel;
        }
        //Adjust Rigidbody Force Direction ////////////////////NEEDS WORK
        Quaternion _offset = _newP.rotation * Quaternion.Inverse(_oldP.rotation);
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
