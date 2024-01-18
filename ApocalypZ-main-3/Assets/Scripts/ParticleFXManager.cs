using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFXManager : MonoBehaviour
{
    //Singleton FX Manager
    public static ParticleFXManager instance;

    [SerializeField] ParticleSystem wallBreakParticle;
    [SerializeField] ParticleSystem breakableWallPointParticle;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple ParticleFXManager instances detected!");
            Destroy(this);
        }
    }

    public void PlayWallBreakParticle(Vector3 position)
    {
        wallBreakParticle.transform.position = position;
        wallBreakParticle.Play();
    }

    public void PlayBreakableWallPointParticle(Vector3 position)
    {
        breakableWallPointParticle.transform.position = position;
        breakableWallPointParticle.Play();
    }
    
}
