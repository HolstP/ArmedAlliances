/////////////////////////////////////////////////////////////////////////////////
/////////////////////////////bl_UtilityHelper.cs/////////////////////////////////
///////This is a helper script that contains multiple and useful functions///////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5_3|| UNITY_5_4 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class bl_UtilityHelper
{

    public static void LoadLevel(string scene)
    {
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_3_OR_NEWER
 SceneManager.LoadScene(scene);
#else
        Application.LoadLevel(scene);
#endif
    }

    public static void LoadLevel(int scene)
    {
#if UNITY_5_3 || UNITY_5_4 || UNITY_5_3_OR_NEWER
 SceneManager.LoadScene(scene);
#else
        Application.LoadLevel(scene);
#endif
    }

    /// <summary>
    /// Call this to capture a custom, screenshot
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D CaptureCustomScreenshot(int width, int height)
    {
        UnityEngine.Texture2D textured = new UnityEngine.Texture2D(width, height, UnityEngine.TextureFormat.RGB24, true, false);
        textured.ReadPixels(new UnityEngine.Rect(0f, 0f, (float)width, (float)height), 0, 0);
        int miplevel = UnityEngine.Screen.width / 800;
        UnityEngine.Texture2D textured2 = new UnityEngine.Texture2D(width >> miplevel, height >> miplevel, UnityEngine.TextureFormat.RGB24, false, false);
        textured2.SetPixels32(textured.GetPixels32(miplevel));
        textured2.Apply();
        return textured2;
    }
    /// <summary>
    /// Call this to capture a screenshot Automatic size
    /// </summary>
    /// <returns></returns>
    public static byte[] CaptureScreenshot()
    {
        UnityEngine.Texture2D textured = new UnityEngine.Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height, UnityEngine.TextureFormat.RGB24, false, false);
        textured.ReadPixels(new UnityEngine.Rect(0f, 0f, (float)UnityEngine.Screen.width, (float)UnityEngine.Screen.height), 0, 0);
        return textured.EncodeToPNG();
    }
    /// <summary>
    /// Call this to capture a custom size screenshot
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static byte[] CaptureScreenshot(int width, int height)
    {
        Texture2D textured = new Texture2D(width, height, UnityEngine.TextureFormat.RGB24, false, false);
        textured.ReadPixels(new UnityEngine.Rect(0f, 0f, (float)width, (float)height), 0, 0);
        return textured.EncodeToPNG();
    }

    /// <summary>
    /// Get ClampAngle
    /// </summary>
    /// <param name="ang"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampAngle(float ang, float min, float max)
    {
        if (ang < -360f)
        {
            ang += 360f;
        }
        if (ang > 360f)
        {
            ang -= 360f;
        }
        return UnityEngine.Mathf.Clamp(ang, min, max);
    }

    /// <summary>
    /// Obtained distance between two positions.
    /// </summary>
    /// <param name="posA"></param>
    /// <param name="posB"></param>
    /// <returns></returns>
    public static float GetDistance(Vector3 posA, Vector3 posB)
    {
        return Vector3.Distance(posA, posB);
    }

    public static GameObject GetGameObjectView(PhotonView m_view)
    {
        GameObject go = PhotonView.Find(m_view.viewID).gameObject;
        return go;
    }
    /// <summary>
    /// obtain only the first two values
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static string GetDoubleChar(float f)
    {
        return f.ToString("00");
    }
    /// <summary>
    /// obtain only the first three values
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static string GetThreefoldChar(float f)
    {
        return f.ToString("000");
    }

    public static string GetTimeFormat(float m, float s)
    {
        return string.Format("{0:00}:{1:00}", m, s);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="force"></param>
    /// <returns></returns>
    public static Vector3 CorrectForceSize(UnityEngine.Vector3 force)
    {
        float num = (1.2f / Time.timeScale) - 0.2f;
        force = (UnityEngine.Vector3)(force * num);
        return force;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="option"></param>
    public static void ShadowLabel(string text, params GUILayoutOption[] option)
    {
        Color color = GUI.color;
        Color black = Color.black;
        black.a = color.a;
        GUI.color = black;
        GUILayout.Label(text, option);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.x--;
        lastRect.y--;
        GUI.color = color;
        GUI.Label(lastRect, text);
    }
    public static void ShadowLabel(UnityEngine.Rect rect, string text)
    {
        ShadowLabel(rect, text, null);
    }

    public static void ShadowLabel(string text, UnityEngine.GUIStyle style, params UnityEngine.GUILayoutOption[] option)
    {
        UnityEngine.Color color = UnityEngine.GUI.color;
        UnityEngine.Color black = UnityEngine.Color.black;
        black.a = color.a;
        UnityEngine.GUI.color = black;
        UnityEngine.GUILayout.Label(text, style, option);
        UnityEngine.Rect lastRect = UnityEngine.GUILayoutUtility.GetLastRect();
        lastRect.x--;
        lastRect.y--;
        UnityEngine.GUI.color = color;
        UnityEngine.GUI.Label(lastRect, text, style);
    }

    public static void ShadowLabel(UnityEngine.Rect rect, string text, UnityEngine.GUIStyle style)
    {
        UnityEngine.Rect position = new UnityEngine.Rect(rect.x + 1f, rect.y + 1f, rect.width, rect.height);
        UnityEngine.Color color = UnityEngine.GUI.color;
        UnityEngine.Color color2 = !(color == UnityEngine.Color.black) ? UnityEngine.Color.black : UnityEngine.Color.white;
        color2.a = color.a;
        UnityEngine.GUI.color = color2;
        if (style != null)
        {
            UnityEngine.GUI.Label(position, text, style);
        }
        else
        {
            UnityEngine.GUI.Label(position, text);
        }
        UnityEngine.GUI.color = color;
        if (style != null)
        {
            UnityEngine.GUI.Label(rect, text, style);
        }
        else
        {
            UnityEngine.GUI.Label(rect, text);
        }
    }
    /// <summary>
    /// Helper for Cursor locked in Unity 5
    /// </summary>
    /// <param name="mLock">cursor state</param>
    public static void LockCursor(bool mLock)
    {
#if UNITY_5
        if (mLock == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#else
        Screen.lockCursor = mLock;
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public static bool GetCursorState
    {
        get
        {
#if UNITY_5
            if (Cursor.visible && Cursor.lockState != CursorLockMode.Locked)
            {
                return false;
            }
            else
            {
                return true;
            }
#else
            return Screen.lockCursor;
#endif
        }
    }

    // The angle between dirA and dirB around axis
    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }
    /// <summary>
    /// 
    /// </summary>
    public static bl_GameManager GetGameManager
    {
        get
        {
            bl_GameManager g = GameObject.FindObjectOfType<bl_GameManager>();
            return g;
        }
    }

    public static void PlayClipAtPoint(AudioClip clip,Vector3 position,float volume,AudioSource sourc)
    {
        GameObject obj2 = new GameObject("One shot audio")
        {
            transform = { position = position }
        };
        AudioSource source = (AudioSource)obj2.AddComponent(typeof(AudioSource));
        source.minDistance = sourc.minDistance;
        source.maxDistance = sourc.maxDistance;
        source.panStereo = sourc.panStereo;
        source.spatialBlend = sourc.spatialBlend;
        source.rolloffMode = sourc.rolloffMode;
        source.priority = sourc.priority;
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Object.Destroy(obj2, clip.length * Time.timeScale);
    }
}