using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilitaire : MonoBehaviour
{
    public static void CreateTraitFond(float fOffsetSpawn, float fAlpha)
    {
        float fPosBordX1 = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        float fPosBordX2 = Camera.main.ScreenToWorldPoint(new Vector2((float)Screen.width, 0)).x;

        float fPosBordY1 = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float fPosBordY2 = Camera.main.ScreenToWorldPoint(new Vector2(0, (float)Screen.height)).y;

        float posX;
        float posY;

        int nSens;

        if (Random.Range(0, 2) == 0)//Spawn à droite ou gauche
        {
            if (Random.Range(0, 2) == 0)
            {
                posX = Random.Range(fPosBordX1 - fOffsetSpawn, fPosBordX1);
                nSens = 1;
            }
            else
            {
                posX = Random.Range(fPosBordX2, fPosBordX2 + fOffsetSpawn);
                nSens = -1;
            }

            posY = Random.Range(fPosBordX1 - fOffsetSpawn, fPosBordX2 + fOffsetSpawn);
        }
        else//Spawn en haut ou en bas
        {
            if (Random.Range(0, 2) == 0)
            {
                posY = Random.Range(fPosBordY1 - fOffsetSpawn, fPosBordY1);
                nSens = 1;
            }
            else
            {
                posY = Random.Range(fPosBordY2, fPosBordY2 + fOffsetSpawn);
                nSens = -1;
            }

            posX = Random.Range(fPosBordY1 - fOffsetSpawn, fPosBordY2 + fOffsetSpawn);
        }

        GameObject go = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_TRAIT_FOND));

        Color c = go.GetComponent<SpriteRenderer>().color;

        go.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, fAlpha);
        go.GetComponent<TraitFondController>().m_nSens = nSens;

        Vector3 vectorPosition = new Vector3(posX, posY, go.transform.position.z);

        go.transform.position = vectorPosition;
    }
}
