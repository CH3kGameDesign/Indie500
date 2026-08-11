using PurrNet;
using UnityEngine;

public class Pickup : Interact
{
    public PhysicsDouble_Object PD_Object;
    public Collider C_collider;
    public override void PlayerInteract(PlayerController _player)
    {
        base.PlayerInteract(_player);
        OnPickedUp(_player);
    }

    public void OnPickedUp(PlayerController _player)
    {
        _player.SetPickup(this);
        OnPickedUp(_player.T_pickupHook);
    }
    
    [ServerRpc(requireOwnership:false)]
    void OnPickedUp(Transform _transform)
    {
        PD_Object.OnPickedUp(_transform);
        B_canInteract.value = false;
        C_collider.isTrigger = true;
    }
    
    [ServerRpc(requireOwnership:false)]
    public void OnDropped(Vector3 _force)
    {
        PD_Object.OnDropped();
        PD_Object.RB_physics.AddForce(_force, ForceMode.Impulse);
        B_canInteract.value = true;
        C_collider.isTrigger = false;
    }
}
