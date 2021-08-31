using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLevelController : MonoBehaviour
{
    private int m_nSelectedWorldIndex;
    private List<Level> m_lstLevels;

    private Transform m_tPlayer;

    private int m_nLevelSelected = 0;
    private bool m_bIsMoving = false;
    private readonly int m_nLevelCOunt = 3;

    private bool m_bIsChangingScene = false;

    private CanvasMenuLevelController m_canvasCtrl;

    private readonly float fXPlus = 0.5f;
    private readonly float fXMoins = -0.5f;

    private readonly float fYPlus = 0.8f;
    private readonly float fYMoins = -0.8f;

    private int m_nXSens = 1;
    private int m_nYSens = 1;

    private readonly float m_fRotatePlus = 10;
    private readonly float m_fRotateMoins = -10;

    private int m_nRotateSens = 1;

    private Transform m_tLevelsHolder;

    public GameObject goTest;

    public Sprite m_sprite_pointille_locked;
    public Sprite m_sprite_pointille_unlocked;

    private bool m_bActionsEnable = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        m_tLevelsHolder = GameObject.FindGameObjectWithTag(StaticResources.TAG_LEVELS).transform;
        m_tPlayer = m_tLevelsHolder.transform.Find(StaticResources.TRANSFORM_PLAYER);
        GameObject goCanvas = GameObject.FindGameObjectWithTag(StaticResources.TAG_CANVAS);
        m_canvasCtrl = goCanvas.GetComponent<CanvasMenuLevelController>();

        m_nSelectedWorldIndex = DataManagerController.instance.m_nSelectedWorld;
        m_lstLevels = LevelManager.GetWorld(m_nSelectedWorldIndex).GetAllLevels();
        InitLevels();

        StartCoroutine(WaitAndChangeText(m_lstLevels[0].GetName()));

        World wo = LevelManager.GetWorld(DataManagerController.instance.m_nSelectedWorld);

        m_canvasCtrl.ChangeWorldText(wo.GetName());

        InvokeRepeating("CreateTraitFond", 0f, 0.5f);
        CreateTraitFond();

        if(DataManagerController.instance.m_nSelectedLevel != -1)
        {
            m_tPlayer.localPosition = new Vector3(m_lstLevels[DataManagerController.instance.m_nSelectedLevel].GetPositionMenu().x, m_lstLevels[DataManagerController.instance.m_nSelectedLevel].GetPositionMenu().y, 2);
            m_nLevelSelected = DataManagerController.instance.m_nSelectedLevel;
        }

        StartCoroutine(WaitBeforeEnableActions());
    }

    // Update is called once per frame
    void Update()
    {
        /*Mouvement des niveaux en position et en rotation*/
        if (m_nXSens > 0)
            m_tLevelsHolder.localPosition += new Vector3(0.1f * Time.deltaTime, 0, 0);
        else
            m_tLevelsHolder.localPosition += new Vector3(-0.1f * Time.deltaTime, 0, 0);

        if (m_nYSens > 0)
            m_tLevelsHolder.localPosition += new Vector3(0, 0.1f * Time.deltaTime, 0);
        else
            m_tLevelsHolder.localPosition += new Vector3(0, -0.1f * Time.deltaTime, 0);

        if (m_tLevelsHolder.localPosition.x > fXPlus || m_tLevelsHolder.localPosition.x < fXMoins)
            m_nXSens *= -1;

        if (m_tLevelsHolder.localPosition.y > fYPlus || m_tLevelsHolder.localPosition.y < fYMoins)
            m_nYSens *= -1;

        if (m_nRotateSens > 0)
            m_tLevelsHolder.localEulerAngles += new Vector3(0, 0, 2f * Time.deltaTime);
        else
            m_tLevelsHolder.localEulerAngles += new Vector3(0, 0, -2f * Time.deltaTime);

        if (m_tLevelsHolder.localEulerAngles.z > m_fRotatePlus || m_tLevelsHolder.localEulerAngles.z < m_fRotateMoins)
            m_nRotateSens *= -1;

        //Actions
        if (!m_bIsMoving && !m_bIsChangingScene && m_bActionsEnable)
        {
            //Les mondes 1, 2 et 3 dont des niveaux linéaires, on peut aller que de droite à gauche ou de gauche à droite
            if (m_nSelectedWorldIndex < 3)
            {
                if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout)) && m_nLevelSelected < m_nLevelCOunt - 1 && DataManagerController.instance.GetLevel(m_nSelectedWorldIndex, m_nLevelSelected + 1).m_bUnlocked)
                {
                    SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_CHANGE_WORLD);

                    StartCoroutine(ChangeSelectedLevel(m_nLevelSelected + 1, 0.5f));
                }
                else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout)) && m_nLevelSelected > 0)
                {
                    SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_CHANGE_WORLD);

                    StartCoroutine(ChangeSelectedLevel(m_nLevelSelected - 1, 0.5f));
                }
            }
            //Le 3ème niveau est différent, on peut aller au milieu puis à droite/gauche/bas
            //Si je rajoute plus de niveau de ce style, il faudra refaire la gestion des touches pour pouvoir gérer tous les déplaements au lieu de gérer les déplacements de chaque niveaux vers les autres individuellement
            //Mais il faudrait aussi revoir la gestion des positions des niveaux dans LevelManager pour gérer ça, donc c'est pas utile s'il n'y a pas plus de niveau
            else if (m_nSelectedWorldIndex == 3)
            {
                //La touche droite permet de se déplacer 0 -> 1 ou 1-> 3
                if(Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout)) && (m_nLevelSelected == 0 || m_nLevelSelected == 1))
                {
                    if (m_nLevelSelected == 0)      StartCoroutine(ChangeSelectedLevel(1, 0.5f));
                    else if(m_nLevelSelected == 1)  StartCoroutine(ChangeSelectedLevel(3, 0.5f));
                }
                //La touche droite permet de se déplacer 3 -> 1 ou 1-> 0
                else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout)) && (m_nLevelSelected == 1 || m_nLevelSelected == 3))
                {
                    if (m_nLevelSelected == 1)      StartCoroutine(ChangeSelectedLevel(0, 0.5f));
                    else if (m_nLevelSelected == 3) StartCoroutine(ChangeSelectedLevel(1, 0.5f));
                }
                else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout)) && m_nLevelSelected == 1)//La touche UP permet de se déplacer 1 -> 2
                    StartCoroutine(ChangeSelectedLevel(2, 0.5f));
                else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout)) && m_nLevelSelected == 2)//La touche DOWN permet de se déplacer 2 -> 1
                    StartCoroutine(ChangeSelectedLevel(1, 0.5f));
            }

            if ((Input.GetKeyDown(StaticResources.KEY_ACTION) || Input.GetKey(StaticResources.Key_ACTION_2)) && DataManagerController.instance.GetLevel(m_nSelectedWorldIndex, m_nLevelSelected).m_bUnlocked)
            {
                SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_LONG);

                StartCoroutine(LoadGameSceneScene(1.0f));
            }

            if (Input.GetKey(StaticResources.KEY_MENU))
            {
                StartCoroutine(LoadMenuWorldScene());
            }
        }
    }

    private void InitLevels()
    {
        foreach(Level lvl in m_lstLevels)
        {
            GameObject obj = null;

            switch (lvl.GetLevelType())
            {
                case TYPE_LEVEL.CARBON: obj = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_LEVEL_CARBON)); break;
                case TYPE_LEVEL.HYDROGEN: obj = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_LEVEL_HYDROGEN)); break;
                case TYPE_LEVEL.OXYGEN: obj = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_LEVEL_OXYGEN)); break;
                case TYPE_LEVEL.SULFUR: obj = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_LEVEL_SULFUR)); break;
            }

            obj.transform.SetParent(m_tLevelsHolder);
            obj.transform.localPosition = lvl.GetPositionMenu();

            if(lvl.GetIndex() != 0)
            {
                GameObject goPointilles = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_LEVEL_POINTILLES));
                goPointilles.transform.SetParent(m_tLevelsHolder);

                Quaternion quart = LevelManager.GetWorld(DataManagerController.instance.m_nSelectedWorld).GetLiaison(lvl.GetIndex() - 1);

                Vector3 vPos = new Vector3(quart.x, quart.y, quart.z);

                goPointilles.transform.localPosition = vPos;
                goPointilles.transform.localEulerAngles = new Vector3(0, 0, quart.w);

                if (!DataManagerController.instance.GetLevel(m_nSelectedWorldIndex, lvl.GetIndex()).m_bUnlocked)
                    goPointilles.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.05f);
            }

            if (!DataManagerController.instance.GetLevel(m_nSelectedWorldIndex, lvl.GetIndex()).m_bUnlocked)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
            }
        }
    }

    private void CreateTraitFond()
    {
        Utilitaire.CreateTraitFond(4f, 0.5f);
    }

    #region Coroutines
    private IEnumerator ChangeSelectedLevel(int nLevelDestination, float fDuration = 1f)
    {
        m_bIsMoving = true;

        m_tPlayer.GetComponent<Animator>().Play(StaticResources.ANIMATION_PLAYER_MENU_2_UNSCALE);

        m_canvasCtrl.LevelLabelDisappear();

        m_nLevelSelected = nLevelDestination;

        float fDeltaX = m_tPlayer.localPosition.x - m_lstLevels[m_nLevelSelected].GetPositionMenu().x;
        float fDeltaY = m_tPlayer.localPosition.y - m_lstLevels[m_nLevelSelected].GetPositionMenu().y;

        float fScale = 1 / fDuration;

        //Mouvement d'un niveau à un autre
        for (float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            float fX = Time.deltaTime * -fDeltaX * fScale;
            float fY = Time.deltaTime * -fDeltaY * fScale;

            Vector3 vPos = m_tPlayer.localPosition;

            m_tPlayer.localPosition = new Vector3(vPos.x + fX, vPos.y + fY, 2);

            yield return null;
        }

        //On s'assure que la position soit exacte
        m_tPlayer.localPosition = new Vector3(m_lstLevels[m_nLevelSelected].GetPositionMenu().x, m_lstLevels[m_nLevelSelected].GetPositionMenu().y, 2);

        StartCoroutine(WaitAndChangeText(m_lstLevels[m_nLevelSelected].GetName()));

        m_bIsMoving = false;
    }

    private IEnumerator WaitAndChangeText(string szText)
    {
        yield return new WaitForSeconds(0.05f);

        m_tPlayer.GetComponent<Animator>().Play(StaticResources.ANIMATION_PLAYER_MENU_2_UPSCALE);

        LevelSave lvlSave = DataManagerController.instance.GetLevel(m_nSelectedWorldIndex, m_nLevelSelected);

        if (lvlSave.m_bCompleted)
        {
            m_canvasCtrl.DisplayCompleted();
            m_canvasCtrl.DisplayScore(lvlSave.m_nScore);
        }
        else
            m_canvasCtrl.DisplayNotCompleted();

        m_canvasCtrl.ChangeLevelText(szText);
        m_canvasCtrl.LevelLabelAppear();
    }

    private IEnumerator LoadMenuWorldScene()
    {
        m_bIsChangingScene = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu_world");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadGameSceneScene(float fAnimationDuration = 1.0f)
    {
        m_canvasCtrl.EnableHidingCircle();

        m_bIsMoving = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;

        DataManagerController.instance.m_nSelectedLevel = m_nLevelSelected;
        DataManagerController.instance.m_nbCoucheSelectedLevel = m_lstLevels[m_nLevelSelected].GetNombreCouches();
        DataManagerController.instance.m_nbNucleonSelectedLevel = m_lstLevels[m_nLevelSelected].GetNombreNucleons();
        DataManagerController.instance.m_fDurationSelectedLevel = m_lstLevels[m_nLevelSelected].GetDuration();

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        m_canvasCtrl.DisableHidingCircleAnimator();

        float fScale = 1 / fAnimationDuration;

        float fHeightToGo = Screen.width * 4;

        float fHeightToAdd = fHeightToGo - m_canvasCtrl.GetHidingCircleRectTransform().rect.width;

        for (float f = fAnimationDuration; f >= 0; f -= Time.deltaTime)
        {
            float fHeightThisFrame = Time.deltaTime * fHeightToAdd * fScale;

            Rect rect = m_canvasCtrl.GetHidingCircleRectTransform().rect;
            
            m_canvasCtrl.SetHidingCircleSizeWithCurrentAnchors(rect.width + fHeightThisFrame, rect.width + fHeightThisFrame);

            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        m_bIsMoving = false;
    }

    private IEnumerator LoadGameSceneScene_WEBGL(float fAnimationDuration = 1.0f)
    {
        m_canvasCtrl.EnableHidingCircle();

        m_bIsMoving = true;

        /*AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;*/

        DataManagerController.instance.m_nSelectedLevel = m_nLevelSelected;
        DataManagerController.instance.m_nbCoucheSelectedLevel = m_lstLevels[m_nLevelSelected].GetNombreCouches();
        DataManagerController.instance.m_nbNucleonSelectedLevel = m_lstLevels[m_nLevelSelected].GetNombreNucleons();
        DataManagerController.instance.m_fDurationSelectedLevel = m_lstLevels[m_nLevelSelected].GetDuration();

        /*while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }*/

        m_canvasCtrl.DisableHidingCircleAnimator();

        float fScale = 1 / fAnimationDuration;

        float fHeightToGo = Screen.width * 4;

        float fHeightToAdd = fHeightToGo - m_canvasCtrl.GetHidingCircleRectTransform().rect.width;

        for (float f = fAnimationDuration; f >= 0; f -= Time.deltaTime)
        {
            float fHeightThisFrame = Time.deltaTime * fHeightToAdd * fScale;

            Rect rect = m_canvasCtrl.GetHidingCircleRectTransform().rect;

            m_canvasCtrl.SetHidingCircleSizeWithCurrentAnchors(rect.width + fHeightThisFrame, rect.width + fHeightThisFrame);

            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        /*asyncLoad.allowSceneActivation = true;*/

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

        float fScaleToAdd = 10 - m_canvasCtrl.GetHiddingCircleLocalScale().x;

        for (float f = fAnimationDuration; f >= 0; f -= Time.deltaTime)
        {
            float fScaleAdd = Time.deltaTime * fScaleToAdd * fScale;

            Vector3 vScale = m_canvasCtrl.GetHiddingCircleLocalScale();

            m_canvasCtrl.SetHiddingCircleLocalScale(new Vector3(vScale.x + fScaleAdd, vScale.y + fScaleAdd, vScale.z));

            yield return null;
        }

        m_canvasCtrl.SetHiddingCircleLocalScale(new Vector3(fScaleToGo, fScaleToGo, 1));

        StartCoroutine(LoadGameSceneScene());
    }

    private IEnumerator WaitBeforeEnableActions()
    {
        yield return new WaitForSeconds(0.5f);

        m_bActionsEnable = true;
    }
    #endregion
}
