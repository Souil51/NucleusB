using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private readonly List<GameObject> m_lstCharges = new List<GameObject>();
    private readonly List<GameObject> m_lstSante = new List<GameObject>();

    private Transform m_tPanelPause;
    private Transform m_tCurseur;

    private bool m_bRotateCurseur = false;

    // Start is called before the first frame update
    void Start()
    {
        m_tPanelPause = transform.Find(StaticResources.TRANSFORM_PANEL_PAUSE);
        m_tCurseur = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_bRotateCurseur)
            m_tCurseur.localEulerAngles += new Vector3(0, 0, 0.5f);
    }

    public void UI_HideAll()
    {
        foreach (GameObject go in m_lstCharges)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in m_lstSante)
        {
            go.SetActive(false);
        }
    }

    public void UpdateScore(int nScore)
    {
        Transform tPnlScore = gameObject.transform.Find(StaticResources.TRANSFORM_PANEL_SCORE);
        Transform tTxtScore = tPnlScore.Find(StaticResources.TRANSFORM_LABEL_SCORE);

        tTxtScore.GetComponent<Text>().text = nScore.ToString();
    }

    public void UpdateLevelCompletion(float fNewValue)
    {
        Transform tSlider = transform.Find(StaticResources.TRANSFORM_SLIDER_COMPLETION);

        if (fNewValue < 0)
            fNewValue = 0;

        if (fNewValue > 1)
            fNewValue = 1;

        tSlider.GetComponent<Slider>().value = fNewValue;
    }

    public void SetTutoVisibility(TUTORIEL tuto, bool bVisibility)
    {
        Transform tPanel = null;

        switch (tuto)
        {
            case TUTORIEL.PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE:
                tPanel = transform.Find("pnlTuto_1");
                break;
            case TUTORIEL.COUCHE_ENERGIE:
                tPanel = transform.Find("pnlTuto_2");
                break;
            case TUTORIEL.PLAYER_CHARGE:
                tPanel = transform.Find("pnlTuto_3");
                break;
            case TUTORIEL.PLAYER_GOD_MODE:
                tPanel = transform.Find("pnlTuto_4");
                break;
        }

        if (tPanel != null)
            tPanel.gameObject.SetActive(bVisibility);

        if (bVisibility)
            SetKeyboadKeyTuto(tuto);
    }

    public void SetKeyboadKeyTuto(TUTORIEL tuto)
    {
        Transform tPanel = null;

        switch (tuto)
        {
            case TUTORIEL.PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE:
                tPanel = transform.Find("pnlTuto_1");
                break;
            case TUTORIEL.COUCHE_ENERGIE:
                tPanel = transform.Find("pnlTuto_2");
                break;
            case TUTORIEL.PLAYER_CHARGE:
                tPanel = transform.Find("pnlTuto_3");
                break;
            case TUTORIEL.PLAYER_GOD_MODE:
                tPanel = transform.Find("pnlTuto_4");
                break;
        }

        foreach (Transform child in tPanel) 
        {
            Text txtChild = child.GetComponent<Text>();

            if(txtChild != null)
            {
                txtChild.text = txtChild.text.Replace("KEY_UP", StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout).ToString());
                txtChild.text = txtChild.text.Replace("KEY_DOWN", StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout).ToString());
                txtChild.text = txtChild.text.Replace("KEY_LEFT", StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout).ToString());
                txtChild.text = txtChild.text.Replace("KEY_RIGHT", StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout).ToString());
            }

            foreach(Transform child2 in child)
            {
                Text txtChild2 = child2.GetComponent<Text>();

                if (txtChild2 != null)
                {
                    txtChild2.text = txtChild2.text.Replace("KEY_UP", StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout).ToString());
                    txtChild2.text = txtChild2.text.Replace("KEY_DOWN", StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout).ToString());
                    txtChild2.text = txtChild2.text.Replace("KEY_LEFT", StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout).ToString());
                    txtChild2.text = txtChild2.text.Replace("KEY_RIGHT", StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout).ToString());
                }
            }
        }
    }

    public void ChangeBackgroundColor(Color c)
    {
        transform.Find(StaticResources.TRANSFORM_PANEL).GetComponent<Image>().color = c;
    }

    #region Sante / Charge
    public void DisplayCharges(int nbCharges, int nbChargePleine = 0)
    {
        if (m_lstCharges.Count == 0)
        {
            GameObject go;
            RectTransform rTrans;

            int nChildIndex = transform.childCount - DataManagerController.NOMBRE_TUTORIELS;

            for (int i = 0; i < nbCharges; i++)
            {
                go = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_CHARGE));
                go.transform.SetParent(this.transform);
                go.transform.SetSiblingIndex(nChildIndex);

                rTrans = go.GetComponent<RectTransform>();
                rTrans.anchorMin = new Vector2(0, 0);
                rTrans.anchorMax = new Vector2(0, 0);
                rTrans.pivot = new Vector2(0.5f, 0.5f);
                go.transform.position = new Vector3(35f + (i * 60f), 55f, 0);

                if (i < nbChargePleine)
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_WIN_CHARGE);
                }
                else
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_LOSE_CHARGE);
                }

                m_lstCharges.Add(go);
            }
        }
        else
        {
            GameObject go;

            for (int i = 0; i < m_lstCharges.Count; i++)
            {
                go = m_lstCharges[i];

                if (i < nbChargePleine)
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_WIN_CHARGE);
                }
                else
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_LOSE_CHARGE);
                }
            }
        }
    }

    public void DisplaySante(int nbSante, int nbSantePleine = 0)
    {
        if (m_lstSante.Count == 0)
        {
            GameObject go;
            RectTransform rTrans;
            RectTransform rTransCanvas;

            int nChildIndex = transform.childCount - DataManagerController.NOMBRE_TUTORIELS;

            for (int i = 0; i < nbSante; i++)
            {
                go = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_SANTE));
                go.transform.SetParent(this.transform);
                go.transform.SetSiblingIndex(nChildIndex);

                rTrans = go.GetComponent<RectTransform>();
                rTrans.anchorMin = new Vector2(0, 0);
                rTrans.anchorMax = new Vector2(0, 0);
                rTrans.pivot = new Vector2(0.5f, 0.5f);

                rTransCanvas = this.GetComponent<RectTransform>();

                float fPositionX = rTransCanvas.rect.width - (60f * (nbSante - 1) + 35f);

                go.transform.position = new Vector3(fPositionX + (i * 60f), 55f, 0);

                if (i < nbSantePleine)
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_WIN_SANTE);
                }
                else
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_LOSE_SANTE);
                }

                m_lstSante.Add(go);
            }
        }
        else
        {
            GameObject go;

            for (int i = 0; i < m_lstSante.Count; i++)
            {
                go = m_lstSante[i];

                if (i < nbSantePleine)
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_WIN_SANTE);
                }
                else
                {
                    go.GetComponent<Animator>().Play(StaticResources.ANIMATION_LOSE_SANTE);
                }
            }
        }
    }
    #endregion

    #region Menu

    public Vector3 GetCurseurLocalPosition()
    {
        return m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR).localPosition;
    }

    public void SetCurseurLocalPosition(Vector3 vNew)
    {
        m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR).localPosition = vNew;
    }

    public Transform GetButton(int n)
    {
        Transform t = null;

        switch (n)
        {
            case 0: t = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_CONTINUE); break;
            case 1: t = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_RECOMMENCER); break;
            case 2: t = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_QUITTER); break;
        }

        return t;
    }

    public void SetButtonLocalPosition(int n, Vector3 vNew)
    {
        GetButton(n).localPosition = vNew;
    }

    public Vector3 GetButtonLocalPosition(int n)
    {
        return GetButton(n).localPosition;
    }

    #endregion

    #region Pause

    public void ShowPausePanel()
    {
        m_tPanelPause.SetParent(null);
        m_tPanelPause.SetParent(transform);

        m_tPanelPause.Find(StaticResources.TRANSFORM_PAUSE).GetComponent<Text>().text = "Pause";
        m_tPanelPause.Find(StaticResources.TRANSFORM_RESUME).gameObject.SetActive(true);

        Transform tBoutonContinuer = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_CONTINUE);
        Transform tBoutonRecommencer = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_RECOMMENCER);
        Transform tBoutonQuitter = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_QUITTER);
        Transform tCurseur = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR);

        tBoutonContinuer.gameObject.SetActive(false);
        tCurseur.localPosition = new Vector3(tCurseur.localPosition.x, 100, tCurseur.localPosition.z);
        tBoutonContinuer.localPosition = new Vector3(60, tBoutonContinuer.localPosition.y, tCurseur.localPosition.z);
        tBoutonRecommencer.localPosition = new Vector3(110, tBoutonRecommencer.localPosition.y, tCurseur.localPosition.z);

        tBoutonRecommencer.GetComponent<Text>().text = "Resume";
        tBoutonQuitter.GetComponent<Text>().text = "Quit";

        m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_SCORE).gameObject.SetActive(false);
        m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_NEW_RECORD).gameObject.SetActive(false);

        m_tPanelPause.gameObject.SetActive(true);

        m_bRotateCurseur = true;
    }

    public void HidePausePanel()
    {
        m_tPanelPause.gameObject.SetActive(false);

        m_bRotateCurseur = true;
    }

    #endregion

    #region End Game

    public void ShowEndGamePanel(bool bShowFirstBouton, bool bIsPlayerDead, int nScore, bool bNewRecord)
    {
        m_tPanelPause.SetParent(null);
        m_tPanelPause.SetParent(transform);

        m_tPanelPause.Find(StaticResources.TRANSFORM_PAUSE).GetComponent<Text>().text = "Level finished";
        m_tPanelPause.Find(StaticResources.TRANSFORM_RESUME).gameObject.SetActive(false);

        Transform tBoutonContinuer = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_CONTINUE);
        Transform tBoutonRecommencer = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_RECOMMENCER);
        Transform tBoutonQuitter = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_BOUTON_QUITTER);
        Transform tCurseur = m_tPanelPause.Find(StaticResources.TRANSFORM_PANEL_BOUTONS).Find(StaticResources.TRANSFORM_MENU_CURSEUR);

        //Position initiale du curseur
        if (bShowFirstBouton)
        {
            tCurseur.localPosition = new Vector3(tCurseur.localPosition.x, 155, tCurseur.localPosition.z);

            tBoutonContinuer.localPosition = new Vector3(110, tBoutonContinuer.localPosition.y, tCurseur.localPosition.z);
            tBoutonRecommencer.localPosition = new Vector3(60, tBoutonRecommencer.localPosition.y, tCurseur.localPosition.z);

            tBoutonContinuer.gameObject.SetActive(true);
            tBoutonContinuer.GetComponent<Text>().text = "Next level";
        }
        else
        {
            tBoutonContinuer.gameObject.SetActive(false);

            tBoutonContinuer.localPosition = new Vector3(60, tBoutonContinuer.localPosition.y, tCurseur.localPosition.z);
            tBoutonRecommencer.localPosition = new Vector3(110, tBoutonRecommencer.localPosition.y, tCurseur.localPosition.z);

            tCurseur.localPosition = new Vector3(tCurseur.localPosition.x, 100, tCurseur.localPosition.z);
        }

        tBoutonRecommencer.GetComponent<Text>().text = "Restart";
        tBoutonQuitter.GetComponent<Text>().text = "Quit";

        m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_SCORE).gameObject.SetActive(true);
        m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_SCORE).GetComponent<Text>().text = "Score : " + nScore;

        if (bNewRecord && !bIsPlayerDead)
        {
            m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_NEW_RECORD).gameObject.SetActive(true);
        }
        else
        {
            m_tPanelPause.Find(StaticResources.TRANSFORM_LABEL_NEW_RECORD).gameObject.SetActive(false);
        }

        m_tPanelPause.gameObject.SetActive(true);

        m_bRotateCurseur = true;
    }

    public void HideEndGamePanel()
    {
        HidePausePanel();
    }

    #endregion

    #region Before Start

    public Transform GetBeforeStartPanel()
    {
        return transform.Find(StaticResources.TRANSFORM_PANEL_BEFORE_START);
    }

    public void SetBeforeStartPanelLocalScale(Vector3 vNewScale)
    {
        transform.Find(StaticResources.TRANSFORM_PANEL_BEFORE_START).localScale = vNewScale;
    }

    public Vector3 GetBeforeStartPanelLocalScale()
    {
        return transform.Find(StaticResources.TRANSFORM_PANEL_BEFORE_START).localScale;
    }

    public void SetBeforeStartTexts(string szWorld, string szLevel)
    {
        transform.Find(StaticResources.TRANSFORM_PANEL_BEFORE_START).Find(StaticResources.TRANSFORM_LABEL_WORLD).GetComponent<Text>().text = szWorld;
        transform.Find(StaticResources.TRANSFORM_PANEL_BEFORE_START).Find(StaticResources.TRANSFORM_LABEL_LEVEL).GetComponent<Text>().text = szLevel;
    }

    #endregion

}
