using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmmoScript : MonoBehaviour
{
    public GameObject player;

    Rigidbody rb;

    private Transform sapliNesne;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("Collision Enter");
            sapliNesne = collision.transform;
            collision.transform.GetComponent<EnemyController>().lifePoint -= 30;
            collision.transform.root.GetComponent<Rigidbody>().AddForce((sapliNesne.transform.position - player.transform.position) * 100 * Time.deltaTime, ForceMode.Impulse);
            transform.parent = collision.gameObject.GetComponentInChildren<Collider>().transform;
            transform.GetComponentInChildren<Collider>().enabled = false;
            collision.transform.root.DOLookAt(player.transform.position, 0.8f, AxisConstraint.Y);

            collision.transform.GetComponent<EnemyController>().EnemyLifeCheck();

        }
        else
        {
            SoundManager.instance.PlayClipAtPoint("Zbam", transform.position, 0.8f, 1.0f);
        }

    }

    public void OnGetBack()
    {
        if (sapliNesne == null) return;
        if (!sapliNesne.root.TryGetComponent<Rigidbody>(out Rigidbody _rb)) return;
        sapliNesne.transform.GetComponent<EnemyController>().EnemyLifeCheck();
        //_rb.AddForce((player.transform.position - sapliNesne.transform.position) * 3200 * Time.deltaTime, ForceMode.Impulse);
        Debug.Log("OnGetBack");
        sapliNesne = null;
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //if (collision.transform.CompareTag("Enemy"))
    //{
    //    Debug.Log("Collision Exit");
    //    collision.transform.root.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position) * 10 * Time.deltaTime, ForceMode.Impulse);
    //    collision.gameObject.GetComponent<Animator>().SetBool("isHitting", true);
    //}

    //}
}
