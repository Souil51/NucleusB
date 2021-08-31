using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public enum TYPE_LEVEL { OXYGEN, HYDROGEN, SULFUR, CARBON }
public enum TUTORIEL { AUCUN, PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE, COUCHE_ENERGIE, PLAYER_CHARGE, PLAYER_GOD_MODE}
public class LevelManager
{
    public static List<World> m_lstWords = new List<World>();

    //Initialisation des mondes/niveaux ici, c'est fait une seule fois au lancement du jeu
    public static void InitLevels()
    {
        //World 1
        //Les positions permettent d'indiquer où afficher les mondes dans le menu des mondes
        m_lstWords.Add(new World(0, "Water", "level_water", new Vector3(0, 4, 0)));

        //Les positions permettent d'indiquer où afficher les niveaux dans le menu des niveaux
        m_lstWords[0].AddLevel(new Level(0, "Hydrogen-1", 1, 1, 15, new Vector3(-3.37f, -0.11f, 1), TYPE_LEVEL.HYDROGEN));
        m_lstWords[0].AddLevel(new Level(1, "Oxygen", 2, 2, 15, new Vector3(0, 2.1f, 1), TYPE_LEVEL.OXYGEN));
        m_lstWords[0].AddLevel(new Level(2, "Hydrogen-2", 1, 1, 30, new Vector3(3.49f, -0.12f, 1), TYPE_LEVEL.HYDROGEN));

        //Les positions permettent d'indiquer où afficher les liaisons/pointillés entre les niveaux dans le menu des niveaux
        m_lstWords[0].AddLiaisonLevel(new Quaternion(-1.78f, 0.89f, 3, 33.61f));
        m_lstWords[0].AddLiaisonLevel(new Quaternion(1.84f, 0.95f, 3, -32.71f));

        //World 2
        m_lstWords.Add(new World(1, "Carbon dioxide", "level_carbon_dioxide", new Vector3(-4, 0, 0)));

        m_lstWords[1].AddLevel(new Level(0, "Oxygen-1", 2, 4, 30, new Vector3(-3.37f, -0.11f, 1), TYPE_LEVEL.OXYGEN));
        m_lstWords[1].AddLevel(new Level(1, "Carbon", 2, 4, 30, new Vector3(0, 2.1f, 1), TYPE_LEVEL.CARBON));
        m_lstWords[1].AddLevel(new Level(2, "Oxygen-2", 2, 4, 30, new Vector3(3.49f, -0.12f, 1), TYPE_LEVEL.OXYGEN));

        m_lstWords[1].AddLiaisonLevel(new Quaternion(-1.78f, 0.89f, 3, 33.61f));
        m_lstWords[1].AddLiaisonLevel(new Quaternion(1.84f, 0.95f, 3, -32.71f));

        m_lstWords.Add(new World(2, "Acetylene", "level_acetylene", new Vector3(0, -4, 0)));

        //World 3
        m_lstWords[2].AddLevel(new Level(0, "Hydrogen-1", 2, 4, 60, new Vector3(-3.53f, -0.31f, 1), TYPE_LEVEL.HYDROGEN));
        m_lstWords[2].AddLevel(new Level(1, "Carbon-1", 2, 4, 60, new Vector3(-0.68f, 2.3f, 1), TYPE_LEVEL.CARBON));
        m_lstWords[2].AddLevel(new Level(2, "Carbon-2", 2, 4, 60, new Vector3(1.66f, -0.09f, 1), TYPE_LEVEL.CARBON));
        m_lstWords[2].AddLevel(new Level(3, "Hydrogen-2", 2, 4, 60, new Vector3(4.35f, 2.49f, 1), TYPE_LEVEL.HYDROGEN));

        m_lstWords[2].AddLiaisonLevel(new Quaternion(-2.05f, 1.04f, 3, 42.84f));
        m_lstWords[2].AddLiaisonLevel(new Quaternion(0.54f, 1.1f, 3, -47.4f));
        m_lstWords[2].AddLiaisonLevel(new Quaternion(2.9f, 1.09f, 3, 44.61f));

        m_lstWords.Add(new World(3, "Sulfur trioxide", "level_sulfur_trioxide", new Vector3(4, 0, 0)));

        //World 4
        m_lstWords[3].AddLevel(new Level(0, "Oxygen-1", 2, 4, 60, new Vector3(-2.52f, 0.27f, 1), TYPE_LEVEL.OXYGEN));
        m_lstWords[3].AddLevel(new Level(1, "Sulfur", 3, 4, 60, new Vector3(0f, 2.26f, 1), TYPE_LEVEL.SULFUR));
        m_lstWords[3].AddLevel(new Level(2, "Oxygen-2", 2, 4, 60, new Vector3(0f, 5.5f, 1), TYPE_LEVEL.OXYGEN));
        m_lstWords[3].AddLevel(new Level(3, "Oxygen-3", 2, 4, 60, new Vector3(2.52f, 0.27f, 1), TYPE_LEVEL.OXYGEN));

        m_lstWords[3].AddLiaisonLevel(new Quaternion(-1.28f, 1.25f, 3, 38.44f));
        m_lstWords[3].AddLiaisonLevel(new Quaternion(0f, 3.82f, 3, 90));
        m_lstWords[3].AddLiaisonLevel(new Quaternion(1.28f, 1.3f, 3, 141.6f));
    }

