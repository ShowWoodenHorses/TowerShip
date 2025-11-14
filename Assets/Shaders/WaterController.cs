using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterController : MonoBehaviour
{
    [Header("Water Colors")]
    [SerializeField] private Color baseColor = new Color(0.2f, 0.5f, 0.8f, 1.0f);
    [SerializeField] private Color waveColor = new Color(0.1f, 0.3f, 0.6f, 1.0f);

    [Header("Wave Settings")]
    [SerializeField, Range(0, 2)] private float waveHeight = 0.5f;
    [SerializeField, Range(0, 5)] private float waveSpeed = 1.0f;
    [SerializeField, Range(0, 10)] private float waveFrequency = 1.5f;
    [SerializeField, Range(0.1f, 5f)] private float waveSharpness = 2.0f;

    [Header("Visual Effects")]
    [SerializeField, Range(0, 1)] private float specularPower = 0.5f;

    [Header("Performance")]
    //[SerializeField] private bool updateInEditMode = true;
    //[SerializeField] private float updateRate = 30f;

    private Material waterMaterial;
    private float lastUpdateTime;

    private void OnEnable()
    {
        InitializeWater();
    }

    //private void OnValidate()
    //{
    //    if (waterMaterial != null)
    //    {
    //        UpdateMaterialProperties();
    //    }
    //}

    //private void Update()
    //{
    //    if (!Application.isPlaying && !updateInEditMode) return;

    //    if (Time.time - lastUpdateTime < 1f / updateRate)
    //        return;

    //    if (waterMaterial != null)
    //    {
    //        UpdateMaterialProperties();
    //        lastUpdateTime = Time.time;
    //    }
    //}

    private void InitializeWater()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            waterMaterial = renderer.sharedMaterial;
            UpdateMaterialProperties();
        }
    }

    private void UpdateMaterialProperties()
    {
        if (waterMaterial == null) return;

        waterMaterial.SetColor("_BaseColor", baseColor);
        waterMaterial.SetColor("_WaveColor", waveColor);
        waterMaterial.SetFloat("_WaveHeight", waveHeight);
        waterMaterial.SetFloat("_WaveSpeed", waveSpeed);
        waterMaterial.SetFloat("_WaveFrequency", waveFrequency);
        waterMaterial.SetFloat("_Sharpness", waveSharpness);

        if (waterMaterial.HasProperty("_SpecularPower"))
            waterMaterial.SetFloat("_SpecularPower", specularPower);
    }
}