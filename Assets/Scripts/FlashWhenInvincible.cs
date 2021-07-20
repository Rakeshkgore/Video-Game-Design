using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FlashWhenInvincible : MonoBehaviour
{
    public Invincibility character;
    public Material[] flashMaterialsWhenInvincible;

    private const float flashHalfPeriod = 12f / 60f;
    private float invincibleSince = float.NaN;
    private new Renderer renderer;
    private Material[] originalMaterials;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        Debug.Assert(character != null, "Character must not be null");
    }

    void LateUpdate()
    {
        if (character.IsInvincible())
        {
            if (float.IsNaN(invincibleSince))
            {
                invincibleSince = Time.time;
                originalMaterials = renderer.sharedMaterials;
            }

            renderer.sharedMaterials =
                (Mathf.FloorToInt((Time.time - invincibleSince) / flashHalfPeriod) % 2 == 0)
                ? flashMaterialsWhenInvincible
                : originalMaterials;
        }
        else
        {
            if (!float.IsNaN(invincibleSince))
            {
                invincibleSince = float.NaN;
                renderer.sharedMaterials = originalMaterials;
                originalMaterials = null;
            }
        }
    }
}
