///////////////////////////////////////////////////////////////////////////////////////
// bl_OnVisible.cs
//
// This script helps to optimize the game as it makes the players remote script
// only run when they are viewed by our player.
//                             Lovatto Studio
///////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class bl_OnVisible : MonoBehaviour {

    public bl_PlayerAnimations NPA;
    public bl_UpperAnimations Upper;
    /// <summary>
    /// when not is visible
    /// </summary>
    void OnBecameInvisible()
    {
        NPA.m_Update = false;
        Upper.m_Update = false;
    }
    /// <summary>
    /// When is visible
    /// </summary>
    void OnBecameVisible()
    {
        NPA.m_Update = true;
        Upper.m_Update = true;
    }
}
