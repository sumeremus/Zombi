using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    Animator animator;

    [SerializeField] GameObject ammoModel;
    [SerializeField] Transform weaponObj;
    [SerializeField] GameObject throwedAmmo;
    Rigidbody throwedAmmoRb;
    [SerializeField] int throwAmmoForce;
    [SerializeField] float recallAmmoForce;
    [SerializeField] Vector3 weaponOffset = new Vector3(0, 0.1f, 0);
    bool callingAmmo = false;
    bool isAmmoOnWeapon = true;

    // UI
    [SerializeField] Image lifeBarImg;
    [SerializeField] public int maxLife = 100;
    [SerializeField] public int lifePoint = 100;

    Tween vibr;

    //DASH
    //[SerializeField] float dashDistance = 5f;
    //[SerializeField] float dashDuration = 0.5f;
    //private bool isDashing = false;
    [SerializeField] LayerMask obstacleLayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        throwedAmmo = Instantiate(ammoModel);
        throwedAmmo.transform.parent = weaponObj;
        throwedAmmo.transform.localPosition = weaponOffset;
        throwedAmmo.transform.DOLocalRotate(Vector3.zero, 0.01f);
        throwedAmmo.GetComponent<AmmoScript>().player = gameObject;
        throwedAmmoRb = throwedAmmo.GetComponent<Rigidbody>();

        UpdateHealthBar();

        vibr = throwedAmmo.transform.DOShakePosition(0.3f, .1f);
    }

    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward)*800, Color.red);
        if (Input.GetMouseButtonDown(0) && isAmmoOnWeapon)
        {
            vibr = throwedAmmo.transform.DOShakePosition(0.08f, .01f).SetLoops(-1);
        }
        if (Input.GetMouseButtonUp(0) && isAmmoOnWeapon)
        {
            vibr.Kill();
            SoundManager.instance.PlayAudio("Whoosh");
            throwedAmmo.transform.position = weaponObj.position + weaponOffset;
            throwedAmmoRb.isKinematic = false;
            throwedAmmoRb.useGravity = false;
            throwedAmmo.transform.parent = null;
            RaycastHit raycastHit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out raycastHit, Mathf.Infinity))
            {
                throwedAmmo.transform.LookAt(raycastHit.point);
            }
            throwedAmmoRb.AddForce(weaponObj.forward * throwAmmoForce, ForceMode.Impulse);
            isAmmoOnWeapon = false;
        }
        if (Input.GetMouseButtonDown(1) && !isAmmoOnWeapon)
        {
            throwedAmmo.GetComponentInChildren<Collider>().enabled = true;
            throwedAmmoRb.useGravity = false;
            throwedAmmoRb.isKinematic = true;
            throwedAmmo.transform.parent = null;
            callingAmmo = true;
            throwedAmmo.GetComponent<AmmoScript>().OnGetBack();
        }
        if (callingAmmo)
        {
            throwedAmmo.transform.position = Vector3.MoveTowards(throwedAmmo.transform.position, weaponObj.position + weaponOffset, recallAmmoForce);
            //throwedAmmoRb.MovePosition(weaponObj.position + weaponOffset);
            if (throwedAmmo.transform.position == weaponObj.position + weaponOffset)
            {
                if(Vector3.Distance(throwedAmmo.transform.position, weaponObj.position + weaponOffset) < 12)SoundManager.instance.PlayClipAtPoint("Recall-Swish", transform.position, 1.0f, 1.0f);
                callingAmmo = false;
                isAmmoOnWeapon = true;
                throwedAmmo.transform.parent = weaponObj;
                throwedAmmo.transform.localEulerAngles = Vector3.zero;
                throwedAmmo.transform.localScale = Vector3.one;
            }
        }
        Debug.DrawRay(transform.position + Vector3.up*2, transform.forward * 50, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 50, Color.blue);
        Debug.DrawRay(transform.position + Vector3.up/2, transform.forward * 50, Color.blue);
        
    }

    //IEnumerator Dash()
    //{
    //    isDashing = true;

    //    GetComponent<CharacterController>().enabled = false;

    //    Vector3 endPos = transform.position + transform.forward * dashDistance;

    //    transform.GetComponent<Rigidbody>().DOMove(endPos, dashDuration).SetEase(Ease.InFlash).OnComplete(() =>
    //    {
    //        GetComponent<CharacterController>().enabled = true;
    //        isDashing = false;
    //    });

    //    yield return null;
    //}


    public void PlayerLifeCheck()
    {
        UpdateHealthBar();
        animator.SetLayerWeight(1, 0);
        if (lifePoint <= 0)
        {
            animator.CrossFade("Player Death", 0.3f);
            Camera.main.GetComponent<CameraScript>().ChangePlayerCameraFollowObj();
        }
        else
        {
            animator.CrossFade("Player Hit", 0.3f);
        }
    }

    // Animation Event
    public void ChangeeAnimationLayerWeight()
    {
        GetComponent<Animator>().SetLayerWeight(1, 1);
    }

    void UpdateHealthBar()
    {
        float fillAmount = (float)lifePoint / (float)maxLife;
        lifeBarImg.fillAmount = fillAmount;
    }
}