    public static World GetWorld(int n)
    {
        if (n >= 0 && n <= m_lstWords.Count)
            return m_lstWords[n];
        else
            return null;
    }

    public static List<World> GetAllWorlds()
    {
        return m_lstWords;
    }


    //Taille des couches en fonction de leur nombre
    public static float GetCoucheScale(int nbCouches)
    {
        float fRes = 1f;

        if (nbCouches == 1)
            fRes = 2f;
        else if (nbCouches == 2)
            fRes = 1f;
        else if (nbCouches == 3)
            fRes = 0.7f;

        return fRes;
    }

    //Permet d'avoir les index de monde et de niveau du prochain niveau
    public static void GetNextLevel(int nCurrentWorld, int nCurrentLevel, out int nWorld, out int nLevel)
    {
        if(nCurrentLevel + 1 >= m_lstWords[nCurrentWorld].GetAllLevels().Count)
        {
            nWorld = nCurrentWorld + 1;
            nLevel = 0;
        }
        else
        {
            nWorld = nCurrentWorld;
            nLevel = nCurrentLevel + 1;
        }
    }

    //Retourne les patterns d'un niveau
    public static List<LevelPattern> GetPatterns(int nWorlds, int nLevel)
    {
        List<LevelPattern> lstPatterns = new List<LevelPattern>();

        if (nWorlds == 0)
        {
            if (nLevel == 0) { } //Niveau 1 : pas de patterns
            else if (nLevel == 1) { } //Niveau 2 : pas de patterns
            else if (nLevel == 2) //Niveau 3
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_1, 3f, 10f, 0, 2f, 0.25f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_1, 15f, 10f, 0, 4f, 0.25f, 100));
            }
        }
        else if (nWorlds == 1)
        {
            if (nLevel == 0)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_1, 3f, 12f, 15, 5f, 0.4f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_1, 10f, 10f, 155, 5f, 0.4f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 20f, 10f, 45, 10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 25f, 5f, 0, 8f, 0.4f, 100));
            }
            else if (nLevel == 1)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 3f, 12f, 0, 5f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 10f, 8f, 180, 10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 18f, 8f, 0, -10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 26f, 4f, 90, 7f, 0.4f, 100));
            }
            else if (nLevel == 2)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 3f, 12f, 0, 10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 20f, 10f, 0, 10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 24f, 6f, 0, -10f, 0.5f, 100));
            }
        }
        else if (nWorlds == 2)
        {
            if (nLevel == 0)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 3f, 7f, 0, 10f, 0.6f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 10f, 5f, 0, 10f, 0.6f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 15, 5f, 0, 10f, 0.6f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 20f, 3f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 21f, 3f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 22f, 3f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 23f, 3f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 25, 5f, 0, 12f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 30, 5f, 36, 12f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 35, 5f, 0, -10f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 40, 5f, 0, 10f, 0.5f, 100));
            }
            else if (nLevel == 1)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_1, 3f, 42f, 0, 10f, 1.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 10f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 10f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 20f, 5f, 0, 8f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 25f, 5f, 0, -8f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 30f, 3f, 0, 0f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 33f, 3f, 45, 0f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 36f, 3f, 90, 0f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 39f, 6f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 39f, 6f, 90, 15f, 0.5f, 100));
            }
            else if (nLevel == 2)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 3f, 7f, 0, 13f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 7f, 5f, 0, 13f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 10f, 5f, 0, 13f, 0.5f, 100));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_1, 15f, 10f, 0, 15f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_1, 25f, 10f, 180, -15f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 35f, 10f, 0, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 35f, 10f, 90, 10f));
            }
            else if(nLevel == 3)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 3f, 5f, 0, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 8f, 3f, 45, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 11f, 3f, 90, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 14f, 3f, 135, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 17f, 3f, 180, 0f, 0.5f, 150));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 20f, 5f, 0, 10f, 0.5f, 120));

                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 25f, 5f, 0, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 22f, 5f, 0, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 24f, 5f, 0, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 26f, 5f, 0, 10f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 31f, 6f, 0, 15f, 0.5f, 120));

                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 37f, 8f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 38f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 39f, 6f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
            }
        }
        else if (nWorlds == 3)
        {
            if (nLevel == 0)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 3f, 4f, 0, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 7f, 2f, 36, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 9f, 2f, 72, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 11f, 2f, 108, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 13f, 2f, 144, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 15f, 2f, 180, 0f, 0.5f, 175));

                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 17f, 5f, 0, 13f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 17f, 5f, 0, 13f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 22f, 2f, 0, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 24f, 2f, 36, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 26f, 2f, 72, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 28f, 2f, 108, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 30f, 2f, 144, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 32f, 2f, 180, 5f, 0.5f, 175));

                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 34f, 3f, 0, 0f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 37f, 3f, 45, 0f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 40f, 3f, 90, 0f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 43f, 3f, 135, 0f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 46f, 1f, 0, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 47f, 1f, 60, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 48f, 1f, 0, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 49f, 1f, 60, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 50f, 1f, 0, 0f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 51f, 1f, 60, 0f, 0.5f, 150));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 52f, 8f, 0, 15f, 0.5f, 150));
            }
            else if (nLevel == 1)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 3f, 4f, 0, 0f, 0.5f, 200));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 7f, 2f, 36, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 7f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 7f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 9f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 9f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 9f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 11f, 2f, 36, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 11f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 11f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 13f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 13f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 13f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 15f, 2f, 36, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 17f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 17f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 17f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 19f, 2f, 36, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 19f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 18f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 21f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 21f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 21f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 23f, 2f, 36, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 21f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 21f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 25f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 25f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 25f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 27f, 2f, 0, 0f, 0.5f, 200));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 27f, 7f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 27f, 7f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 30f, 5f, 0, 10f, 0.5f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 35f, 5f, 0, -10f, 0.5f, 185));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 40f, 10f, 0, 10f, 0.5f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 40f, 10f, 60, 10f, 0.5f, 185));

                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 50f, 10f, 0, 15f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 50f, 10f, 60, 15f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 50f, 10f, 120, 15f));
            }
            else if (nLevel == 2)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 3f, 6f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 7f, 4f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 9f, 4f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 11f, 4f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 12f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 13f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 14f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 15f, 5f));

                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 18f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 18f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 18f, 2f, 0, 12f, 0.5f, 150));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 20f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 20f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 20f, 2f, 0, -12f, 0.5f, 150));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 22f, 10f, 0, 12f, 0.8f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 32f, 10f, 0, 14f, 0.8f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_3, 42f, 10f, 0, 18f, 0.8f, 185));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 50f, 10f, 0, 12f, 0.8f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 50f, 7f, 0, 10f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_2, 55f, 5f, 0, -12f, 0.8f, 185));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 55f, 5f, 110, 10f));
            }
            else if (nLevel == 3)
            {
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 3f, 4f, 0, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 8f, 2f, 36, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 11f, 2f, 72, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 14f, 2f, 108, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 17f, 2f, 144, 0f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 20f, 2f, 180, 0f, 0.5f, 175));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 23f, 2f, 0, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 26f, 2f, 36, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 29f, 2f, 72, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 32f, 2f, 108, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 35f, 2f, 144, 5f, 0.5f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 38f, 2f, 180, 5f, 0.5f, 175));

                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));
                lstPatterns.Add(new LevelPattern(Pattern.BARRIERE, 40f, 5f));

                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 45f, 5f, 0, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 45f, 5f, 60, 10f));
                lstPatterns.Add(new LevelPattern(Pattern.LASER_2, 45f, 5f, 120, 10f));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 50f, 2f, 0, 13f, 0.8f, 175));
                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_4, 52f, 2f, 0, -13f, 0.8f, 175));

                lstPatterns.Add(new LevelPattern(Pattern.BRANCHE_5, 54f, 6f, 0, 25f, 0.8f, 210));
            }
        }

        return lstPatterns;
    }

    public static TUTORIEL LevelHasTutoriel(int nWorld, int nLevel)
    {
        TUTORIEL tutoRes = TUTORIEL.AUCUN;

        if (nWorld == 0 && nLevel == 0)
            tutoRes = TUTORIEL.PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE;
        else if (nWorld == 0 && nLevel == 1)
            tutoRes = TUTORIEL.COUCHE_ENERGIE;
        else if (nWorld == 0 && nLevel == 2)
            tutoRes = TUTORIEL.PLAYER_CHARGE;
        else if (nWorld == 2 && nLevel == 0)
            tutoRes = TUTORIEL.PLAYER_GOD_MODE;

        return tutoRes;
    }
}

