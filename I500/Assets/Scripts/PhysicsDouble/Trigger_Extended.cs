using UnityEngine;
using UnityEngine.Events;

public class Trigger_Extended : MonoBehaviour
{
    public UnityEvent<Collider> triggerEnter;
    public UnityEvent<Collider> triggerExit;

    void OnTriggerEnter(Collider other) { triggerEnter.Invoke(other); }
    void OnTriggerExit(Collider other) { triggerExit.Invoke(other); }
}
