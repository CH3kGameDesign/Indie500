using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform T_hook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = T_hook.position;
        transform.rotation = Quaternion.LookRotation(T_hook.forward, Vector3.up);
    }
}
