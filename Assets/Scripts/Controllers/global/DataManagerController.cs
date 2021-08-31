using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerController : MonoBehaviour
{
    public static int NOMBRE_TUTORIELS = 4;

    public static DataManagerController instance;

    public StaticResources.KeyboardLayout m_keyboardLayout = StaticResources.KeyboardLayout.QWERTY;
    public int m_nSelectedWorld = 0;
    public int m_nSelectedLevel = 1;
    public int m_nbNucleonSelectedLevel = 1;
    public float m_fDurationSelectedLevel = 15;
    public int m_nbCoucheSelectedLevel = 2;

    public Save m_save;
    public bool m_bIsInit = false;
    public bool m_bMusicSound = true;
    public bool m_bFXSound = true;

    /// <summary>Awake is called when the script instance is being loaded.</summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void InitData()
    {
        if (m_bIsInit)
            return;

        LevelManager.InitLevels();

        m_save = SaveManager.GetSave();

        m_bIsInit = true;
    }

    public void SaveGame()
    {
        SaveManager.SaveGame(m_save);
    }

    public LevelSave GetLevel(int nWorld, int nLevel)
    {
        WorldSave wld = m_save.GetWorld(nWorld);
        LevelSave lvl = new LevelSave();

        if(wld.m_nWorldIndex >= 0)
        {
            lvl = wld.GetLevel(nLevel);
        }

        return lvl;
    }

    public void SetLevelCompleted(int nScore)
    {
        m_save.m_lstWorlds[m_nSelectedWorld].m_lstLevels[m_nSelectedLevel].m_bCompleted = true;

        if (m_save.m_lstWorlds[m_nSelectedWorld].m_lstLevels[m_nSelectedLevel].m_nScore == 0
            || nScore > m_save.m_lstWorlds[m_nSelectedWorld].m_lstLevels[m_nSelectedLevel].m_nScore)
            m_save.m_lstWorlds[m_nSelectedWorld].m_lstLevels[m_nSelectedLevel].m_nScore = nScore;

        LevelManager.GetNextLevel(m_nSelectedWorld, m_nSelectedLevel, out int nNextWorld, out int nNextLevel);

        LevelSave lvlSave = DataManagerController.instance.GetLevel(nNextWorld, nNextLevel);

        if (lvlSave.m_nLevelIndex >= 0) 
        { 
            m_save.m_lstWorlds[nNextWorld].m_bUnlocked = true;
            m_save.m_lstWorlds[nNextWorld].m_lstLevels[nNextLevel].m_bUnlocked = true;
        }

        SaveManager.SaveGame(m_save);
    }

    public void UnlockAllLevels()
    {
        SaveManager.UnlockAll(m_save);
    }

    public StaticResources.KeyboardLayout ChangeLayout()
    {
        if (m_keyboardLayout == StaticResources.KeyboardLayout.QWERTY)
            m_keyboardLayout = StaticResources.KeyboardLayout.AZERTY;
        else
            m_keyboardLayout = StaticResources.KeyboardLayout.QWERTY;

        return m_keyboardLayout;
    }
}
