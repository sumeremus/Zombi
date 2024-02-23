using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableButton : MonoBehaviour
{
    Tween buttonTween;
    [SerializeField] GameObject attachedObject;
    void Start()
    {
        buttonTween = transform.DOMove(transform.position - transform.up * 0.1f, 0.5f).SetAutoKill(false).Pause();
    }

    public void ActivateButton()
    {
        buttonTween.PlayForward();
        if (attachedObject.CompareTag("Door"))
        {
            attachedObject.GetComponent<Animator>().CrossFade("UnlockDoor", 0.3f);
        }
        else if (attachedObject.CompareTag("PullablePlatform"))
        {
            attachedObject.transform.GetComponent<PullablePlatform>().PullPlatform();
        }
    }

    public void DeactivateButton()
    {
        buttonTween.PlayBackwards();
        if (attachedObject.CompareTag("Door"))
        {
            attachedObject.GetComponent<Animator>().CrossFade("LockDoor", 0.3f);
        }
        else if (attachedObject.CompareTag("PullablePlatform"))
        {
            attachedObject.transform.GetComponent<PullablePlatform>().RewindPlatform();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            ActivateButton();
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DeactivateButton();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("PullableObject"))
        {
            ActivateButton();
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("PullableObject"))
        {
            DeactivateButton();
        }
    }
}
