using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using RootMotion.FinalIK;

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
    bool isWeaponEquipped = false;

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
        animator.SetLayerWeight(1, 0);
        weaponObj.gameObject.SetActive(false);
        animator.SetBool("isWeaponEquipped", false);
        GetComponent<AimIK>().enabled = false;
    }

    void Update()
    {
        if (!isWeaponEquipped && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 800))
            {
                if (hitInfo.transform.CompareTag("Weapon"))
                {
                    weaponObj.gameObject.SetActive(true);
                    hitInfo.transform.gameObject.SetActive(false);
                    animator.SetLayerWeight(1, 1);
                    animator.SetBool("isWeaponEquipped", true);
                    GetComponent<AimIK>().enabled = true;
                }
            }
        }

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
            
            throwedAmmo.GetComponentInChildren<Collider>().enabled = false;
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
                if (Vector3.Distance(throwedAmmo.transform.position, weaponObj.position + weaponOffset) < 12) SoundManager.instance.PlayClipAtPoint("Recall-Swish", transform.position, 1.0f, 1.0f);
                callingAmmo = false;
                isAmmoOnWeapon = true;
                throwedAmmo.GetComponentInChildren<Collider>().enabled = true;
                throwedAmmo.transform.parent = weaponObj;
                throwedAmmo.transform.localEulerAngles = Vector3.zero;
                throwedAmmo.transform.localScale = Vector3.one;
            }
        }

    }



    public void TakeDamage(int damage)
    {
        lifePoint -= damage;

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


    public void TakeHeal(int heal)
    {
        lifePoint += heal;

        UpdateHealthBar();
    }

    // Animation Event
    public void ChangeAnimationLayerWeight()
    {
        if (isWeaponEquipped)
        {
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    void UpdateHealthBar()
    {
        float fillAmount = (float)lifePoint / (float)maxLife;
        lifeBarImg.fillAmount = fillAmount;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("RocknRoll"))
        {
            Vector3 collisionForce = collision.impulse / Time.fixedDeltaTime;
            Debug.Log(collisionForce.magnitude);
            if (collisionForce.magnitude > 20000)
            {
                TakeDamage(9999);
            }
        }
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Key"))
        {
            other.transform.parent.GetComponent<Animator>().CrossFade("UnlockDoor", 0.3f);
            Destroy(other.gameObject, 0.3f);
        }
    
        if (other.transform.CompareTag("Apple"))
        {
            TakeHeal(20);
            Destroy(other.gameObject);
        }
    }
}
