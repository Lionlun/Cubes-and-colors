using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skybox : MonoBehaviour
{
    [SerializeField] private Material day;
    [SerializeField] private Material evening;
    [SerializeField] private Material night;
    [SerializeField] private HeightMeasurement heightMeasurment;

    [SerializeField] private float maxDayHeight = 5;
    [SerializeField] private float maxEveningHeight = 10;
    [SerializeField] private float maxNightHeight = 15;


    private void FixedUpdate()
    {
        if(heightMeasurment.Height <= maxDayHeight)
        {
            RenderSettings.skybox = day;
        }
        if(heightMeasurment.Height > maxDayHeight && heightMeasurment.Height <= maxEveningHeight)
        {
            RenderSettings.skybox = evening;
        }
        if (heightMeasurment.Height > maxEveningHeight)
        {
            RenderSettings.skybox = night;
        }
    }
}
