using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class SpikyTrap : MonoBehaviour
{
    Tween spikeTween;
    [SerializeField] int spikeDamage = 10;

    void Start()
    {
        spikeTween = transform.DOMoveY(transform.position.y + .35f, .5f).SetAutoKill(false).Pause();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Player hit by spike");
            spikeTween.SetEase(Ease.OutBounce).PlayForward();
            PlayerScript.instance.TakeDamage(spikeDamage);
        }
    }
    

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
             spikeTween.SetEase(Ease.InBack).PlayBackwards();
        }
    }
}
