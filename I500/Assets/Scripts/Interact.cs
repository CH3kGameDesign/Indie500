using PurrNet;
using UnityEngine;

public class Interact : NetworkBehaviour
{
    public string S_verb = "Drive [0]";
    public string S_name = "Bus";
    [HideInInspector] public SyncVar<bool> B_canInteract = new SyncVar<bool>(true);

    public string GetInteractString()
    {
        string _temp = S_verb.Replace("[0]", S_name);
        return _temp;
    }
    public virtual void PlayerInteract(PlayerController _player)
    {
        
    }
}
