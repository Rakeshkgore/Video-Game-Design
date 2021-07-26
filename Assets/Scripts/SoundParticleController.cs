using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(LookAtConstraint))]
public class SoundParticleController : MonoBehaviour
{
    public float fadeInDuration = 0.5f;
    public float stayDuration = 5f;
    public float fadeOutDuration = 0.5f;
    public Vector3 velocity;

    private new Renderer renderer;
    private int colorPropertyId;
    private Color color;
    private MaterialPropertyBlock properties;
    private float spawnedAt;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        color = renderer.sharedMaterial.color;
        colorPropertyId = Shader.PropertyToID("_Color");
        properties = new MaterialPropertyBlock();
        properties.SetColor(colorPropertyId, new Color(color.r, color.g, color.b, 0f));

        GetComponent<LookAtConstraint>().AddSource(
            new ConstraintSource()
            {
                sourceTransform = Camera.main.transform,
                weight = 1f,
            }
        );
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnedAt = Time.time;
        renderer.SetPropertyBlock(properties);
    }

    // Update is called once per frame
    void Update()
    {
        float fadeInStartTime = spawnedAt;
        float fadeOutStartTime = spawnedAt + fadeInDuration + stayDuration;
        float normalizedTimeSinceFadeInStart = (Time.time - fadeInStartTime) / fadeInDuration;
        float normalizedTimeSinceFadeOutStart = (Time.time - fadeOutStartTime) / fadeOutDuration;

        if (normalizedTimeSinceFadeOutStart >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        if (normalizedTimeSinceFadeInStart < 1f)
        {
            float a = normalizedTimeSinceFadeInStart * color.a;
            properties.SetColor(colorPropertyId, new Color(color.r, color.g, color.b, a));
        }
        else if (normalizedTimeSinceFadeOutStart >= 0f)
        {
            float a = (1f - normalizedTimeSinceFadeOutStart) * color.a;
            properties.SetColor(colorPropertyId, new Color(color.r, color.g, color.b, a));
        }
        else
        {
            properties.SetColor(colorPropertyId, color);
        }

        renderer.SetPropertyBlock(properties);
        transform.position += Time.deltaTime * velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("ground"))
        {
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.SendMessage("OnSoundParticleHit", SendMessageOptions.DontRequireReceiver);
            }
            Destroy(gameObject);
        }
    }
}
