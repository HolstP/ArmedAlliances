using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class bl_NetworkGun : MonoBehaviour
{

    public Transform FirePoint;
    public GameObject m_bullet;
    public GameObject m_muzzleFlash;
    public Renderer DesactiveOnOffAmmo;
    [Space(5)]
    public bool TakeLocalFirePoint = true;
    /// <summary>
    /// This asigne auto not need asigne in inspector
    /// </summary>
    public int WeaponID;
    /// <summary>
    /// Local Gun reference (the same Gun but local)
    /// </summary>
    public bl_Gun IsGun;
    /// <summary>
    /// The type of gun wath is this (signe auto)
    /// </summary>
    public bl_Gun.weaponType m_weaponType = bl_Gun.weaponType.Machinegun;
    //private
    private bl_UpperAnimations m_upper;
    private Material defaultMat;

    void Awake()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        m_upper = this.transform.root.GetComponentInChildren<bl_UpperAnimations>();
        if (DesactiveOnOffAmmo)
        {
            defaultMat = DesactiveOnOffAmmo.material;
        }
        if (IsGun != null)
        {
            WeaponID = IsGun.GunID;
            m_weaponType = IsGun.typeOfGun;
        }
        else
        {
            Debug.LogError("This NetworkGun No have reference of GUN");
        }
    }
    /// <summary>
    /// Update type each is enable 
    /// </summary>
    void OnEnable()
    {
        SetUpType();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetUpType()
    {
        if (m_upper != null)
        {
            m_upper.ChangeType(m_weaponType);
        }
        else
        {
            if (transform.root.GetComponentInChildren<bl_UpperAnimations>() == null)
            {
                Debug.LogError("No have bl_UpperAnimations.cs attach in this player");
            }
            else
            {
                m_upper = this.transform.root.GetComponentInChildren<bl_UpperAnimations>();
                m_upper.ChangeType(m_weaponType);
            }
        }
    }

    /// <summary>
    /// Fire Sync in network player
    /// </summary>
    public void Fire(float m_spread,Vector3 pos,Quaternion rot)
    {
        if (IsGun != null)
        {
            //bullet info is set up in start function
            GameObject newBullet = Instantiate(m_bullet, pos, rot) as GameObject; // create a bullet
            // set the gun's info into an array to send to the bullet
            bl_BulletInitSettings t_info = new bl_BulletInitSettings();
            t_info.m_damage = 0;
            t_info.m_ImpactForce = 0;
            t_info.m_MaxPenetration = 0;
            t_info.m_maxspread = IsGun.maxSpread;
            t_info.m_spread = m_spread;
            t_info.m_speed = IsGun.bulletSpeed;
            t_info.m_weaponname = "";
            t_info.m_position = this.transform.root.position;
            t_info.isNetwork = true;

            newBullet.GetComponent<bl_Bullet>().SetUp(t_info);
            newBullet.GetComponent<bl_Bullet>().isTracer = true;
            GetComponent<AudioSource>().clip = IsGun.FireSound;
            GetComponent<AudioSource>().spread = Random.Range(1.0f, 1.5f);
            GetComponent<AudioSource>().Play();
        }
        if (m_muzzleFlash)
        {
            StartCoroutine(MuzzleFlash());
        }
    }
    /// <summary>
    /// if grenade 
    /// </summary>
    /// <param name="s"></param>
    public void GrenadeFire(float s)
    {
        if (IsGun != null)
        {
            Vector3 position = FirePoint.position; // position to spawn bullet is at the muzzle point of the gun       
            //bullet info is set up in start function
            GameObject newBullet = Instantiate(m_bullet, position, FirePoint.rotation) as GameObject; // create a bullet
            // set the gun's info into an array to send to the bullet
            bl_BulletInitSettings t_info = new bl_BulletInitSettings();
            t_info.m_damage = 0;
            t_info.m_ImpactForce = 0;
            t_info.m_MaxPenetration = 0;
            t_info.m_maxspread = IsGun.maxSpread;
            t_info.m_spread = s;
            t_info.m_speed = IsGun.bulletSpeed;
            t_info.m_weaponname = "";
            t_info.m_position = this.transform.root.position;
            t_info.isNetwork = true;

            newBullet.GetComponent<bl_Grenade>().SetUp(t_info);
            GetComponent<AudioSource>().clip = IsGun.FireSound;
            GetComponent<AudioSource>().spread = Random.Range(1.0f, 1.5f);
            GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// When is knife only reply sounds
    /// </summary>
    public void KnifeFire()
    {
        if (IsGun != null)
        {
            GetComponent<AudioSource>().clip = IsGun.FireSound;
            GetComponent<AudioSource>().spread = Random.Range(1.0f, 1.5f);
            GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    public void DesactiveGrenade(bool active,Material mat)
    {
        if(m_weaponType != bl_Gun.weaponType.Launcher)
        {
            Debug.LogError("Gun type is not grenade, can't desactive it");
            return;
        }
        //when hide netgun / grenade we use method of change material to a invicible
        //due that if desative the render cause animation  player broken.
        if (DesactiveOnOffAmmo != null)
        {
            DesactiveOnOffAmmo.material = (active) ? defaultMat : mat;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator MuzzleFlash()
    {
        m_muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.04f);
        m_muzzleFlash.SetActive(false);

    }
}