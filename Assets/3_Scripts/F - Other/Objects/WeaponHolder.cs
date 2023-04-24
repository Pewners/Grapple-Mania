 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using DG.Tweening;

// weapon Holder -> Moved to sway
// weapon -> Moved for reload, away, ads and normal
// model -> Moved for recoil
public class WeaponHolder : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 8;
    [SerializeField] private float multiplier = 2;

    [Header("Different Blasters")]
    public UltimateBlaster blaster;
    public List<UltimateBlaster> allBlasters;

    public UltimateBlaster.BlasterType blasterType;

    public AnimationCurve weaponRecoilCurve;

    [Header("Rigging & Animation")]
    public Transform camT;
    public Transform aimConstraintTarget;
    public LayerMask whatIsObject;

    public WeaponPos weaponPos;
    public WeaponPos weaponPosTarget;
    public float transitionValue;
    private float remainingTransitionTime;
    public enum WeaponPos
    {
        Away,
        Reload,
        Normal,
        Ads
    }

    // transition times
    private float adsTime;
    private float readyTime;
    private float awayTime;

    private Transform targetTransform;

    private PlayerInput input;
    private PlayerCam_MLab cam;
    private Inventory inv;

    private float setupTime = 0.25f;
    private bool settingUp = true;

    // public bool swayWithWalking = false;


    Quaternion startRotationOffset = Quaternion.identity;
    private void Start()
    {
        input = PlayerReferences.instance.input;
        cam = PlayerReferences.instance.cam;
        inv = PlayerReferences.instance.inventory;

        inv.OnWeaponEquipped += SetTransitionTimes;

        startRotationOffset = transform.localRotation;

        Invoke(nameof(Setup), setupTime);
    }

    private void Setup()
    {
        settingUp = false;
        //StartTransition(WeaponPos.Normal);
    }

    private void Update()
    {
        // weapon sway
        Sway();

        // move and rotate weapon


        RaycastHit hit;
        if(Physics.Raycast(camT.position, camT.forward, out hit, 999f, whatIsObject))
        {
            // aimConstraintTarget.position = hit.point;
        }

        if (remainingTransitionTime >= 0)
            remainingTransitionTime -= Time.deltaTime;
    }

    private void Sway()
    {
        // get mouse input
        float mouseX = input.mouseX * multiplier;
        float mouseY = input.mouseY * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY * startRotationOffset;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void PlayWeaponRecoil(float rangeOfRecoil, float recoveryTime)
    {
        if (blaster.lockWeaponModelRecoil)
            return;

        if(rangeOfRecoil > 1)
        {
            Debug.LogError("Range of recoil can't be greater then 1");
            rangeOfRecoil = 1;
        }

        DOVirtual.Float(0f, 1f, recoveryTime, v =>
        {
            float t = weaponRecoilCurve.Evaluate(v) * rangeOfRecoil;
            blaster.weaponModelT.localPosition = Vector3.Lerp(blaster.GetNormalLocalPos(), blaster.weaponModelRecoilPos.localPosition, t);
            blaster.weaponModelT.localRotation = Quaternion.Lerp(blaster.GetNormalLocalRot(), blaster.weaponModelRecoilPos.localRotation, t);
        });
    }

    Tweener tweener;
    public float StartTransition(WeaponPos targetPos)
    {
        if (settingUp) return 0;
        if (blaster == null) return 0;

        weaponPosTarget = targetPos;

        Vector3 localTargetPos = GetLocalPosition(targetPos);
        Vector3 localTargetRot = GetLocalRotation(targetPos).eulerAngles;

        print("Getting TransitionTime from: " + transitionValue + " to " + (int)targetPos);
        float transitionTime = GetTransitionTime(transitionValue, (int)targetPos);

        remainingTransitionTime = transitionTime; /// count down in void Update

        blaster.weaponT.DOKill();
        blaster.weaponT.DOLocalMove(localTargetPos, transitionTime);
        blaster.weaponT.DOLocalRotate(localTargetRot, transitionTime);

        float startTransitionValue = transitionValue;
        tweener.Kill();
        tweener = DOVirtual.Float(startTransitionValue, (int)targetPos, transitionTime, v =>
        {
            transitionValue = v;
            print("transitionValue: " + transitionValue);
        }).OnComplete(() => { weaponPos = weaponPosTarget; });

        return transitionTime;
    }

    public float GetTransitionTime(WeaponPos targetPos)
    {
        return GetTransitionTime(transitionValue, (int)targetPos);
    }
    private float GetTransitionTime(float startPoint, int endPoint)
    {
        if(startPoint == endPoint)
            return 0;

        bool transitioningUp = endPoint > startPoint;

        int previousPoint = transitioningUp ? Mathf.FloorToInt(startPoint) : Mathf.CeilToInt(startPoint);

        float differenceStartAndPrevious = Mathf.Abs(startPoint - previousPoint);
        float firstTransitionMultiplier = transitioningUp ? 1 - differenceStartAndPrevious : 1 - differenceStartAndPrevious;

        int stepsBetweenTransitionPoints = Mathf.CeilToInt(Mathf.Abs(endPoint - startPoint));

        print("txt up" + transitioningUp + "steps" + stepsBetweenTransitionPoints + "prev" + previousPoint + "mult" + firstTransitionMultiplier);

        float transitionTime = 0;

        int safety = 0;

        // transitioning up
        if (transitioningUp)
        {
            for (int i = previousPoint; i < previousPoint + stepsBetweenTransitionPoints; i++)
            {
                // first transition
                if (i == previousPoint) transitionTime += GetSubTransitionTime(i, transitioningUp) * firstTransitionMultiplier;
                // all others (full)
                else transitionTime += GetSubTransitionTime(i, transitioningUp);

                safety++;
                if (safety >= 10) return -1;
            }
        }

        // transitioning down
        if (!transitioningUp)
        {
            for (int i = previousPoint; i > previousPoint - stepsBetweenTransitionPoints; i--)
            {
                // first transition
                if (i == previousPoint) transitionTime += GetSubTransitionTime(i, transitioningUp) * firstTransitionMultiplier;
                // all others (full)
                else transitionTime += GetSubTransitionTime(i, transitioningUp);

                safety++;
                if (safety >= 10) return -1;
            }
        }

        return transitionTime;
    }

    // the fixed transition times according to the weapon stats
    // away - 0, reload - 1, normal - 2, ads - 3
    /// subtransition 0 = 0 -> 1 = 1/2 of readyTime or awayTime
    /// subtransition 1 = 1 -> 2 = 1/2 of readyTime or awayTime
    /// subtransition 2 = 2 -> 3 = adsTime
    private float GetSubTransitionTime(int transitionStart, bool up)
    {
        // away
        if (transitionStart == 0)
        {
            if (up) return readyTime * 0.5f;
        }

        // reload
        else if (transitionStart == 1)
        {
            if (up) return readyTime * 0.5f;
            else return awayTime * 0.5f;
        }

        // normal
        else if (transitionStart == 2)
        {
            if (up) return adsTime;
            else return awayTime * 0.5f;
        }

        // ads
        else if (transitionStart == 3)
        {
            if (!up) return adsTime;
        }

        Debug.LogError("Subtransition index out of defined range -> " + transitionStart + up);
        return 0;
    }

    public Vector3 GetLocalPosition(WeaponPos weaponPos)
    {
        if (weaponPos == WeaponPos.Normal) return blaster.GetNormalLocalPos();
        else if (weaponPos == WeaponPos.Ads) return blaster.GetAdsLocalPos();
        else if (weaponPos == WeaponPos.Reload) return blaster.GetReloadLocalPos();
        else if (weaponPos == WeaponPos.Away) return blaster.GetAwayLocalPos();
        else return Vector3.zero;
    }

    public Quaternion GetLocalRotation(WeaponPos weaponPos)
    {
        if (weaponPos == WeaponPos.Normal) return blaster.GetNormalLocalRot();
        else if (weaponPos == WeaponPos.Ads) return blaster.GetAdsLocalRot();
        else if (weaponPos == WeaponPos.Reload) return blaster.GetReloadLocalRot();
        else if (weaponPos == WeaponPos.Away) return blaster.GetAwayLocalRot();
        else return Quaternion.identity;
    }

    #region Setters

    public void SwitchBlaster(UltimateBlaster.BlasterType blasterType)
    {
        print("switching to blasterType " + blasterType);
        if(blaster != null)
            blaster.gameObject.SetActive(false);

        for (int i = 0; i < allBlasters.Count; i++)
        {
            if (allBlasters[i].blasterType == blasterType)
            {
                blaster = allBlasters[i];
                blaster.gameObject.SetActive(true);
            }
        }
    }

    public void SetTransitionTimes(RangedWeapon weapon)
    {
        adsTime = weapon.handling.adsTime;
        readyTime = weapon.handling.readyTime;
        awayTime = weapon.handling.awayTime;
    }

    #endregion


    #region Getters

    public UltimateBlaster GetCurrBlaster() { return blaster; }

    public Transform GetCurrAttackPointTransform() { return blaster.attackPoint; }

    public Vector3 GetCurrAttackPoint() { return blaster.attackPoint.position; }

    public float GetAdsProgress()
    {
        if (transitionValue > 2) return transitionValue - 2;
        else return 0;
    }

    #endregion
}
