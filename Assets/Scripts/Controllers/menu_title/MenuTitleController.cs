using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTitleController : MonoBehaviour
{
    private bool m_bIsMoving = false;
    private int m_nButtonSelected = 0;
    private readonly int m_nButtonCount = 3;

    private bool m_bAllUnlocked = false;
    private readonly List<KeyCode> m_lstKeyCode = new List<KeyCode>();
    private readonly List<KeyCode> m_lstKonamiCode = new List<KeyCode> { StaticResources.KEY_UP_2, StaticResources.KEY_UP_2 , StaticResources.KEY_DOWN_2 , StaticResources.KEY_DOWN_2, StaticResources.KEY_LEFT_2, StaticResources.KEY_RIGHT_2, StaticResources.KEY_LEFT_2, StaticResources.KEY_RIGHT_2, StaticResources.KEY_SECRET_B, StaticResources.KEY_SECRET_A };
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    private CanvasMenuTitleController m_canvasCtrl;

    // Start is called before the first frame update
    void Start()
    {
        m_canvasCtrl = GameObject.FindGameObjectWithTag(StaticResources.TAG_CANVAS).GetComponent<CanvasMenuTitleController>();

        InvokeRepeating("CreateTraitFond", 0f, 0.5f);
        CreateTraitFond();

        DataManagerController.instance.InitData();
        SoundManager.LoadAudioResources();

        m_canvasCtrl.ChangeLayout(DataManagerController.instance.m_keyboardLayout);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bAllUnlocked && Input.anyKeyDown)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in keyCodes)
                {
                    if (Input.GetKey(keyCode))
                    {
                        m_lstKeyCode.Add(keyCode);
                        break;
                    }
                }
            }

            if (VerifierKonamiCode())
            {
                DataManagerController.instance.UnlockAllLevels();
                m_bAllUnlocked = true;

                m_canvasCtrl.LabelAllUnlockedVisible(true);
            }
        }

        if (Input.GetKeyDown(StaticResources.KEY_LAYOUT))
        {
            StaticResources.KeyboardLayout keyboardLayout = DataManagerController.instance.ChangeLayout();

            m_canvasCtrl.ChangeLayout(keyboardLayout);
        }

        if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout)) && !m_bIsMoving && m_nButtonSelected > 0)
        {
            StartCoroutine(SelectedButtonChanged(-1, 0.15f));
        }
        else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout)) && !m_bIsMoving && m_nButtonSelected < m_nButtonCount - 1)
        {
            StartCoroutine(SelectedButtonChanged(1, 0.15f));
        }

        if ((Input.GetKey(StaticResources.KEY_ACTION) || Input.GetKey(StaticResources.Key_ACTION_2)) && !m_bIsMoving)
        {
            SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_SHORT);

            if (m_nButtonSelected == 0)
                StartCoroutine(DisplayHidingCircle(1.0f));
            else if (m_nButtonSelected == 1)
                StartCoroutine(LoadMenuWorldScene(0.3f));
            else if (m_nButtonSelected == 2)
                Application.Quit();
        }
    }

    private void CreateTraitFond()
    {
        Utilitaire.CreateTraitFond(4f, 0.5f);
    }

    private bool VerifierKonamiCode()
    {
        bool bRes = true;

        if (m_lstKeyCode.Count < m_lstKonamiCode.Count)
            bRes = false;
        else
        {
            for(int i = 0; i < m_lstKonamiCode.Count; i++)
            {
                if(m_lstKonamiCode[i] != m_lstKeyCode[m_lstKeyCode.Count - m_lstKonamiCode.Count + i])
                {
                    bRes = false;
                    break;
                }
            }
        }

        return bRes;
    }

    private IEnumerator SelectedButtonChanged(int nSens, float fDuration = 1.0f)
    {
        m_bIsMoving = true;

        float fScale = 1 / fDuration;

        int nNewSelectedButton = m_nButtonSelected + nSens;

        float fXToGo = 50f;
        float fYToGo = 55f;

        int nIndexBouton_1;
        int nIndexBouton_2;

        if (nSens > 0)
        {
            nIndexBouton_1 = m_nButtonSelected;
            nIndexBouton_2 = nNewSelectedButton;
        }
        else
        {
            nIndexBouton_1 = nNewSelectedButton;
            nIndexBouton_2 = m_nButtonSelected;
        }

        for (float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            float fXAdd = Time.deltaTime * fXToGo * fScale;
            float fYAdd = Time.deltaTime * fYToGo * fScale;

            Vector3 vPosContinuer = m_canvasCtrl.GetButtonLocalPosition(nIndexBouton_1);
            Vector3 vPosNiveaux = m_canvasCtrl.GetButtonLocalPosition(nIndexBouton_2);
            Vector3 vPosCurseur = m_canvasCtrl.GetCurseurLocalPosition();

            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_1, new Vector3(vPosContinuer.x - (fXAdd * nSens), vPosContinuer.y, vPosContinuer.z));
            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_2, new Vector3(vPosNiveaux.x + (fXAdd * nSens), vPosNiveaux.y, vPosNiveaux.z));
            m_canvasCtrl.SetCurseurLocalPosition(new Vector3(vPosCurseur.x, vPosCurseur.y - (fYAdd * nSens), vPosCurseur.z));

            yield return null;
        }

        Vector3 vLocalPositionBouton_1 = m_canvasCtrl.GetButtonLocalPosition(nIndexBouton_1);
        Vector3 vLocalPositionBouton_2 = m_canvasCtrl.GetButtonLocalPosition(nIndexBouton_2);

        if (nSens > 0)
        {
            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_1, new Vector3(60, vLocalPositionBouton_1.y, vLocalPositionBouton_1.z));
            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_2, new Vector3(110, vLocalPositionBouton_2.y, vLocalPositionBouton_2.z));
        }
        else
        {
            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_1, new Vector3(110, vLocalPositionBouton_1.y, vLocalPositionBouton_1.z));
            m_canvasCtrl.SetButtonLocalPosition(nIndexBouton_2, new Vector3(60, vLocalPositionBouton_2.y, vLocalPositionBouton_2.z));
        }

        m_nButtonSelected = nNewSelectedButton;

        float fCurseurY = ((m_nButtonCount - 1 - m_nButtonSelected) * 55) - 10;
        Vector3 vCurseurLocalPosition = m_canvasCtrl.GetCurseurLocalPosition();
        m_canvasCtrl.SetCurseurLocalPosition(new Vector3(vCurseurLocalPosition.x, fCurseurY, vCurseurLocalPosition.z));

        m_bIsMoving = false;
    }
    private IEnumerator LoadMenuWorldScene(float fAnimationDuration = 1.0f)
    {
        m_bIsMoving = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu_world");
        asyncLoad.allowSceneActivation = false;

        float fScale = 1 / fAnimationDuration;

        float fScaleToGo = 1.6f;
        float fScaleToGoPanel = 1f;

        float fDurationUnscaleMenu = fAnimationDuration / 2;

        for (float f = fDurationUnscaleMenu; f >= 0; f -= Time.deltaTime)
        {
            float fScaleAdd = Time.deltaTime * fScaleToGo * fScale * 2;
            float fScalePanel = Time.deltaTime * -fScaleToGoPanel * fScale;

            Vector3 vScale = m_canvasCtrl.GetCircleImageLocalScale();
            Vector3 vScalePanel = m_canvasCtrl.GetPanelButtonsLocalScale();

            m_canvasCtrl.SetCircleImageLocalScale(new Vector3(vScale.x + fScaleAdd, vScale.y + fScaleAdd, vScale.z));
            m_canvasCtrl.SetPanelButtonsLocalScale(new Vector3(vScalePanel.x + fScalePanel, vScalePanel.y + fScalePanel, vScalePanel.z));

            yield return null;
        }

        m_canvasCtrl.SetCircleImageLocalScale(new Vector3(0, 0, 0));
        m_canvasCtrl.SetPanelButtonsLocalScale(new Vector3(0, 0, 0));

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        m_bIsMoving = false;
    }

    private IEnumerator DisplayHidingCircle(float fAnimationDuration = 1.0f)
    {
        m_bIsMoving = true;

        float fScale = 1 / fAnimationDuration;
        float fScaleToGo = 10f;

        for (float f = fAnimationDuration; f >= 0; f -= Time.deltaTime)
        {
            float fScaleAdd = Time.deltaTime * fScaleToGo * fScale;

            Vector3 vScale = m_canvasCtrl.GetCircleImageLocalScale();

            m_canvasCtrl.SetCircleImageLocalScale(new Vector3(vScale.x + fScaleAdd, vScale.y + fScaleAdd, vScale.z));

            yield return null;
        }

        m_canvasCtrl.SetCircleImageLocalScale(new Vector3(fScaleToGo, fScaleToGo, 1));

        StartCoroutine(LoadGameSceneScene());
    }

    private IEnumerator LoadGameSceneScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        SaveManager.GetLevelToComplete(DataManagerController.instance.m_save, out int nWorld, out int nLevel);

        DataManagerController.instance.m_nSelectedWorld = nWorld;
        DataManagerController.instance.m_nSelectedLevel = nLevel;

        World w = LevelManager.GetWorld(nWorld);
        Level lvl = w.GetLevel(nLevel);

        DataManagerController.instance.m_nbCoucheSelectedLevel = lvl.GetNombreCouches();
        DataManagerController.instance.m_nbNucleonSelectedLevel = lvl.GetNombreNucleons();
        DataManagerController.instance.m_fDurationSelectedLevel = lvl.GetDuration();

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        m_bIsMoving = false;
    }
}
