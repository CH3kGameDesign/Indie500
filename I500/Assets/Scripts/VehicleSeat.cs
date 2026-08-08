using UnityEngine;

public class VehicleSeat : Interact
{
    public PrometeoCarController V_vehicle;

    public override void PlayerInteract(PlayerController _player)
    {
        base.PlayerInteract(_player);
        _player.SetVehicle(V_vehicle);
        B_canInteract = false;
    }

    public void PlayerExit()
    {
        B_canInteract = true;
    }
}
