using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script to fade in and out a light for an effect, as well as randomize movement for constant effects
/// </summary>
public class LightScript : MonoBehaviour
{
    [Tooltip("Random seed for movement, 0 for no movement.")]
    public float Seed = 100.0f;

    [Tooltip("Multiplier for light intensity.")]
    public float IntensityModifier = 2.0f;

    [SingleLine("Min and max intensity range.")]
    public RangeOfFloats IntensityMaxRange = new RangeOfFloats { Minimum = 0.0f, Maximum = 8.0f };

    private Light PointLight;
    private float lightIntensity;
    private float seed;
    private BaseScript BaseScript;
    private float baseY;

    private void Awake()
    {
        // find a point light
        PointLight = gameObject.GetComponentInChildren<Light>();
        if (PointLight != null)
        {
            // we have a point light, set the intensity to 0 so it can fade in nicely
            lightIntensity = PointLight.intensity;
            PointLight.intensity = 0.0f;
            baseY = PointLight.gameObject.transform.position.y;
        }
        seed = UnityEngine.Random.value * Seed;
        BaseScript = gameObject.GetComponent<BaseScript>();
    }

    private void Update()
    {
        if (PointLight == null)
        {
            return;
        }

        if (seed != 0)
        {
            // we have a random movement seed, set up with random movement
            bool setIntensity = true;
            float intensityModifier2 = 1.0f;
            if (BaseScript != null)
            {
                if (BaseScript.Stopping)
                {
                    // don't randomize intensity during a stop, it looks bad
                    setIntensity = false;
                    PointLight.intensity = Mathf.Lerp(PointLight.intensity, 0.0f, BaseScript.StopPercent);
                }
                else if (BaseScript.Starting)
                {
                    intensityModifier2 = BaseScript.StartPercent;
                }
            }

            if (setIntensity)
            {
                float intensity = Mathf.Clamp(IntensityModifier * intensityModifier2 * Mathf.PerlinNoise(seed + Time.time, seed + 1 + Time.time),
                    IntensityMaxRange.Minimum, IntensityMaxRange.Maximum);
                PointLight.intensity = intensity;
            }

            // random movement with perlin noise
            float x = Mathf.PerlinNoise(seed + 0 + Time.time * 2, seed + 1 + Time.time * 2) - 0.5f;
            float y = baseY + Mathf.PerlinNoise(seed + 2 + Time.time * 2, seed + 3 + Time.time * 2) - 0.5f;
            float z = Mathf.PerlinNoise(seed + 4 + Time.time * 2, seed + 5 + Time.time * 2) - 0.5f;
            PointLight.gameObject.transform.localPosition = Vector3.up + new Vector3(x, y, z);
        }
        else if (BaseScript.Stopping)
        {
            // fade out
            PointLight.intensity = Mathf.Lerp(PointLight.intensity, 0.0f, BaseScript.StopPercent);
        }
        else if (BaseScript.Starting)
        {
            // fade in
            PointLight.intensity = Mathf.Lerp(0.0f, lightIntensity, BaseScript.StartPercent);
        }
    }
}