public class World
{
    private readonly int m_nIndex;
    private readonly string m_szName;
    private readonly string m_szGameObjectName;
    private Vector3 m_vPos;

    private readonly List<Level> m_lstLevels = new List<Level>();
    private readonly List<Quaternion> m_lstLiaisonsLevels = new List<Quaternion>();

    public World(int nIndex, string szName, string szGOSprite, Vector3 vPos)
    {
        this.m_nIndex = nIndex;
        this.m_szName = szName;
        this.m_szGameObjectName = szGOSprite;
        this.m_vPos = vPos;
    }

    public void AddLevel(Level lvl)
    {
        m_lstLevels.Add(lvl);
    }

    public void AddLiaisonLevel(Quaternion qPos)
    {
        m_lstLiaisonsLevels.Add(qPos);
    }

    public Level GetLevel(int n)
    {
        if (n >= 0 && n <= m_lstLevels.Count)
            return m_lstLevels[n];
        else
            return null;
    }

    public List<Level> GetAllLevels()
    {
        return m_lstLevels;
    }

    public int GetIndex()
    {
        return m_nIndex;
    }

    public string GetGOName()
    {
        return m_szGameObjectName;
    }

    public string GetName()
    {
        return m_szName;
    }

    public Vector3 GetPositionMenu()
    {
        return m_vPos;
    }

