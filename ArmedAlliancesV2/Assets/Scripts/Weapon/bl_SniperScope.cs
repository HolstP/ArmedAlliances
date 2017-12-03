using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class bl_SniperScope : MonoBehaviour {

    public Sprite Scope;
    /// <summary>
    /// Object to desactivate when is aimed
    /// </summary>
    public List<GameObject> OnScopeDisable = new List<GameObject>();
    public bool m_show_distance = true;
    /// <summary>
    /// maximum distance raycast
    /// </summary>
    public float Max_Distance = 1000;

    public float m_SmoothAppear = 12;
    //private
    private bl_Gun m_gun;
    private float m_alpha = 0;
    private Vector3 m_point = Vector3.zero;
    private float m_dist = 0.0f;
    private Image ScopeImage;
    private CanvasGroup Alpha;
    private Text DistanceText;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        m_gun = this.GetComponent<bl_Gun>();
        ScopeImage = GameObject.FindWithTag("GameManager").GetComponent<bl_Crosshair>().SniperScopeImage;
       
        if (ScopeImage)
        {
            ScopeImage.sprite = Scope;
            Alpha = ScopeImage.gameObject.GetComponent<CanvasGroup>();
            DistanceText = ScopeImage.gameObject.GetComponentInChildren<Text>();
        }
    }
    /// <summary>
    /// 
    /// </summary>
	/*void OnGUI() {
        if (Scope == null)
            return;

        GUI.depth = m_depth;
        if (m_gun.isAmed)
        {
            if (m_show_distance)
            {
                GetDistance();
            }
            //add a little fade in to avoid the impact of appearing once
            m_alpha = Mathf.Lerp(m_alpha, 1.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(false);
            }
            GUI.color = new Color(1, 1, 1, m_alpha);
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Scope);
            if (m_show_distance)
            {
                GUI.Label(new Rect(Screen.width / 2 + 25, Screen.height / 2 + 25, 100, 30), m_dist.ToString("0.0")+"<size=12>m</size>",style);
            }
        }
        else
        {
            m_alpha = Mathf.Lerp(m_alpha, 0.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(true);
            }
        }
	}*/


    void Update()
    {
        if (Scope == null || ScopeImage == null)
            return;

        if (m_gun.isAmed)
        {
            if (m_show_distance)
            {
                GetDistance();
            }
            //add a little fade in to avoid the impact of appearing once
            m_alpha = Mathf.Lerp(m_alpha, 1.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(false);
            }
        }
        else
        {
            m_alpha = Mathf.Lerp(m_alpha, 0.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(true);
            }
        }
        if(m_show_distance && DistanceText)
        {
            DistanceText.text = m_dist.ToString("00") + "<size=10>m</size>";
        }
        Alpha.alpha = m_alpha;

    }

    /// <summary>
    /// calculate the distance to the first object that raycast hits
    /// </summary>
    void GetDistance()
    {
        RaycastHit m_ray;
        Vector3 fwd = Camera.main.transform.forward;
        if (Physics.Raycast(Camera.main.transform.position, fwd, out m_ray, Max_Distance))
        {
            m_point = m_ray.point;
            m_dist = bl_UtilityHelper.GetDistance(m_point, Camera.main.transform.position);
        }
        else
        {
            m_dist = 0.0f;
        }
    }

}