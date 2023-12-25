using Cinemachine;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachine;
    [SerializeField] Transform playerUpperChest;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePlayerCameraFollowObj() {
        cinemachine.Follow = playerUpperChest;
        PlayerScript.instance.GetComponent<AimIK>().enabled = false;
    }
}
