using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using UnityEngine.UI;

public class bl_SettingPropiertis : MonoBehaviour
{

    [Header("Settings")]
    public GameMode m_GameMode = GameMode.FFA;
    [Header("References")]
    public GameObject CTFObjects;
    public Texture2D FFABox;
    public GUIStyle Style;
    public UI_ UI;

    //Private
    private int Team_1_Score;
    private int Team_2_Score;
    private float UpdatePing = 5;//Update Player Ping each 5s
    private bool isFinish = false;
    private string FinalRoundText = string.Empty;
    private bl_RoomMenu RMenu;
    private bl_RoundTime RTime;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if (!PhotonNetwork.connected || PhotonNetwork.room == null)
            return;

        RMenu = base.GetComponent<bl_RoomMenu>();
        RTime = base.GetComponent<bl_RoundTime>();
        SettingPropiertis();
        GetRoomInfo();
    }
    /// <summary>
    /// 
    /// </summary>
    public void SettingPropiertis()
    {
        //Initialize new properties where the information will stay Room
        if (PhotonNetwork.isMasterClient)
        {
            Hashtable setTeamScore = new Hashtable();
            setTeamScore.Add(PropiertiesKeys.Team1Score, 0);
            PhotonNetwork.room.SetCustomProperties(setTeamScore);

            Hashtable setTeam2Score = new Hashtable();
            setTeam2Score.Add(PropiertiesKeys.Team2Score, 0);
            PhotonNetwork.room.SetCustomProperties(setTeam2Score);
        }
        //Initialize new properties where the information will stay Players
        Hashtable PlayerTeam = new Hashtable();
        PlayerTeam.Add(PropiertiesKeys.TeamKey, Team.All.ToString());
        PhotonNetwork.player.SetCustomProperties(PlayerTeam);

        Hashtable PlayerKills = new Hashtable();
        PlayerKills.Add(PropiertiesKeys.KillsKey, 0);
        PhotonNetwork.player.SetCustomProperties(PlayerKills);

        Hashtable PlayerDeaths = new Hashtable();
        PlayerDeaths.Add(PropiertiesKeys.DeathsKey, 0);
        PhotonNetwork.player.SetCustomProperties(PlayerDeaths);

        Hashtable PlayerScore = new Hashtable();
        PlayerScore.Add(PropiertiesKeys.ScoreKey, 0);
        PhotonNetwork.player.SetCustomProperties(PlayerScore);

        Hashtable PlayerPing = new Hashtable();
        PlayerPing.Add(PropiertiesKeys.Ping, 0);
        PhotonNetwork.player.SetCustomProperties(PlayerPing);
    }
    /// <summary>
    /// 
    /// </summary>
    void GetRoomInfo()
    {
        if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.FFA.ToString())
        {
            m_GameMode = GameMode.FFA;
            CTFObjects.SetActive(false);
            UI.FFAScoreRoom.SetActive(true);
            UI.TDMScoreRoom.SetActive(false);
        }
        else if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.TDM.ToString())
        {
            m_GameMode = GameMode.TDM;
            CTFObjects.SetActive(false);
            UI.TDMScoreRoom.SetActive(true);
            UI.FFAScoreRoom.SetActive(false);
        }
        else if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.CTF.ToString())
        {
            m_GameMode = GameMode.CTF;
            CTFObjects.SetActive(true);
            UI.TDMScoreRoom.SetActive(true);
            UI.FFAScoreRoom.SetActive(false);
        }

        //
        if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.RoomRoundKey] == "1")
        {
            RTime.m_RoundStyle = RoundStyle.Rounds;
        }
        else
        {
            RTime.m_RoundStyle = RoundStyle.OneMacht;
        }
        if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.TeamSelectionKey] == "1")
        {
            RMenu.AutoTeamSelection = true;
        }
        else
        {
            RMenu.AutoTeamSelection = false;
        }
        if (PlayerPrefs.HasKey("quality"))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));
        }
        if (PlayerPrefs.HasKey("volumen"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("volumen");
        }
        if (PlayerPrefs.HasKey("anisotropic"))
        {
            int i = PlayerPrefs.GetInt("anisotropic");
            if (i == 0)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            }
            else if (i == 1)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            }
            else
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        InvokeRepeating("GetPing", 1, UpdatePing);
        bl_EventHandler.OnRoundEnd += this.OnRoundEnd;
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        CancelInvoke("GetPing");
        bl_EventHandler.OnRoundEnd -= this.OnRoundEnd;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (m_GameMode != GameMode.FFA)
        {
            //Room Score for TDM
            if (PhotonNetwork.room != null)
            {
                //Room Score for TDM
                if (PhotonNetwork.room != null)
                {
                    Team_1_Score = PhotonNetwork.room.GetRoomScore(Team.Delta);
                    Team_2_Score = PhotonNetwork.room.GetRoomScore(Team.Recon);

                    if (UI.DeltaScoreText != null)
                    {
                        UI.DeltaScoreText.text = bl_UtilityHelper.GetThreefoldChar(Team_1_Score) + "\n <size=9>Delta</size>";
                    }
                    if (UI.ReconScoreText != null)
                    {
                        UI.ReconScoreText.text = bl_UtilityHelper.GetThreefoldChar(Team_2_Score) + "\n <size=9>Recon</size>";
                    }
                }
            }
        }
        else
        {
            string t_PlayerStart = m_Menu.PlayerStar;
            UI.StarPlayerText.text = "PLAYER START:\n<size=14><color=black>" + t_PlayerStart + "</color></size>";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        if (isFinish) { FinalUI(); }
    }
    /// <summary>
    /// configure your custom Final GUI
    /// </summary>
    void FinalUI()
    {
        GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 2 - 25, 250, 50), "<size=25><color=" + ColorKeys.MFPSOrange + ">" + FinalRoundText + "</color></size> Won Match", FFA_Style);
    }
    /// <summary>
    /// Get Curren Player Ping and Update Info for other Players
    /// </summary>
    void GetPing()
    {
        int Ping = PhotonNetwork.GetPing();

        Hashtable PlayerPing = new Hashtable();
        PlayerPing.Add(PropiertiesKeys.Ping, Ping);
        PhotonNetwork.player.SetCustomProperties(PlayerPing);
        if (m_Menu != null)
        {
            if (Ping > m_Menu.MaxPing)
            {
                m_Menu.ShowWarningPing = true;
            }
            else
            {
                m_Menu.ShowWarningPing = false;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void OnRoundEnd()
    {
        isFinish = true;
        StartCoroutine(DisableUI());
        if (m_GameMode != GameMode.FFA)
        {
            if (Team_1_Score > Team_2_Score)
            {
                FinalRoundText = "Delta";
            }
            else if (Team_1_Score < Team_2_Score)
            {
                FinalRoundText = "Recon";
            }
            else if (Team_1_Score == Team_2_Score)
            {
                FinalRoundText = "No one";
            }
        }
        else if (m_GameMode == GameMode.FFA)
        {
            FinalRoundText = m_Menu.PlayerStar;
        }
    }

    public GUIStyle FFA_Style
    {
        get
        {
            GUIStyle t_style = new GUIStyle();
            t_style.font = Style.font;
            t_style.normal.textColor = Style.normal.textColor;
            t_style.fontSize = Style.fontSize;
            t_style.alignment = Style.alignment;
            t_style.normal.background = FFABox;
            t_style.richText = true;
            return t_style;
        }
    }
    public bl_RoomMenu m_Menu
    {
        get
        {
            if (GetComponent<bl_RoomMenu>() != null)
            {
                return GetComponent<bl_RoomMenu>();
            }
            else
            {
                return null;
            }
        }
    }
    IEnumerator DisableUI()
    {
        yield return new WaitForSeconds(10);
        isFinish = false;
    }

    [System.Serializable]
    public class UI_
    {
        public GameObject TDMScoreRoom;
        public GameObject FFAScoreRoom;
        public Text DeltaScoreText;
        public Text ReconScoreText;
        public Text StarPlayerText;
    }
}