using UnityEngine;

public class bl_SpawnPoint : bl_PhotonHelper {

    public Team m_Team = Team.All;
    public float SpawnSpace = 3f;
    // Use this for initialization
    void Start()
    {
        if (this.transform.GetComponent<Renderer>() != null)
        {
            this.GetComponent<Renderer>().enabled = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        bl_UtilityHelper.GetGameManager.RegisterSpawnPoint(this);
    }

    //Debug Spawn Spcae
    void OnDrawGizmos()
    {
        Color c = (m_Team == Team.Recon) ? ColorKeys.ReconColor : ColorKeys.DeltaColor;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, SpawnSpace);
        Gizmos.color = new Color(c.r,c.g,c.b,0.4f);
        Gizmos.DrawSphere(transform.position, SpawnSpace);
    }
}
