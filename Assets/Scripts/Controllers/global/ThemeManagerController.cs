using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManagerController : MonoBehaviour
{
    public static ThemeManagerController instance;
    private float m_fVolume;

    /// <summary>Awake is called when the script instance is being loaded.</summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            m_fVolume = GetComponent<AudioSource>().volume;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartPlayTheme()
    {
        GetComponent<AudioSource>().Play();
    }

    public void StopPlayTheme()
    {
        GetComponent<AudioSource>().Stop();
    }

    public void MuteTheme()
    {
        GetComponent<AudioSource>().volume = 0;
    }

    public void UnmuteTheme()
    {
        GetComponent<AudioSource>().volume = m_fVolume;
    }
}
