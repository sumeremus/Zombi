using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    [SerializeField] float explosionRadius = 1;
    [SerializeField] float explosionPower = 1;
    [SerializeField] int explosionHitPoint = 50;
    ParticleSystem explosionParticle;

    private void Start()
    {
        explosionParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Weapon") || collision.transform.CompareTag("Ammo"))
        {
            Vector3 explosionPos = transform.position;
            HashSet<Rigidbody> alreadyAffectedRigidbodies = new HashSet<Rigidbody>();

            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.transform.root.GetComponentInChildren<Rigidbody>();

                if (rb != null && !alreadyAffectedRigidbodies.Contains(rb))
                {
                    rb.AddExplosionForce(explosionPower, explosionPos, explosionRadius, 3f);
                    alreadyAffectedRigidbodies.Add(rb);

                    if (rb.TryGetComponent<EnemyController>(out EnemyController enemyScript))
                    {
                        Debug.Log("Enemy Exploded");
                        enemyScript.lifePoint -= explosionHitPoint;
                        enemyScript.EnemyLifeCheck();
                    }
                    if (rb.TryGetComponent<PlayerScript>(out _))
                    {
                        Debug.Log("Player Exploded");
                        PlayerScript.instance.lifePoint -= explosionHitPoint;
                        PlayerScript.instance.PlayerLifeCheck();
                    }
                }
            }


            SoundManager.instance.PlayAudio("Explosion");
            explosionParticle.transform.parent = null;
            explosionParticle.Play();
            collision.transform.root.GetComponent<Rigidbody>().useGravity = true;
            collision.transform.root.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
