using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] Vector3 unlockedDoorPos;
    [SerializeField] float unlockTime;
    public void Unlock()
    {
        transform.DOMove(unlockedDoorPos, unlockTime);
        GetComponent<AudioSource>().Play();
    }
}
