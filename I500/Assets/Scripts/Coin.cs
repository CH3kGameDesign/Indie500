using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PhysicsDouble_Object _PDO;
        if (other.TryGetComponent<PhysicsDouble_Object>(out _PDO))
        {
            if (_PDO.PC_player != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
