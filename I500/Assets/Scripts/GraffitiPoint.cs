using UnityEngine;

public class GraffitiPoint : Interact
{
    GraffitiLine _line;
    int _pointNum = 0;
    void Start()
    {
        _line = this.GetComponentInParent<GraffitiLine>();
        for (int i = 0; i < _line.points.Length; i++)
            if(_line.points[i] == this)
                _pointNum = i;
    }
    public override void PlayerInteract(PlayerController _player)
    {
        base.PlayerInteract(_player);
        _line.PlayerInteract(_player, _pointNum);
    }
}