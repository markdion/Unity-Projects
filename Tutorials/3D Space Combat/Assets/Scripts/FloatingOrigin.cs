﻿// FloatingOrigin.cs
// Written by Peter Stirling
// 11 November 2010
// Uploaded to Unify Community Wiki on 11 November 2010
// Updated to Unity 5.x particle system by Tony Lovell 14 January, 2016
// URL: http://wiki.unity3d.com/index.php/Floating_Origin
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 100.0f;
    public float physicsThreshold = 1000.0f; // Set to zero to disable

#if OLD_PHYSICS
    public float defaultSleepVelocity = 0.14f;
    public float defaultAngularVelocity = 0.14f;
#else
    public float defaultSleepThreshold = 0.14f;
#endif

    ParticleSystem.Particle[] parts = null;

    void LateUpdate()
    {
        if (GameManager.instance.Player.CurrentState != Player.State.Warping)
        {
            Vector3 cameraPosition = gameObject.transform.position;
            cameraPosition.y = 0f;
            if (cameraPosition.magnitude > threshold)
            {
                Object[] objects = FindObjectsOfType(typeof(Transform));
                foreach (Object o in objects)
                {
                    if ((o as Transform).gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                    {
                        continue;
                    }

                    Transform t = (Transform)o;
                    if (t.parent == null)
                    {
                        t.position -= cameraPosition;
                    }
                }

                // new particles... very similar to old version above
                objects = FindObjectsOfType(typeof(ParticleSystem));
                foreach (UnityEngine.Object o in objects)
                {
                    ParticleSystem sys = (ParticleSystem)o;

                    if (sys.simulationSpace != ParticleSystemSimulationSpace.World)
                        continue;

                    if (parts == null || parts.Length < sys.maxParticles)
                        parts = new ParticleSystem.Particle[sys.maxParticles];

                    int num = sys.GetParticles(parts);
                    for (int i = 0; i < num; ++i)
                    {
                        parts[i].position -= cameraPosition;
                    }

                    sys.SetParticles(parts, num);
                }

                if (physicsThreshold > 0f)
                {
                    float physicsThreshold2 = physicsThreshold * physicsThreshold; // simplify check on threshold
                    objects = FindObjectsOfType(typeof(Rigidbody));
                    foreach (UnityEngine.Object o in objects)
                    {
                        Rigidbody r = (Rigidbody)o;
                        if (r.gameObject.transform.position.sqrMagnitude > physicsThreshold2)
                        {
#if OLD_PHYSICS
                        r.sleepAngularVelocity = float.MaxValue;
                        r.sleepVelocity = float.MaxValue;
#else
                            r.sleepThreshold = float.MaxValue;
#endif
                        }
                        else
                        {
#if OLD_PHYSICS
                        r.sleepAngularVelocity = defaultSleepVelocity;
                        r.sleepVelocity = defaultAngularVelocity;
#else
                            r.sleepThreshold = defaultSleepThreshold;
#endif
                        }
                    }
                }
            }
        }
    }
}