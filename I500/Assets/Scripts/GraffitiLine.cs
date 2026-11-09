using UnityEngine;

public class GraffitiLine : MonoBehaviour
{
    public GraffitiPoint[] points;
    public void PlayerInteract(PlayerController _player)
    {
        _player.Graffiti_Start(this, GenerateButtons());
    }

    public int[] GenerateButtons()
    {
        int[] _buttons = new int[points.Length];
        for (int i = 0; i < _buttons.Length; i++)
            _buttons[i] = Random.Range(0, 5);
        return _buttons;
    }
}
