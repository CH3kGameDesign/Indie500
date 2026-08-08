using UnityEngine;

public class Interact : MonoBehaviour
{
    public string S_verb = "Drive [0]";
    public string S_name = "Bus";
    [HideInInspector] public bool B_canInteract = true;

    public string GetInteractString()
    {
        string _temp = S_verb.Replace("[0]", S_name);
        return _temp;
    }
    public virtual void PlayerInteract(PlayerController _player)
    {
        
    }
}
