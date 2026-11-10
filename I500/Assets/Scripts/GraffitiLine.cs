using UnityEngine;

public class GraffitiLine : MonoBehaviour
{
    public GraffitiPoint[] points;
    public GameObject[] enabledObjects;
    public GameObject[] disabledObjects;
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

    public void OnSuccess(int _num)
    {
        if (enabledObjects.Length > _num)
            if (enabledObjects[_num] != null)
                enabledObjects[_num].SetActive(false);
        if (disabledObjects.Length > _num)
            if (disabledObjects[_num] != null)
                disabledObjects[_num].SetActive(true);
    }
}
