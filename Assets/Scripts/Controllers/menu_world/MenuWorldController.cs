using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuWorldController : MonoBehaviour
{
    private GameObject m_levels;
    private GameObject m_playerMenu;

    private Transform m_tPlayerSprite;
    private Transform m_tCouche;

    private CanvasMenuWorldController m_canvasCtrl;

    private List<World> m_lstWorlds = new List<World>();
    private readonly List<Transform> m_lstWorldsTransform = new List<Transform>();

    private int m_nSelectedLevel = 0;
    private float m_fAngleMenu;

    private bool m_bIsMoving = true;
    private bool m_bIsChangingScene = false;

    // Start is called before the first frame update
    void Start()
    {
        m_lstWorlds = LevelManager.GetAllWorlds();

        //Récupération des objets
        m_tCouche = GameObject.FindGameObjectWithTag(StaticResources.TAG_COUCHE).transform;
        m_levels = GameObject.FindGameObjectWithTag(StaticResources.TAG_LEVELS);
        m_playerMenu = GameObject.FindGameObjectWithTag(StaticResources.TAG_PLAYER);
        m_tPlayerSprite = m_playerMenu.transform.Find(StaticResources.TRANSFORM_PLAYER_SPRITE);
        GameObject goCanvas = GameObject.FindGameObjectWithTag(StaticResources.TAG_CANVAS);
        m_canvasCtrl = goCanvas.GetComponent<CanvasMenuWorldController>();

        //Initialisation
        m_canvasCtrl.DisableLabel();
        m_canvasCtrl.LevelLabelDisappear();
        m_tPlayerSprite.localScale = new Vector3(0, 0, 1);
        m_tCouche.localScale = new Vector3(0, 0, 1);

        InitWorlds();
        m_fAngleMenu = 360 / m_lstWorlds.Count;
        StartCoroutine(WaitAndChangeText(m_lstWorlds[0].GetName()));
        HighlightLevel(0);
        DisplayAll();

        InvokeRepeating("CreateTraitFond", 0f, 0.5f);
        CreateTraitFond();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bIsMoving && !m_bIsChangingScene)
        {
            if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout)))
            {
                SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_CHANGE_WORLD);

                StartCoroutine(MoveAngle(-1, 0.5f));
            }
            else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout)))
            {
                SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_CHANGE_WORLD);

                StartCoroutine(MoveAngle(1, 0.5f));
            }

            if ((Input.GetKey(StaticResources.KEY_ACTION) || Input.GetKey(StaticResources.Key_ACTION_2)) && DataManagerController.instance.m_save.GetWorld(m_nSelectedLevel).m_bUnlocked)
            {
                SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_UI_LONG);

                StartCoroutine(LoadMenuLevelScene());
            }

            if (Input.GetKey(StaticResources.KEY_MENU))
            {
                StartCoroutine(LoadMenuTitleScene());
            }
        }
    }

    private void CreateTraitFond()
    {
        Utilitaire.CreateTraitFond(4f, 0.5f);
    }

    private void DisplayAll()
    {
        StartCoroutine(DisplayAllAnimation(0.3f));
    }

    //Changement le level "éclairé"
    private void HighlightLevel(int nLevel)
    {
        if(DataManagerController.instance.m_save.GetWorld(m_nSelectedLevel).m_bUnlocked)
            m_lstWorldsTransform[m_nSelectedLevel].Find(StaticResources.TRANSFORM_LEVEL).GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 0.25f);

        if (DataManagerController.instance.m_save.GetWorld(nLevel).m_bUnlocked)
            m_lstWorldsTransform[nLevel].Find(StaticResources.TRANSFORM_LEVEL).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        m_nSelectedLevel = nLevel;
    }

    private void InitWorlds()
    {
        foreach(World w in m_lstWorlds)
        {
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(w.GetGOName()));
            go.transform.SetParent(m_levels.transform);
            go.transform.position = m_lstWorlds[w.GetIndex()].GetPositionMenu();
            m_lstWorldsTransform.Add(go.transform);

            go.transform.localScale = new Vector3(0, 0, 1);

            if (!DataManagerController.instance.m_save.GetWorld(w.GetIndex()).m_bUnlocked)
                go.transform.Find(StaticResources.TRANSFORM_LEVEL).GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
            else
                go.transform.Find(StaticResources.TRANSFORM_LEVEL).GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 0.25f);
        }
    }

    #region Coroutines
    private IEnumerator MoveAngle(int nSens, float fDuration = 1)
    {
        bool bPosInitiale_Y = false;

        m_bIsMoving = true;//On indique qu'on est en train de bouger pour bloquer le mouvement pendant l'animation

        //Pendant qu'on bouge, on désactive l'animation
        m_tPlayerSprite.GetComponent<Animator>().enabled = false;

        m_canvasCtrl.LevelLabelDisappear();
        
        //Changement de l'index sélectionné
        int nNewLevelSelcted = m_nSelectedLevel + nSens;

        if (nNewLevelSelcted == -1)
            nNewLevelSelcted = m_lstWorlds.Count - 1;
        else if (nNewLevelSelcted == m_lstWorlds.Count)
            nNewLevelSelcted = 0;

        //Le mouvement à faire sur le Y pour remettre le PlayerMenu à 2.5f
        float fDifference = m_tPlayerSprite.localPosition.y - 2.5f;

        float fScale = 1 / fDuration;

        //Comme le changement d'angle peut intervenir à n'importe quel moment de l'animation du Player
        //On remettra le PlayerMenu à sa position initiale pendant le mouvement, mais plus rapidement 
        int nRapportInnerAnim = 5;
        float fDurationPart = fDuration / nRapportInnerAnim;

        for(float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            //Tant qu'on a pas remis le PlayerMenu à sa position
            if (f > fDuration - fDurationPart)
            {
                m_tPlayerSprite.localPosition = new Vector3(0, m_tPlayerSprite.localPosition.y - (Time.deltaTime * fDifference * nRapportInnerAnim * fScale), -1);
            }
            else if (m_tPlayerSprite.localPosition.y != 2.5f && !bPosInitiale_Y)
            {
                //Dès qu'on a dépassé le 5ème de l'animation, on s'assure de la position exacte du PlayerMenu
                m_tPlayerSprite.localPosition = new Vector3(0, 2.5f, -1);
                bPosInitiale_Y = true;
            }

            //On change l'angle en fonction de la durée de la dernière frame
            float fAngleAdded = Time.deltaTime * m_fAngleMenu * fScale * nSens;

            Vector3 vEuler = m_playerMenu.gameObject.transform.localEulerAngles;

            m_playerMenu.transform.localEulerAngles = new Vector3(vEuler.x, vEuler.y, vEuler.z + fAngleAdded);

            yield return null;
        }

        m_playerMenu.transform.localEulerAngles = new Vector3(0, 0, nNewLevelSelcted * m_fAngleMenu);

        //On réinitialise l'animator pour repartir de la position de base du PlayerMenu
        m_tPlayerSprite.GetComponent<Animator>().Rebind();
        m_tPlayerSprite.GetComponent<Animator>().enabled = true;

        //On change le level surligné
        HighlightLevel(nNewLevelSelcted);

        StartCoroutine(WaitAndChangeText(m_lstWorlds[m_nSelectedLevel].GetName()));

        m_bIsMoving = false;
    }

    private IEnumerator WaitAndChangeText(string szText)
    {
        yield return new WaitForSeconds(0.05f);

        if (!DataManagerController.instance.m_save.GetWorld(m_nSelectedLevel).m_bUnlocked)
            m_canvasCtrl.DisplayNotUnlocked();
        else
            m_canvasCtrl.HideNotUnlocked();

        m_canvasCtrl.ChangeLevelText(szText);
        m_canvasCtrl.LevelLabelAppear();
    }

    private IEnumerator LoadMenuLevelScene()
    {
        DataManagerController.instance.m_nSelectedWorld = m_nSelectedLevel;

        //Si le prochain niveau à compléter est dans ce monde, on le sélectionne automatiquement. Sinon on sélectionne le premier niveau du monde
        SaveManager.GetLevelToComplete(DataManagerController.instance.m_save, out int nWorld, out int nLevel);
        DataManagerController.instance.m_nSelectedLevel = nWorld == DataManagerController.instance.m_nSelectedWorld ? nLevel : 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu_level");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator DisplayAllAnimation(float fDuration = 1.0f)
    {
        yield return new WaitForSeconds(0.1f);

        m_tPlayerSprite.GetComponent<Animator>().enabled = false;
        m_tPlayerSprite.localPosition = new Vector3(0, 2.5f, m_tPlayerSprite.localPosition.z);

        //Apparition de la couche

        float fScale = 1 / fDuration;

        float fScaleToGo = 0.5f;

        for (float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            //On change l'angle en fonction de la durée de la dernière frame
            float fScaleAdd = Time.deltaTime * fScaleToGo * fScale;

            Vector3 vScale = m_tCouche.localScale;

            m_tCouche.transform.localScale = new Vector3(vScale.x + fScaleAdd, vScale.y + fScaleAdd, vScale.z);

            yield return null;
        }

        //Apparition du Player et des levels
        float fPlayerScaleToGo = 0.8f;
        float fLevelsScaleToGo = 0.2f;

        for (float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            //On change l'angle en fonction de la durée de la dernière frame
            float fPlayerScaleAdd = Time.deltaTime * fPlayerScaleToGo * fScale;
            float fLevelsScaleAdd = Time.deltaTime * fLevelsScaleToGo * fScale;

            Vector3 vPlayerScale = m_tPlayerSprite.localScale;
            Vector3 vLevelsScale = m_lstWorldsTransform[0].localScale;

            m_tPlayerSprite.localScale = new Vector3(vPlayerScale.x + fPlayerScaleAdd, vPlayerScale.y + fPlayerScaleAdd, vPlayerScale.z);

            foreach (Transform t in m_lstWorldsTransform)
            {
                t.localScale = new Vector3(vLevelsScale.x + fLevelsScaleAdd, vLevelsScale.y + fLevelsScaleAdd, vLevelsScale.z);
            }

            yield return null;
        }

        m_tPlayerSprite.GetComponent<Animator>().Rebind();
        m_tPlayerSprite.GetComponent<Animator>().enabled = true;

        m_canvasCtrl.EnabledLabel();

        m_bIsMoving = false;
    }

    private IEnumerator LoadMenuTitleScene()
    {
        m_bIsChangingScene = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu_title");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    #endregion
}
