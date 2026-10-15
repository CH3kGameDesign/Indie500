using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshPro[] TM_time;
    public TextMeshPro[] TM_avgSpeed;
    private float f_time = 0;
    public float f_distance = 10f;
    private bool b_active = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    
    void DisplayTime()
    {
        foreach (var item in TM_time) item.text = (Mathf.Round(f_time*100f)/100f).ToString() + "s";
        foreach (var item in TM_avgSpeed) item.text = (Mathf.Round(f_distance / f_time * 100f)/100f).ToString() + "m/s";
    }

    // Update is called once per frame
    void Update()
    {
        if (b_active) TimerUpdate();
    }
    
    void TimerUpdate()
    {
        f_time += Time.deltaTime;
        DisplayTime();
    }

    public void StartTimer() { b_active = true; ResetTimer(); }
    public void ResetTimer()
    {
        f_time = 0;
        DisplayTime();
    }
    public void EndTimer()
    {
        b_active = false;
        DisplayTime();
    }
}
