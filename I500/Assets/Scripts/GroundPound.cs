using UnityEngine;

public class GroundPound : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PhysicsDouble_Object _PDO;
        if (other.TryGetComponent<PhysicsDouble_Object>(out _PDO))
        {
            if (_PDO.PC_player != null)
            {
                if (_PDO.PC_player.b_groundPound)
                    gameObject.SetActive(false);
            }
        }
    }
}