    public Quaternion GetLiaison(int n)
    {
        return m_lstLiaisonsLevels[n];
    }
}

public class Level
{
    private readonly int m_nIndex;
    private readonly string m_szName;
    private Vector3 m_vPosition;
    private readonly TYPE_LEVEL m_typeLevel;
    private readonly int m_nbCouches;
    private readonly int m_nbNucleons;
    private readonly float m_fDuration;

    public Level(int nIndex, string szName, int nbCouches, int nbNucleons, float fDuration, Vector3 vPosition, TYPE_LEVEL type)
    {
        this.m_nIndex = nIndex;
        this.m_szName = szName;
        this.m_vPosition = vPosition;
        this.m_typeLevel = type;
        this.m_nbCouches = nbCouches;
        this.m_nbNucleons = nbNucleons;
        this.m_fDuration = fDuration;
    }

    public TYPE_LEVEL GetLevelType()
    {
        return m_typeLevel;
    }

    public Vector3 GetPositionMenu()
    {
        return m_vPosition;
    }

    public int GetIndex()
    {
        return m_nIndex;
    }

    public string GetName()
    {
        return m_szName;
    }
    
    public int GetNombreCouches()
    {
        return this.m_nbCouches;
    }

    public int GetNombreNucleons()
    {
        return this.m_nbNucleons;
    }

    public float GetDuration()
    {
        return this.m_fDuration;
    }
}

/// <summary>
/// Classe utilisée pour définir un pattern d'un niveau (tirs d'electron, barrieres, lasers...)
/// </summary>
public class LevelPattern
{
    public Pattern m_pattern;
    public float m_fDuration;
    public float m_fLaunchTime;
    public int m_nAngle;
    public float m_fTimeBetweenShots;
    public float m_fRotationAngle;
    public int m_nForce;

    /// <summary>
    /// Initialisation d'un élement pour un tir d'electron
    /// </summary>
    public LevelPattern(Pattern pattern, float fLaunchTIme, float fDuration, int nAngle, float fRotationAngle, float fTimeBetweenShots, int nForce)
    {
        this.m_pattern = pattern;
        this.m_fDuration = fDuration;
        this.m_fLaunchTime = fLaunchTIme;
        this.m_nAngle = nAngle;
        this.m_fTimeBetweenShots = fTimeBetweenShots;
        this.m_fRotationAngle = fRotationAngle;
        this.m_nForce = nForce;
    }

    /// <summary>
    /// Initialisation d'un élement pour une barriere
    /// </summary>
    public LevelPattern(Pattern pattern, float fLaunchTIme, float fDuration)
    {
        this.m_pattern = pattern;
        this.m_fDuration = fDuration;
        this.m_fLaunchTime = fLaunchTIme;
        this.m_nAngle = 0;
        this.m_fTimeBetweenShots = 0;
        this.m_fRotationAngle = 0;
    }

    /// <summary>
    /// Initialisation d'un élement pour un laser
    /// </summary>
    public LevelPattern(Pattern pattern, float fLaunchTIme, float fDuration, int nAngle, float fRotationAngle)
    {
        this.m_pattern = pattern;
        this.m_fDuration = fDuration;
        this.m_fLaunchTime = fLaunchTIme;
        this.m_nAngle = nAngle;
        this.m_fTimeBetweenShots = 0;
        this.m_fRotationAngle = fRotationAngle;
    }
}