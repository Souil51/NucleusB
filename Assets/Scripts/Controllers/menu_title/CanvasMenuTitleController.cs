using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuTitleController : MonoBehaviour
{

    void Update()
    {
        transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR).localEulerAngles += new Vector3(0, 0, 0.5f);
    }

    private Transform GetButton(int n)
    {
        Transform t = null;

        switch (n)
        {
            case 0: t = transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_CONTINUE); break;
            case 1: t = transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_NIVEAUX); break;
            case 2: t = transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_QUITTER); break;
        }

        return t;
    }

    public Vector3 GetButtonLocalPosition(int n)
    {
        return GetButton(n).localPosition;
    }

    public void SetButtonLocalPosition(int n, Vector3 vNew)
    {
        Transform tBouton = GetButton(n);

        tBouton.localPosition = vNew;
    }

    private Transform GetCurseur()
    {
        return transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR);
    }

    public Vector3 GetCurseurLocalPosition()
    {
        return GetCurseur().localPosition;
    }

    public void SetCurseurLocalPosition(Vector3 vNew)
    {
        GetCurseur().localPosition = vNew;
    }

    private Transform GetPanelButtons()
    {
        return transform.Find(StaticResources.TRANSFORM_PANEL_BOUTONS);
    }

    public Vector3 GetPanelButtonsLocalScale()
    {
        return GetPanelButtons().localScale;
    }

    public void SetPanelButtonsLocalScale(Vector3 vNew)
    {
        GetPanelButtons().localScale = vNew;
    }

    private Transform GetCircleImage()
    {
        return transform.Find(StaticResources.TRANSFORM_CIRCLE_IMAGE);
    }

    public Vector3 GetCircleImageLocalScale()
    {
        return GetCircleImage().localScale;
    }

    public void SetCircleImageLocalScale(Vector3 vNew)
    {
        GetCircleImage().localScale = vNew;
    }

    public void LabelAllUnlockedVisible(bool bValue)
    {
        Transform tAllUnlocked = transform.Find(StaticResources.TRANSFORM_LABEL_ALL_UNLOCKED);

        tAllUnlocked.GetComponent<Text>().text = "All levels are unlocked !";
    }

    public void ChangeLayout(StaticResources.KeyboardLayout layout)
    {
        if(layout == StaticResources.KeyboardLayout.QWERTY)
        {
            transform.Find("lblCurrentLayout").GetComponent<Text>().text = "Current keyboard layout : QWERTY";
            transform.Find("lblChangeLayout").GetComponent<Text>().text = "Press K to change to AZERTY";
        }
        else
        {
            transform.Find("lblCurrentLayout").GetComponent<Text>().text = "Current keyboard layout : AZERTY";
            transform.Find("lblChangeLayout").GetComponent<Text>().text = "Press K to change to QWERTY";
        }
    }
}
