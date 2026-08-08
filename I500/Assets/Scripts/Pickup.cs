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
        PD_Object.OnPickedUp(_player.T_pickupHook);
        B_canInteract = false;
        C_collider.isTrigger = true;
    }
    public void OnDropped(Vector3 _force)
    {
        PD_Object.OnDropped();
        PD_Object.RB_physics.AddForce(_force, ForceMode.Impulse);
        B_canInteract = true;
        C_collider.isTrigger = false;
    }
}
