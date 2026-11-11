using UnityEngine;
using UnityEngine.Events;

public class GraffitiLine : MonoBehaviour
{
    public GraffitiPoint[] points;
    public GameObject[] enabledObjects;
    public GameObject[] disabledObjects;
    public UnityEvent UE_OnFinish;
    public void PlayerInteract(PlayerController _player, int _num = 0)
    {
        bool _ascending = _num <= (points.Length / 2);
        _player.Graffiti_Start(this, GenerateButtons(), _ascending);
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

    public void ResetGraffiti_Delay(float _delay = 1)
    {
        Invoke(nameof(ResetGraffiti), _delay);
    }
    public void ResetGraffiti()
    {
        foreach (var item in enabledObjects)
            if (item != null)
                item.SetActive(true);
        foreach (var item in disabledObjects)
            if (item != null)
                item.SetActive(false);
    }
}
