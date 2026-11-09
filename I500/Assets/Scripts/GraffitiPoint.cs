using UnityEngine;

public class GraffitiPoint : Interact
{
    GraffitiLine _line;
    void Start()
    {
        _line = this.GetComponentInParent<GraffitiLine>();
    }
    public override void PlayerInteract(PlayerController _player)
    {
        base.PlayerInteract(_player);
        _line.PlayerInteract(_player);
    }
}