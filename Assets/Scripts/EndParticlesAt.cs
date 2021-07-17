using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EndParticlesAt : MonoBehaviour
{
    public Transform endPoint;
    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        Debug.Assert(endPoint != null, "End Point must not be null");
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, endPoint.position);
        int numParticles = particleSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; ++i)
        {
            ParticleSystem.Particle particle = particles[i];
            if ((particle.startLifetime - particle.remainingLifetime) * particle.velocity.magnitude >= distance)
            {
                particles[i].remainingLifetime = -1f;
            }
        }
        particleSystem.SetParticles(particles, numParticles);
    }
}
