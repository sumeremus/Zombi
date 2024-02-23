using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStick : MonoBehaviour
{
    [SerializeField] LockedDoor lockedDoor;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ammo"))
        {
            lockedDoor.Unlock();
        }
    }

}
