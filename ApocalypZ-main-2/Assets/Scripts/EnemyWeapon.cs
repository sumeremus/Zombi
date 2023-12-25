using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    GameObject objAffectedFromHit;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objAffectedFromHit != other.transform.root.gameObject)
        {
            Debug.Log("Player Hitted");
            objAffectedFromHit = other.transform.root.gameObject;
            PlayerScript.instance.lifePoint -= 30;
            PlayerScript.instance.PlayerLifeCheck();
        }
    }

    public void ResetAffectedObjFromHit()
    {
        objAffectedFromHit = null;
    }
}
