using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleInvisibleMaterialController : MonoBehaviour
{
    public Invincibility invincibility;
    public Invisibility invisibility;

    public Material[] materials;
    private MaterialPropertyBlock[] propertyBlocks;
    private Material[] originalMaterials;
    private MaterialPropertyBlock[] originalPropertyBlocks;
    private int colorPropertyId;

    private const float invincibleOpacity = 0.5f;
    private const float invincibleFlashHalfPeriod = 12f / 60f;
    private const float invisibleOpacity = 0.5f;
    private const float invisibleWarnDuration = 3f;
    private const float invisibleWarnPeriod = 0.5f;
    private float invincibleSince = float.NaN;
    private new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        colorPropertyId = Shader.PropertyToID("_Color");
        Debug.Assert(invincibility != null, "Invincibility must not be null");
        Debug.Assert(invisibility != null, "Invisibility must not be null");

        propertyBlocks = new MaterialPropertyBlock[materials.Length];
        for (int i = 0; i < propertyBlocks.Length; ++i)
        {
            propertyBlocks[i] = new MaterialPropertyBlock();
        }
    }

    void LateUpdate()
    {
        float opacity = CalculateOpacityForInvincibility() * CalculateOpacityForInvisibility();
        if (opacity < 1f)
        {
            if (originalMaterials == null)
            {
                originalMaterials = renderer.sharedMaterials;
                originalPropertyBlocks = new MaterialPropertyBlock[originalMaterials.Length];
                for (int i = 0; i < originalPropertyBlocks.Length; ++i)
                {
                    MaterialPropertyBlock properties = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(properties, i);
                    originalPropertyBlocks[i] = properties;
                }

                renderer.sharedMaterials = materials;
            }

            for (int i = 0; i < materials.Length; i++)
            {
                MaterialPropertyBlock properties = propertyBlocks[i];
                Color color = materials[i].GetColor(colorPropertyId);
                color.a = opacity;
                properties.SetColor(colorPropertyId, color);
                renderer.SetPropertyBlock(properties, i);
            }
        }
        else
        {
            if (originalMaterials != null)
            {
                renderer.sharedMaterials = originalMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    renderer.SetPropertyBlock(originalPropertyBlocks[i], i);
                }

                originalMaterials = null;
                originalPropertyBlocks = null;
            }
        }
    }

    private float CalculateOpacityForInvincibility()
    {
        if (!invincibility.IsInvincible())
        {
            invincibleSince = float.NaN;
            return 1f;
        }

        if (float.IsNaN(invincibleSince))
        {
            invincibleSince = Time.time;
        }

        float invincibleElapsed = Time.time - invincibleSince;
        if (Mathf.FloorToInt(invincibleElapsed / invincibleFlashHalfPeriod) % 2 != 0)
        {
            return 1f;
        }

        return invincibleOpacity;
    }

    private float CalculateOpacityForInvisibility()
    {
        if (!invisibility.IsInvisible())
        {
            return 1f;
        }

        float remainingTime = invisibility.invisibleUntil - Time.time;
        float warnPeriodsElapsed = (invisibleWarnDuration - remainingTime) / invisibleWarnPeriod;
        if (warnPeriodsElapsed < 0f)
        {
            return invisibleOpacity;
        }

        float warnPhase = warnPeriodsElapsed % 1f;
        return Mathf.Lerp(invisibleOpacity, 1f, warnPhase);
    }
}
