using UnityEngine;

public class PhysicsDouble_Surface : MonoBehaviour
{
    public Transform T_visualModel;
    public Collider C_visualBounds;
    [Space(10)]
    public Transform T_physicsModel;
    public Collider C_physicsBounds;

    public void OnTriggerEnter(Collider other)
    {
        PhysicsDouble_Object _temp;
        if (other.TryGetComponent<PhysicsDouble_Object>(out _temp))
            _temp.EnterSurface(this);
    }

    public void OnTriggerExit(Collider other)
    {
        PhysicsDouble_Object _temp;
        if (other.TryGetComponent<PhysicsDouble_Object>(out _temp))
            _temp.ExitSurface(this);
    }
}
