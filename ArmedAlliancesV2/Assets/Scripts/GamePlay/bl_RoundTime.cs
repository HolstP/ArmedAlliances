using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using UnityEngine.UI;

public class bl_RoundTime : MonoBehaviour {

    public GUISkin Style;
    /// <summary>
    /// mode of the round room
    /// </summary>
    public RoundStyle m_RoundStyle;
    /// <summary>
    /// expected duration in round (automatically obtained)
    /// </summary>
   [HideInInspector] public int RoundDuration;
    private bl_GameManager m_Manager = null;
    [HideInInspector]
    public float CurrentTime;
    [System.Serializable]
    public class UI_
    {
        public Text TimeText;
    }
    public UI_ UI;
    //private
    private const string StartTimeKey = "RoomTime";       // the name of our "start time" custom property.
    private float m_Reference;
    private int m_countdown = 10;
    private bool isFinish = false;
    private bl_SettingPropiertis m_propiertis;
    private bl_RoomMenu RoomMenu;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            bl_UtilityHelper.LoadLevel(0);
            return;
        }

        m_Manager = GetComponent<bl_GameManager>();
        GetTime();
        m_propiertis = this.GetComponent<bl_SettingPropiertis>();
        RoomMenu = this.GetComponent<bl_RoomMenu>();
    }
    /// <summary>
    /// get the current time and verify if it is correct
    /// </summary>
    void GetTime()
    {
        RoundDuration = (int)PhotonNetwork.room.customProperties[PropiertiesKeys.TimeRoomKey];
        if (PhotonNetwork.isMasterClient)
        {
            m_Reference = (float)PhotonNetwork.time;

            Hashtable startTimeProp = new Hashtable();  // only use ExitGames.Client.Photon.Hashtable for Photon
            startTimeProp.Add(StartTimeKey, m_Reference);
            PhotonNetwork.room.SetCustomProperties(startTimeProp);
        }
        else
        {
            m_Reference = (float)PhotonNetwork.room.customProperties[StartTimeKey];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        float t_time = RoundDuration - ((float)PhotonNetwork.time - m_Reference);
        if (t_time > 0)
        {
            CurrentTime = t_time;
        }
        else if (t_time <= 0.001 && GetTimeServed == true)//Round Finished
        {
            CurrentTime = 0;
            
            bl_EventHandler.OnRoundEndEvent();
            if (!isFinish)
            {
                isFinish = true;
                RoomMenu.isFinish = true;
                InvokeRepeating("countdown", 1, 1);
            }
        }
        else//even if I do not photonnetwork.time then obtained to regain time
        {
            Refresh();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        GUI.skin = Style;
        //Display Time Round
        int normalSecons = 60;
        float remainingTime = Mathf.CeilToInt(CurrentTime);
        int m_Seconds = Mathf.FloorToInt(remainingTime % normalSecons);
        int m_Minutes = Mathf.FloorToInt((remainingTime / normalSecons) % normalSecons);
        string t_time = bl_UtilityHelper.GetTimeFormat(m_Minutes, m_Seconds);

        if (UI.TimeText != null)
        {
            UI.TimeText.text = string.Format(t_time);
        }

        if (isFinish)
        {
            if (m_RoundStyle == RoundStyle.OneMacht)
            {
                string rtText = "<size=15>Return to Lobby in</size><size=30><color=orange>" + m_countdown + "</color></size>";
                Vector2 rtlSize = GUI.skin.label.CalcSize(new GUIContent(rtText));
                GUI.Label(new Rect(Screen.width / 2 - rtlSize.x / 2, Screen.height / 2 + 25, rtlSize.x, 60), rtText);
            }
            else if (m_RoundStyle == RoundStyle.Rounds)
            {
                string rtText = "<size=15>Next Round in </size> <size=30><color=orange>" + m_countdown + "</color></size>";
                Vector2 rtlSize = GUI.skin.label.CalcSize(new GUIContent(rtText));
                GUI.Label(new Rect(Screen.width / 2 - rtlSize.x / 2, Screen.height / 2 + 25, rtlSize.x, 60), rtText);
            }
        }
    }

    public GUIStyle TimeStyle
    {
        get
        {
            if (Style != null)
            {
                return Style.customStyles[1];
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// with this fixed the problem of the time lag in the Photon
    /// </summary>
    void Refresh()
    {
        if (PhotonNetwork.isMasterClient)
        {
            m_Reference = (float)PhotonNetwork.time;

            Hashtable startTimeProp = new Hashtable();  // only use ExitGames.Client.Photon.Hashtable for Photon
            startTimeProp.Add(StartTimeKey, m_Reference);
            PhotonNetwork.room.SetCustomProperties(startTimeProp);
        }
        else
        {
            m_Reference = (float)PhotonNetwork.room.customProperties[StartTimeKey];
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void countdown()
    {
        m_countdown--;
        if (m_countdown <= 0)
        {
            FinishGame();
            CancelInvoke("countdown");
            m_countdown = 10;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void FinishGame()
    {
        bl_UtilityHelper.LockCursor(false);
        if (m_RoundStyle == RoundStyle.OneMacht)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                bl_UtilityHelper.LoadLevel(0);
            }
        }
        if (m_RoundStyle == RoundStyle.Rounds)
        {
            GetTime();
            if (m_propiertis)
            {
                m_propiertis.SettingPropiertis();
            }
            isFinish = false;          
            if (m_Manager == null)
                return;

                m_Manager.DestroyPlayer();
            if (RoomMenu != null)
            {
                RoomMenu.isFinish = false;
                RoomMenu.isPlaying = false;
                RoomMenu.showMenu = true;
                bl_UtilityHelper.LockCursor(false);
            }
        }
    }

    bool GetTimeServed
    {
        get
        {
            bool m_bool = false ;
            if (Time.timeSinceLevelLoad > 7)
            {
                m_bool = true;
            }
            return m_bool;
        }
    }

}