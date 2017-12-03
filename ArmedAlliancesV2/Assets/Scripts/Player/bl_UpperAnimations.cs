using UnityEngine;
using System.Collections;

public class bl_UpperAnimations : MonoBehaviour {
    [HideInInspector]
    public bool m_Update = true;

    public Transform ShoulderR;
    public Transform ShouldeL;
    public Transform HeadBone;
    public bool TakeHeadInAnim = true;
    public string m_state;
    public string FireAnimation = "StandingFire";
    public string FirePistolAnim = "StandingPistolFire";
    public string FireKnifeAnim = "KnifeFire";
    [Space(5)]
    public string ReloadAnimation = "StandingReloadM4";
    [Space(5)]
    public string AimedAnimation = "StandingAim";
    public string IdleAnimation = "Idle";
    public string IdlePistolAnim = "StandingPistol";
    public string IdleKnifeAnim = "KnifeIdle";
    public string RunningAnim = "Standing";
    private bool isKnifeFire = false;
    public bl_Gun.weaponType m_WeaponType = bl_Gun.weaponType.Machinegun;

    private bool isIdle = false;
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        SetupAnimations();
    }
    /// <summary>
    /// When add a new animation configure same how this
    /// </summary>
    void SetupAnimations()
    {
        GetComponent<Animation>()[FireAnimation].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[FireAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[FireAnimation].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[FireAnimation].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[FireAnimation].layer = 3;
        GetComponent<Animation>()[FireAnimation].time = 0;
        GetComponent<Animation>()[FireAnimation].speed = 1.1f;
        GetComponent<Animation>()[FireAnimation].weight = 1f;
        //
        GetComponent<Animation>()[ReloadAnimation].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[ReloadAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[ReloadAnimation].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[ReloadAnimation].wrapMode = WrapMode.Once;
        GetComponent<Animation>()[ReloadAnimation].layer = 3;
        GetComponent<Animation>()[ReloadAnimation].time = 0;
        GetComponent<Animation>()[ReloadAnimation].speed = 0.9f;
        GetComponent<Animation>()[ReloadAnimation].weight = 1f;
        //
        GetComponent<Animation>()[AimedAnimation].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[AimedAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[AimedAnimation].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[AimedAnimation].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[AimedAnimation].layer = 3;
        GetComponent<Animation>()[AimedAnimation].time = 0;
        GetComponent<Animation>()[AimedAnimation].speed = 1.0f;
        GetComponent<Animation>()[AimedAnimation].weight = 1f;
        //
        GetComponent<Animation>()[IdleAnimation].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[IdleAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[IdleAnimation].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[IdleAnimation].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[IdleAnimation].layer = 3;
        GetComponent<Animation>()[IdleAnimation].time = 0;
        GetComponent<Animation>()[IdleAnimation].speed = 1.0f;
        GetComponent<Animation>()[IdleAnimation].weight = 1f;
        //
        GetComponent<Animation>()[FirePistolAnim].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[FirePistolAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[FirePistolAnim].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[FirePistolAnim].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[FirePistolAnim].layer = 3;
        GetComponent<Animation>()[FirePistolAnim].time = 0;
        GetComponent<Animation>()[FirePistolAnim].speed = 1.0f;
        GetComponent<Animation>()[FirePistolAnim].weight = 1f;
        //
        GetComponent<Animation>()[FireKnifeAnim].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[FireKnifeAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[FireKnifeAnim].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[FireKnifeAnim].wrapMode = WrapMode.PingPong;
        GetComponent<Animation>()[FireKnifeAnim].layer = 3;
        GetComponent<Animation>()[FireKnifeAnim].time = 0;
        GetComponent<Animation>()[FireKnifeAnim].speed = 1.0f;
        GetComponent<Animation>()[FireKnifeAnim].weight = 1f;
        //
        GetComponent<Animation>()[IdlePistolAnim].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[IdlePistolAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[IdlePistolAnim].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[IdlePistolAnim].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[IdlePistolAnim].layer = 3;
        GetComponent<Animation>()[IdlePistolAnim].time = 0;
        GetComponent<Animation>()[IdlePistolAnim].speed = 1.0f;
        GetComponent<Animation>()[IdlePistolAnim].weight = 1f;
        //
        GetComponent<Animation>()[IdleKnifeAnim].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[IdleKnifeAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[IdleKnifeAnim].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[IdleKnifeAnim].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[IdleKnifeAnim].layer = 3;
        GetComponent<Animation>()[IdleKnifeAnim].time = 0;
        GetComponent<Animation>()[IdleKnifeAnim].speed = 1.0f;
        GetComponent<Animation>()[IdleKnifeAnim].weight = 1f;
        //
        GetComponent<Animation>()[RunningAnim].AddMixingTransform(ShoulderR);
        GetComponent<Animation>()[RunningAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            GetComponent<Animation>()[RunningAnim].AddMixingTransform(HeadBone);
        }
        GetComponent<Animation>()[RunningAnim].wrapMode = WrapMode.Loop;
        GetComponent<Animation>()[RunningAnim].layer = 3;
        GetComponent<Animation>()[RunningAnim].time = 0;
        GetComponent<Animation>()[RunningAnim].speed = 1.0f;
        GetComponent<Animation>()[RunningAnim].weight = 1f;
    }


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!m_Update)
            return;
        if (isKnifeFire)
            return;

        if (m_state == "Firing")
        {
            if (m_WeaponType == bl_Gun.weaponType.Machinegun)
            {
                GetComponent<Animation>().CrossFade(FireAnimation, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Pistol)
            {
                GetComponent<Animation>().CrossFade(FirePistolAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Launcher)
            {
                GetComponent<Animation>().CrossFade(FireKnifeAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Knife)
            {
                GetComponent<Animation>().Rewind(FireKnifeAnim);
                GetComponent<Animation>()[FireKnifeAnim].time = 0;
                GetComponent<Animation>().Play(FireKnifeAnim);
                StartCoroutine(WaitKnife());
            }
            else//When you have more animation per example launcher Fire add "else if(m_WeaponType == bl_Gun.weaponType.Launcher o wherever"
            {
                GetComponent<Animation>().CrossFade(FireAnimation, 0.2f);
            }
            isIdle = false;
        }
        else if (m_state == "Reloading" && m_WeaponType != bl_Gun.weaponType.Launcher)
        {
            GetComponent<Animation>().CrossFade(ReloadAnimation, 0.2f);
            isIdle = false;
        }
        else if (m_state == "Aimed" && m_WeaponType != bl_Gun.weaponType.Launcher )
        {
            GetComponent<Animation>().CrossFade(AimedAnimation, 0.2f);
            isIdle = false;
        }
        else if (m_state == "Running")
        {
            GetComponent<Animation>().CrossFade(RunningAnim, 0.2f);
            isIdle = false;
        }
        else if(!isIdle)
        {
            if (m_WeaponType == bl_Gun.weaponType.Machinegun)
            {
                GetComponent<Animation>().CrossFade(IdleAnimation, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Pistol)
            {
                GetComponent<Animation>().CrossFade(IdlePistolAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Launcher)
            {
                GetComponent<Animation>().CrossFade(IdleKnifeAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Knife)
            {              
                GetComponent<Animation>().CrossFade(IdleKnifeAnim, 0.2f);
            }
            else//When you have more animation per example launcher Idle add "else if(m_WeaponType == bl_Gun.weaponType.Launcher o wherever"
            {
                GetComponent<Animation>().CrossFade(IdleAnimation, 0.2f);
            }

        }
        isIdle = true; ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_type"></param>
    public void ChangeType(bl_Gun.weaponType _type)
    {
        m_WeaponType = _type;
        isIdle = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitKnife()
    {
        isKnifeFire = true;
        yield return new WaitForSeconds(0.44f);
        isKnifeFire = false;
    }
}