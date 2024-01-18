using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BreakableWallPoint : MonoBehaviour
{
    BreakableWall parentBreakableWall;
    void Start()
    {
        parentBreakableWall = transform.parent.GetComponent<BreakableWall>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            parentBreakableWall.BreakWall(gameObject);
            Destroy(gameObject);
        }
    }
}
