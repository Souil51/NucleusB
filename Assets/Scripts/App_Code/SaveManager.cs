using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class SaveManager
{
    private static readonly bool m_bUseFile = false;
    private static readonly string m_szSaveFileName = "save.json";

    public static Save GetSave()
    {
        Save save;

        if (m_bUseFile) 
        { 
            string szFilesPath = GetSaveLocation();

            if (!File.Exists(szFilesPath))
            {
                InitGameSave();
            }

            StreamReader sr = new StreamReader(szFilesPath);
            string szJson = sr.ReadToEnd();

            save = JsonUtility.FromJson<Save>(szJson);

            sr.Close();
            sr.Dispose();
        }
        else
        {
            save = InitGameSave();
        }

        return save;
    }

    private static string GetSaveLocation()
    {
        return Application.persistentDataPath + "/" + m_szSaveFileName;
    }

    private static void SaveGameToFile(Save save)
    {
        string szFilesPath = GetSaveLocation();

        string jsonString = JsonUtility.ToJson(save);

        StreamWriter sw = new StreamWriter(szFilesPath);
        sw.Write(jsonString);
        sw.Close();
        sw.Dispose();
    }

    public static void SaveGame(Save save)
    {
        if (m_bUseFile)
            SaveGameToFile(save);
    }

    private static Save InitGameSave()
    {
        Save save = new Save();

        List<World> lstWorlds = LevelManager.GetAllWorlds();

        foreach (World w in lstWorlds)
        {
            WorldSave wSave = new WorldSave(w.GetIndex(), false);

            if (w.GetIndex() == 0)
                wSave.m_bUnlocked = true;

            foreach (Level lvl in w.GetAllLevels())
            {
                LevelSave lvlSave = new LevelSave(lvl.GetIndex(), false, false);

                if (wSave.m_nWorldIndex == 0 && lvlSave.m_nLevelIndex == 0)
                    lvlSave.m_bUnlocked = true;

                wSave.m_lstLevels.Add(lvlSave);
            }

            save.m_lstWorlds.Add(wSave);
        }

        if(m_bUseFile)
            SaveGameToFile(save);

        return save;
    }

    public static void GetLevelToComplete(Save save, out int nWorld, out int nLevel)
    {
        nWorld = 0;
        nLevel = 0;

        bool bFound = false;

        foreach (WorldSave world in save.m_lstWorlds)
        {
            foreach (LevelSave level in world.m_lstLevels)
            {
                if (!level.m_bCompleted)
                {
                    nWorld = world.m_nWorldIndex;
                    nLevel = level.m_nLevelIndex;

                    break;
                }
            }

            if (bFound)
                break;
        }
    }

    public static void UnlockAll(Save save)
    {
        foreach(WorldSave wld in save.m_lstWorlds)
        {
            wld.m_bUnlocked = true;
            
            foreach(LevelSave lvl in wld.m_lstLevels)
            {
                lvl.m_bUnlocked = true;
                lvl.m_bCompleted = true;
            }
        }
    }
}

[Serializable]
public class Save
{
    public List<WorldSave> m_lstWorlds;

    public Save()
    {
        m_lstWorlds = new List<WorldSave>();
    }

    public WorldSave GetWorld(int nIndex)
    {
        if (nIndex >= m_lstWorlds.Count)
            return new WorldSave();
        else
            return m_lstWorlds[nIndex];
    }
}

[Serializable]
public class WorldSave
{
    public int m_nWorldIndex;
    public bool m_bUnlocked;
    public List<LevelSave> m_lstLevels;

    public WorldSave()
    {
        m_nWorldIndex = -1;
        m_bUnlocked = false;
        m_lstLevels = new List<LevelSave>();
    }

    public WorldSave(int nIndex, bool bUnlocked)
    {
        m_nWorldIndex = nIndex;
        m_bUnlocked = bUnlocked;
        m_lstLevels = new List<LevelSave>();
    }

    public LevelSave GetLevel(int nIndex)
    {
        if (nIndex >= m_lstLevels.Count)
            return new LevelSave();
        else
            return m_lstLevels[nIndex];
    }
}

[Serializable]
public class LevelSave
{
    public int m_nLevelIndex;
    public bool m_bUnlocked;
    public bool m_bCompleted;

    public int m_nScore;

    public LevelSave()
    {
        m_nLevelIndex = -1;
        m_bUnlocked = false;
        m_bCompleted = false;
        m_nScore = 0;
    }

    public LevelSave(int nIndex, bool bUnlocked, bool bCompleted)
    {
        m_nLevelIndex = nIndex;
        m_bUnlocked = bUnlocked;
        m_bCompleted = bCompleted;
    }
}