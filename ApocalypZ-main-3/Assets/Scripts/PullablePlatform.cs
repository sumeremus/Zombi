using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PullablePlatform : MonoBehaviour
{
    [SerializeField] Vector3 positionToPull;
    [SerializeField] float pullSpeed;
    Tween platformTween;

    void Start()
    {
        platformTween = transform.DOMove(positionToPull, pullSpeed).SetAutoKill(false).Pause();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PullPlatform()
    {
        platformTween.PlayForward();
    }

    public void RewindPlatform()
    {
        platformTween.PlayBackwards();
    }
}
