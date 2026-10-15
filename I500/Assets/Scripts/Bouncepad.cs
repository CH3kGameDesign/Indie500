using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    public float F_force = 10;
    public float F_groundPoundMultiplier = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Bounce(Rigidbody _RB, bool _groundPound = false)
    {
        _RB.linearVelocity = new Vector3(_RB.linearVelocity.x, 0f, _RB.linearVelocity.z);
        Vector3 _force = transform.up * F_force * (_groundPound ? F_groundPoundMultiplier : 1f);
        _RB.AddForce(_force, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other)
    {
        PhysicsDouble_Object _PDO;
        if (other.TryGetComponent<PhysicsDouble_Object>(out _PDO))
        {
            if (_PDO.PC_player != null)
            {
                Bounce(_PDO.RB_physics, _PDO.PC_player.b_groundPound);
                _PDO.PC_player.BouncePad();
                return;
            }
        }
            
        Rigidbody _RB;
        if (other.TryGetComponent<Rigidbody>(out _RB))
            Bounce(_RB);
    }
}
