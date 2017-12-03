//////////////////////////////////////////////////////////////////
//bl_PhotonHelper.cs
//
//This a simple base class
//to us serve as an extension of Photon.Monobehaviour default
//                   Lovatto Studio
//////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class bl_PhotonHelper : Photon.PunBehaviour {

    protected GameMode mGameMode = GameMode.FFA;

    /// <summary>
    /// 
    /// </summary>
    public string myTeam
    {
        get
        {
            string t = (string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey];
            return t;
        }
    }
    /// <summary>
    /// Find a player gameobject by the viewID 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public GameObject FindPlayerRoot(int view)
    {
        PhotonView m_view = PhotonView.Find(view);

        if (m_view != null)
        {
            return m_view.gameObject;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    ///  get a photonView by the viewID
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public PhotonView FindPlayerView(int view)
    {
        PhotonView m_view = PhotonView.Find(view);

        if (m_view != null)
        {
            return m_view;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public PhotonView GetPhotonView(GameObject go)
    {
        PhotonView view = go.GetComponent<PhotonView>();
        if (view == null)
        {
            view = go.GetComponentInChildren<PhotonView>();
        }
        return view;
    }
    /// <summary>
    /// 
    /// </summary>
    public Transform Root
    {
        get
        {
            return transform.root;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Transform Parent
    {
        get
        {
            return transform.parent;
        }
    }

    /// <summary>
    /// True if the PhotonView is "mine" and can be controlled by this client.
    /// </summary>
    /// <remarks>
    /// PUN has an ownership concept that defines who can control and destroy each PhotonView.
    /// True in case the owner matches the local PhotonPlayer.
    /// True if this is a scene photonview on the Master client.
    /// </remarks>
    public bool isMine
    {
        get
        {
            return (this.photonView.ownerId == PhotonNetwork.player.ID) || (!this.photonView.isOwnerActive && PhotonNetwork.isMasterClient);
        }
    }
    /// <summary>
    /// Get Photon.connect
    /// </summary>
    public bool isConnected
    {
        get
        {
            return PhotonNetwork.connected;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public GameObject FindPhotonPlayer(PhotonPlayer p)
    {
        GameObject player = GameObject.Find(p.name);
        if (player == null)
        {
            return null;
        }
          return player;
    }
    /// <summary>
    /// Get the team of players
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public string GetTeam(PhotonPlayer p)
    {
        if (p == null || !isConnected)
            return null;

            string t = (string)p.customProperties[PropiertiesKeys.TeamKey];
            return t;
    }

    /// <summary>
    /// Get the team of players
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Team GetTeamEnum(PhotonPlayer p)
    {
        if (p == null || !isConnected)
            return Team.All;

        string t = (string)p.customProperties[PropiertiesKeys.TeamKey];
        
        switch (t)
        {
            case "Recon":
                return Team.Recon;
            case "Delta":
                return Team.Delta;
        }
        return Team.All;
    }
    /// <summary>
    /// Get current gamemode
    /// </summary>
    public GameMode GetGameMode
    {
        get
        {
            if (!isConnected || PhotonNetwork.room == null)
                return GameMode.TDM;

            GameMode[] result = (GameMode[])System.Enum.GetValues(typeof(GameMode));
            string gm = (string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey];
            for (int i = 0; i < result.Length; i++)
            {
                if(result[i].ToString() == gm)
                {
                    mGameMode = result[i];
                }
            }

            return mGameMode;
        }
    }

    public string LocalName
    {
        get
        {
            if (PhotonNetwork.player != null && isConnected)
            {
                string n = PhotonNetwork.player.name;
                return n;
            }
            else
            {
                return "None";
            }
        }
    }

    /// <summary>
    /// Get All Player in Room
    /// for get this hash 00xki8697
    /// </summary>
    public List<PhotonPlayer> AllPlayerList
    {
        get
        {
            List<PhotonPlayer> p = new List<PhotonPlayer>();

            foreach (PhotonPlayer pp in PhotonNetwork.playerList)
            {
                p.Add(pp);
            }
            return p;
        }
    }
    /// <summary>
    /// Get All Player in Room of a specific team
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<PhotonPlayer> GetPlayersInTeam(string team)
    {
        List<PhotonPlayer> p = new List<PhotonPlayer>();

        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if ((string)pp.customProperties[PropiertiesKeys.TeamKey] == team)
            {
                p.Add(pp);
            }
        }
        return p;
    }
}