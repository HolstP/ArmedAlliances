using UnityEngine;
using System.Collections;

public class bl_FootSteps : bl_PhotonHelper
{
    [HideInInspector]
    public bool CanUpdate = true;
    public bool CanSyncSteps = true;
    [Header("Sounds Lists")]
    public AudioClip[] m_dirtSounds;
    public AudioClip[] m_concreteSounds;
    public AudioClip[] m_WoodSounds;
    public AudioClip[] m_WaterSounds;
    [Header("Settings")]
    public float m_minSpeed = 2f;
    public float m_maxSpeed = 8f;
    public float audioStepLengthCrouch = 0.65f;
    public float audioStepLengthWalk = 0.45f;
    public float audioStepLengthRun = 0.25f;
    public float audioVolumeCrouch = 0.3f;
    public float audioVolumeWalk = 0.4f;
    public float audioVolumeRun = 1.0f;
    [Space(5)]
    public AudioSource InstanceReference;
    public PhotonView m_view;
    //private
    private bool isStep = true;
    private string m_MaterialHit;
    private Vector3 LastPost = Vector3.zero;
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!CanUpdate)//if remote is not updated, only receives the RPC call (optimization helps)
            return;
        if (InstanceReference == null)
            return;
        if (!isStep)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10))
        {
            m_MaterialHit = hit.collider.transform.tag;
        }
        float m_magnitude = m_charactercontroller.velocity.magnitude;
        if (m_charactercontroller.isGrounded && isStep)
        {
            if (m_magnitude < m_minSpeed && m_magnitude > 0.75f)
            {
                if (CanSyncSteps && !PhotonNetwork.offlineMode)
                {
                    m_view.RPC("SyncSteps", PhotonTargets.Others, "Crouch", m_MaterialHit);
                }
                SyncSteps("Crouch", m_MaterialHit);
            }
            else if (m_magnitude > m_minSpeed && m_magnitude < m_maxSpeed)
            {
                if (CanSyncSteps && !PhotonNetwork.offlineMode)
                {
                    m_view.RPC("SyncSteps", PhotonTargets.Others, "Walk", m_MaterialHit);
                }
                SyncSteps("Walk", m_MaterialHit);
            }
            else if (m_magnitude > m_maxSpeed)
            {
                if (CanSyncSteps && !PhotonNetwork.offlineMode)
                {
                    m_view.RPC("SyncSteps", PhotonTargets.Others, "Run", m_MaterialHit);
                }
                SyncSteps("Run", m_MaterialHit);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m_material"></param>
    /// <returns></returns>
    IEnumerator Crouch(string m_material)
    {
        if (InstanceReference.GetComponent<AudioSource>() == null)
            yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
                bl_UtilityHelper.PlayClipAtPoint(m_dirtSounds[Random.Range(0, m_dirtSounds.Length)], LastPost, audioVolumeCrouch, InstanceReference);
                break;
            case "Concrete":
                bl_UtilityHelper.PlayClipAtPoint(m_concreteSounds[Random.Range(0, m_concreteSounds.Length)], LastPost, audioVolumeCrouch, InstanceReference);
                break;
            case "Wood":
                bl_UtilityHelper.PlayClipAtPoint(m_WoodSounds[Random.Range(0, m_WoodSounds.Length)], LastPost, audioVolumeCrouch, InstanceReference);
                break;
            case "Water":
                bl_UtilityHelper.PlayClipAtPoint(m_WaterSounds[Random.Range(0, m_WaterSounds.Length)], LastPost, audioVolumeCrouch, InstanceReference);
                break;

        }
        yield return new WaitForSeconds(audioStepLengthCrouch);
        isStep = true;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="m_material"></param>
    /// <returns></returns>
    IEnumerator Walk(string m_material)
    {
        if (InstanceReference.GetComponent<AudioSource>() == null)
            yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
                bl_UtilityHelper.PlayClipAtPoint(m_dirtSounds[Random.Range(0, m_dirtSounds.Length)], LastPost, audioVolumeWalk, InstanceReference);
                break;
            case "Concrete":
                bl_UtilityHelper.PlayClipAtPoint(m_concreteSounds[Random.Range(0, m_concreteSounds.Length)], LastPost, audioVolumeWalk, InstanceReference);
                break;
            case "Wood":
                bl_UtilityHelper.PlayClipAtPoint(m_WoodSounds[Random.Range(0, m_WoodSounds.Length)], LastPost, audioVolumeWalk, InstanceReference);
                break;
            case "Water":
                bl_UtilityHelper.PlayClipAtPoint(m_WaterSounds[Random.Range(0, m_WaterSounds.Length)], LastPost, audioVolumeWalk, InstanceReference);
                break;

        }
        yield return new WaitForSeconds(audioStepLengthWalk);
        isStep = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m_material"></param>
    /// <returns></returns>
    IEnumerator Run(string m_material)
    {
        if (InstanceReference.GetComponent<AudioSource>() == null)
            yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
                bl_UtilityHelper.PlayClipAtPoint(m_dirtSounds[Random.Range(0, m_dirtSounds.Length)], LastPost, audioVolumeRun, InstanceReference);
                break;
            case "Concrete":
                bl_UtilityHelper.PlayClipAtPoint(m_concreteSounds[Random.Range(0, m_concreteSounds.Length)], LastPost, audioVolumeRun, InstanceReference);
                break;
            case "Wood":
                bl_UtilityHelper.PlayClipAtPoint(m_WoodSounds[Random.Range(0, m_WoodSounds.Length)], LastPost, audioVolumeRun, InstanceReference);
                break;
            case "Water":
                bl_UtilityHelper.PlayClipAtPoint(m_WaterSounds[Random.Range(0, m_WaterSounds.Length)], LastPost, audioVolumeRun, InstanceReference);
                break;

        }

        yield return new WaitForSeconds(audioStepLengthRun);
        isStep = true;
    }

    [PunRPC]
    void SyncSteps(string t_corrutine, string m_material, PhotonMessageInfo m_info)
    {
        if (m_info.sender != null)
        {
            if (m_info.sender.name == gameObject.name)
            {
                GameObject player = FindPlayerRoot(m_info.photonView.viewID);
                LastPost = player.transform.position;
                StartCoroutine(t_corrutine, m_material);
            }
        }
        else
        {
            LastPost = this.InstanceReference.transform.position;
            StartCoroutine(t_corrutine, m_material);
        }
    }

    void SyncSteps(string t_corrutine, string m_material)
    {
        LastPost = this.InstanceReference.transform.position;
        StartCoroutine(t_corrutine, m_material);

    }

    public void OffUpdate()
    {
        CanUpdate = false;
    }
    public CharacterController m_charactercontroller
    {
        get
        {
            return this.transform.root.GetComponent<CharacterController>();
        }
    }
}