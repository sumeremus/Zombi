using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmmoScript : MonoBehaviour
{
    public GameObject player;

    Rigidbody rb;

    private Transform attachedObject;

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
        attachedObject = collision.transform;
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("Collision Enter");
            collision.transform.GetComponent<EnemyController>().lifePoint -= 30;
        transform.GetComponentInChildren<Collider>().enabled = false;
            collision.transform.root.GetComponent<Rigidbody>().AddForce((attachedObject.transform.position - player.transform.position) * 100 * Time.deltaTime, ForceMode.Impulse);
            transform.parent = collision.gameObject.GetComponentInChildren<Collider>().transform;
            collision.transform.root.DOLookAt(player.transform.position, 0.8f, AxisConstraint.Y);

            collision.transform.GetComponent<EnemyController>().EnemyLifeCheck();

        }
        else
        {
            SoundManager.instance.PlayClipAtPoint("Zbam", transform.position, 0.8f, 1.0f);
        }

        if (collision.transform.CompareTag("RocknRoll"))
        {
            transform.parent = collision.gameObject.GetComponentInChildren<Collider>().transform;
            transform.GetComponentInChildren<Collider>().enabled = false;
            collision.transform.GetComponent<Rigidbody>().isKinematic = false;
            collision.transform.GetComponent<Rigidbody>().AddTorque(transform.right * 50000 * Time.deltaTime, ForceMode.Impulse);
        }

        if (collision.transform.CompareTag("InteractableButton"))
        {
            collision.transform.GetComponent<InteractableButton>().ActivateButton();
        }

    }

    public void OnGetBack()
    {
        if (attachedObject == null) return;
        Debug.Log("OnGetBack");
        Debug.Log(attachedObject.name);
        if (attachedObject.root.CompareTag("Enemy"))
        {
            if (attachedObject.root.TryGetComponent<Rigidbody>(out Rigidbody _rb))
            {
                attachedObject.transform.GetComponent<EnemyController>().EnemyLifeCheck();
                //_rb.AddForce((player.transform.position - sapliNesne.transform.position) * 3200 * Time.deltaTime, ForceMode.Impulse);
            }
        }
        else if (attachedObject.root.CompareTag("PullablePlatform"))
        {
            if (attachedObject.root.TryGetComponent<PullablePlatform>(out PullablePlatform _))
            {
                attachedObject.transform.GetComponent<PullablePlatform>().PullPlatform();
            }
        }
        else if (attachedObject.CompareTag("PullableObjectTarget"))
        {
            if (attachedObject.root.TryGetComponent<Rigidbody>(out Rigidbody _rb))
            {
                attachedObject.root.transform.DOMove(attachedObject.root.transform.position+attachedObject.root.transform.forward*2, 0.5f );
            }
        }
        else if (attachedObject.CompareTag("InteractableButton"))
        {
            attachedObject.GetComponent<InteractableButton>().DeactivateButton();
        }


        attachedObject = null;
    }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.transform.CompareTag("Pullable"))
    //     {
    //         collision.transform.GetComponent<PullablePlatform>().PullPlatform();
    //     }

    // }
}
