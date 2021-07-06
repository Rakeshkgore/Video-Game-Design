using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LerpMaterials : MonoBehaviour
{
    public Material[] materials;
    public float[] stops = {0f, 1f};
    public float duration = 1f;

    private float startTime;
    private int currIndex = 0;
    private new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        Debug.Assert(materials.Length >= 2, "Must have at least two materials to lerp between!");
        Debug.Assert(stops.Length == materials.Length, "Must have same number of stops as materials!");
        Debug.Assert(stops[0] == 0f, "First stop must be 0!");
        Debug.Assert(stops[stops.Length - 1] == 1f, "Last stop must be 1!");
        for (int i = 0; i < stops.Length - 1; ++i)
        {
            Debug.Assert(stops[i] < stops[i + 1], "Stops must be in ascending order!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        renderer.material = materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        float normTime = (Time.time - startTime) / duration;
        while (currIndex < stops.Length - 1 && normTime >= stops[currIndex + 1])
        {
            ++currIndex;
        }

        if (currIndex >= stops.Length - 1)
        {
            renderer.material = materials[materials.Length - 1];
            enabled = false;
        }
        else
        {
            Material from = materials[currIndex];
            Material to = materials[currIndex + 1];
            float lerp = (normTime - stops[currIndex]) / (stops[currIndex + 1] - stops[currIndex]);
            renderer.material.Lerp(from, to, lerp);
        }
    }
}
