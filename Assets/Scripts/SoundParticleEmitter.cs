using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticleEmitter : MonoBehaviour
{
    public RhinoAI rhino;
    public SoundParticleController prefab;
    public float radius = 1f;
    public int slices = 90;
    public float angle = 5f;
    public float speed = 1f;
    public float emitPeriod = 0.5f;

    private float nextEmitTime = float.NaN;

    void Update()
    {
        if (rhino.IsAnimationPlaying("shout") || rhino.IsAnimationPlaying("hit"))
        {
            if (float.IsNaN(nextEmitTime))
            {
                nextEmitTime = Time.time;
            }
        }
        else
        {
            nextEmitTime = float.NaN;
        }

        if (Time.time >= nextEmitTime)
        {
            for (int i = 0; i < slices; ++i)
            {
                float theta = 2f * Mathf.PI * i / slices;
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                Vector3 position = transform.position + cos * radius * transform.right + sin * radius * transform.up;
                Vector3 velocity = Quaternion.AngleAxis(angle, -sin * transform.right + cos * transform.up) * transform.forward * speed;

                SoundParticleController particle = Instantiate(prefab, position, transform.rotation);
                particle.velocity = velocity;
            }

            nextEmitTime += emitPeriod;
        }
    }
}
