using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    List<GameObject> childBreakPoints = new();
    [SerializeField] bool isBreakableWithAmmo = true;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).gameObject.TryGetComponent<ParticleSystem>(out _))
            {
                childBreakPoints.Add(transform.GetChild(i).gameObject);
                transform.GetChild(i).gameObject.AddComponent<BreakableWallPoint>();
            }
        }
    }

    void Update()
    {

    }

    public void BreakWall(GameObject childPoint)
    {
        if (!isBreakableWithAmmo) return;
        childBreakPoints.Remove(childPoint);
        ParticleFXManager.instance.PlayBreakableWallPointParticle(childPoint.transform.position);
        if (childBreakPoints.Count <= 0)
        {
            ParticleFXManager.instance.PlayWallBreakParticle(transform.position);
            Destroy(gameObject);
        }

    }

    public void BreakWall()
    {
        ParticleFXManager.instance.PlayWallBreakParticle(transform.position);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("RocknRoll"))
        {
            BreakWall();
        }

        if (collision.transform.CompareTag("Ammo") && childBreakPoints.Count <= 0)
        {
            if (!isBreakableWithAmmo) return;
            BreakWall();
        }
    }
}
