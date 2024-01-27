using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float UpdateInterval = 0.5f; // How often should the number update

    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;
    private float fps;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        timeleft = UpdateInterval;
    }

    private void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // Display two fractional digits (f2 format)
            fps = (accum / frames);
            timeleft = UpdateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        // Display the fps and round to 2 decimals
        GUI.Label(new Rect(5, 5, 100, 25), fps.ToString("F2") + "FPS");
    }
}
