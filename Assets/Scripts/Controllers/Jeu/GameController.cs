using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static float TAILLE_COUCHE_DEFAUT_BASE = 5.12f;

    public static int SCORE_PHOTON = 100;
    public static int SCORE_COEF = 2;
    public static int SCORE_FOR_SANTE = 1000;
    public static int SCORE_FOR_PLAYER_CHARGE = 1000;
    public static int SCORE_NOYAU_HIT = 5000;

    private readonly float m_fOffsetPhotonSpawn = 2;//Distance de spanw des photons par rapport aux bords
    private readonly int m_nMaxPlayerCharge = 3;
    private readonly int m_nMaxPlayerSante = 3;
    private readonly int m_nPauseButtonsCount = 3;
    private readonly float m_fShakeAmount = 0.1f;
    private readonly float m_fDeacreeaseFactor = 1.0f;

    //Jeu
    private float m_fAudioVolume;
    private int m_nNombreCouche = 2;
    private float m_fTailleCoucheDefaut = 5.12f;
    private int m_nScore = 0;
    private bool m_bNewRecord = false;
    private int m_nScoreFromReset = 0;
    private int m_nScoreFromPlayerCharge = 0;
    private float m_levelDuration = 60;//Durée du niveau en seconde
    public bool m_bIsInit = false;
    public bool m_IsGameEnded = false;
    private bool bIsChangingScene = false;
    private bool m_bAnimationBeforeStart = true;
    private bool m_bPause = false;
    private bool m_bCanPause = true;
    //Camera Shake
    private bool m_bIsShaking = false;
    private float m_fShakeDuration = 0f;
    private Vector3 m_vOriginalPos;
    //Menu
    private bool m_bPauseIsMoving = false;
    private int m_nPauseButtonSelected = 0;
    private int m_nMinButtonIndex = 0;

    private CanvasController m_canvasCtrl;
    private PlayerController m_playerCtrl;
    private NoyauController m_noyauCtrl;
    private AudioSource m_audioBackground;
    public IEnumerator coroutine_endScreen;

    public int m_nInitState = 0;
    private float fDeltaTime = 0;//Utilisé quand les fps ont besoin d'être affichés
    private bool m_bTutoDisplayer = false;
    private TUTORIEL m_tutoDisplayed = TUTORIEL.AUCUN;

    [SerializeField]
    private GameObject m_goNoyau;
    [SerializeField]
    private GameObject m_goPlayer;
    [SerializeField]
    private GameObject m_goCanvas;
    [SerializeField]
    private GameObject m_goEventSystem;
    [SerializeField]
    private GameObject m_goDataManager;
    [SerializeField]
    private GameObject m_goThemeManager;

    // Start is called before the first frame update
    void Start()
    {
        DataManagerController.instance.InitData();
        SoundManager.LoadAudioResources();

        Application.targetFrameRate = 60;

        Time.timeScale = 0;
        m_bPause = true;

        m_goCanvas.SetActive(true);
        m_canvasCtrl = m_goCanvas.GetComponent<CanvasController>();

        m_goEventSystem.SetActive(true);

        //TUTO ?
        TUTORIEL tutoLevel = LevelManager.LevelHasTutoriel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel);

        if (tutoLevel == TUTORIEL.AUCUN)
        {
            Time.timeScale = 1;
            StartCoroutine(BeforeStartAnimation());
            m_bTutoDisplayer = false;
        }
        else
        {
            m_bTutoDisplayer = true;
            switch (tutoLevel)
            {
                case TUTORIEL.PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE: ShowTutoPanel(TUTORIEL.PLAYER_PROGRESSION_PHOTON_POINT_ELECTRON_VIE); break;
                case TUTORIEL.COUCHE_ENERGIE: ShowTutoPanel(TUTORIEL.COUCHE_ENERGIE); break;
                case TUTORIEL.PLAYER_CHARGE: ShowTutoPanel(TUTORIEL.PLAYER_CHARGE); break;
                case TUTORIEL.PLAYER_GOD_MODE: ShowTutoPanel(TUTORIEL.PLAYER_GOD_MODE); break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Le chargement des éléments est fait sur 4 frames pour ne pas tout faire sur une suele frame et qu'elle ne dure trop longtemps
        if (!m_bIsInit) 
        {
            if (m_nInitState == 4)
                return;

            if (m_nInitState == 0)
            {
                m_goDataManager.SetActive(true);

                SoundManager.LoadAudioResources();
                ThemeManagerController.instance.MuteTheme();

                m_goPlayer.SetActive(true);
                m_playerCtrl = m_goPlayer.GetComponent<PlayerController>();
                m_playerCtrl.SetPlayerStop(true);
                
                m_goNoyau.SetActive(true);
                m_noyauCtrl = m_goNoyau.GetComponent<NoyauController>();

                m_nNombreCouche = DataManagerController.instance.m_nbCoucheSelectedLevel;

                World w = LevelManager.GetWorld(DataManagerController.instance.m_nSelectedWorld);
                string szWorld = w.GetName();
                string szLevel = w.GetLevel(DataManagerController.instance.m_nSelectedLevel).GetName();

                m_canvasCtrl.SetBeforeStartTexts((w.GetIndex() + 1) + " - " + szWorld, szLevel);

                m_nInitState = 1;
            }
            else if(m_nInitState == 1)
            {
                m_nNombreCouche = DataManagerController.instance.m_nbCoucheSelectedLevel;
                InitCouches();
                m_vOriginalPos = Camera.main.transform.localPosition;

                //On lance la création de photon seulement après le tutoriel
                if (LevelManager.LevelHasTutoriel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel) == TUTORIEL.AUCUN)
                {
                    StartPhotoGeneration();
                }

                InvokeRepeating("CreateTraitFond", 0f, 1f);
                CreateTraitFond();

                m_nInitState = 2;
            }
            else if (m_nInitState == 2)
            {
                m_levelDuration = DataManagerController.instance.m_fDurationSelectedLevel;
                m_noyauCtrl.InitNoyau(DataManagerController.instance.m_nbNucleonSelectedLevel);
                m_noyauCtrl.InitDuration(DataManagerController.instance.m_fDurationSelectedLevel);
                m_noyauCtrl.SetPatterns(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel);

                m_nInitState = 3;
            }
            else if (m_nInitState == 3)
            {
                m_playerCtrl.InitPlayer(m_nNombreCouche);

                UpdateScore();

                m_audioBackground = GetComponent<AudioSource>();
                m_fAudioVolume = m_audioBackground.volume;

                UpdateBackgroundAudioState();

                m_bPause = false;
                m_bIsInit = true;

                m_nInitState = 4;
            }

            return;
        }

        //Une fois l'initialisation, si il y a une page de tuto, on attend l'appuie sur la touche d'action
        if (m_bTutoDisplayer)
        {
            if (Input.GetKeyDown(StaticResources.Key_ACTION_2))
            {
                HideTutoPanel();

                Time.timeScale = 1;

                StartCoroutine(BeforeStartAnimation());
                m_bTutoDisplayer = false;
            }

            return;
        }

        //Ensuite on attends que l'animation de début soit terminée
        if (m_bAnimationBeforeStart)
        {
            return;
        }

        //Label des FPS pour les tests
        /*fDeltaTime += (Time.deltaTime - fDeltaTime) * 0.1f;
        float fps = 1.0f / fDeltaTime;

        if(Time.timeScale != 0)
            m_goCanvas.transform.Find(StaticResources.TRANSFORM_LABEL_FPS).GetComponent<Text>().text = Mathf.Ceil(fps).ToString();*/

        //Tremblement de l'écran
        if (m_bIsShaking)
        {
            if (m_fShakeDuration > 0)
            {
                Camera.main.transform.localPosition = m_vOriginalPos + Random.insideUnitSphere * m_fShakeAmount;

                m_fShakeDuration -= Time.deltaTime * m_fDeacreeaseFactor;
            }
            else
            {
                m_fShakeDuration = 0f;
                Camera.main.transform.localPosition = m_vOriginalPos;

                m_bIsShaking = false;
            }
        }

        //Appuie sur le bouton de pause
        if (Input.GetKey(StaticResources.KEY_MENU) && m_bCanPause)
        {
            m_bCanPause = false;

            if (!m_bPause)
            {
                Time.timeScale = 0;
                m_bPause = true;

                m_nMinButtonIndex = 1;
                m_nPauseButtonSelected = 1;

                StartCoroutine(DownVolume(0.5f));
                m_canvasCtrl.ShowPausePanel();
            }
            else
            {
                Time.timeScale = 1;
                m_bPause = false;

                StartCoroutine(UpVolume(0.5f));
                m_canvasCtrl.HidePausePanel();
            }

            StartCoroutine(WaitAndAllowPause());
        }

        //Gestion du menu de pause
        if (m_bPause)
        {
            //Changement de bouton
            if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout)) && !m_bPauseIsMoving && m_nPauseButtonSelected > m_nMinButtonIndex)
            {
                StartCoroutine(PauseSelectedButtonChanged(-1, 0.15f));
            }
            else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout)) && !m_bPauseIsMoving && m_nPauseButtonSelected < m_nPauseButtonsCount - 1)
            {
                StartCoroutine(PauseSelectedButtonChanged(1, 0.15f));
            }

            //Si le curseur n'est pas déjà en train de bouger
            if ((Input.GetKey(StaticResources.KEY_ACTION) || Input.GetKey(StaticResources.Key_ACTION_2)) && !m_bPauseIsMoving)
            {
                if (!m_IsGameEnded)
                {
                    if (m_nPauseButtonSelected == 1)
                    {
                        Time.timeScale = 1;
                        m_bPause = false;

                        StartCoroutine(UpVolume(0.5f));
                        m_canvasCtrl.HidePausePanel();
                    }
                    else
                    {
                        StartCoroutine(LoadLevelScene());
                    }
                }
                else
                {
                    if (m_nPauseButtonSelected == 0)
                    {
                        LevelManager.GetNextLevel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel, out int nNextWorld, out int nNextLevel);

                        LevelSave lvlSave = DataManagerController.instance.GetLevel(nNextWorld, nNextLevel);

                        if (lvlSave.m_nLevelIndex >= 0)
                        {
                            DataManagerController.instance.m_nSelectedWorld = nNextWorld;
                            DataManagerController.instance.m_nSelectedLevel = nNextLevel;

                            World w = LevelManager.GetWorld(nNextWorld);
                            Level l = w.GetLevel(nNextLevel);

                            DataManagerController.instance.m_nbCoucheSelectedLevel = l.GetNombreCouches();
                            DataManagerController.instance.m_nbNucleonSelectedLevel = l.GetNombreNucleons();
                            DataManagerController.instance.m_fDurationSelectedLevel = l.GetDuration();
                        }

                        //Puis on restart la scene pour charger le même niveau ou le prochain
                        Restart();
                    }
                    else if (m_nPauseButtonSelected == 1)
                    {
                        Restart();
                    }
                    else if(!bIsChangingScene)
                    {
                        LevelManager.GetNextLevel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel, out int nNextWorld, out int nNextLevel);

                        LevelSave lvlSave = DataManagerController.instance.GetLevel(nNextWorld, nNextLevel);

                        if (lvlSave.m_nLevelIndex >= 0)
                        {
                            DataManagerController.instance.m_nSelectedWorld = nNextWorld;
                            DataManagerController.instance.m_nSelectedLevel = nNextLevel;
                        }

                        StartCoroutine(LoadLevelScene());
                    }
                }
            }
        }
    }
    
    private void InitCouches()
    {
        float fScale = LevelManager.GetCoucheScale(m_nNombreCouche);

        m_fTailleCoucheDefaut = TAILLE_COUCHE_DEFAUT_BASE * fScale;

        for (int i = 0; i < m_nNombreCouche; i++)
        {
            GameObject goCouche = (GameObject)GameObject.Instantiate(Resources.Load(StaticResources.RESOURCE_COUCHE));
            goCouche.transform.localScale = new Vector3((i + 1) * fScale, (i + 1) * fScale, 1);
        }
    }

    private void CreatePhoton()
    {
        float fPosBordX1 = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        float fPosBordX2 = Camera.main.ScreenToWorldPoint(new Vector2((float)Screen.width, 0)).x;

        float fPosBordY1 = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float fPosBordY2 = Camera.main.ScreenToWorldPoint(new Vector2(0, (float)Screen.height)).y;

        float posX;
        float posY;

        if (Random.Range(0, 2) == 0)//Spawn à droite ou gauche
        {
            if (Random.Range(0, 2) == 0)
                posX = Random.Range(fPosBordX1 - m_fOffsetPhotonSpawn, fPosBordX1);
            else
                posX = Random.Range(fPosBordX2, fPosBordX2 + m_fOffsetPhotonSpawn);

            posY = Random.Range(fPosBordX1 - m_fOffsetPhotonSpawn, fPosBordX2 + m_fOffsetPhotonSpawn);
        }
        else//Spawn en haut ou en bas
        {
            if (Random.Range(0, 2) == 0)
                posY = Random.Range(fPosBordY1 - m_fOffsetPhotonSpawn, fPosBordY1);
            else
                posY = Random.Range(fPosBordY2, fPosBordY2 + m_fOffsetPhotonSpawn);

            posX = Random.Range(fPosBordY1 - m_fOffsetPhotonSpawn, fPosBordY2 + m_fOffsetPhotonSpawn);
        }

        GameObject go = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_PHOTON));

        Vector3 vectorPosition = new Vector3(posX, posY, go.transform.position.z);

        go.transform.position = vectorPosition;
    }

    private void CreateTraitFond()
    {
        Utilitaire.CreateTraitFond(m_fOffsetPhotonSpawn, 0.125f);
    }

    private void StartPhotoGeneration()
    {
        InvokeRepeating("CreatePhoton", 0f, 1f);
        CreatePhoton();
    }

    #region Getter

    public int GetMaxCharges()
    {
        return this.m_nMaxPlayerCharge;
    }

    public int GetMaxSante()
    {
        return this.m_nMaxPlayerSante;
    }

    public int GetCoucheCount()
    {
        return m_nNombreCouche;
    }

    public float GetTailleCouche()
    {
        return m_fTailleCoucheDefaut;
    }

    public bool IsPaused()
    {
        return m_bPause;
    }

    #endregion

    //Ajoute un de score
    public void AddScore(int nScore)
    {
        m_nScore += nScore;
        m_nScoreFromReset += nScore;
        m_nScoreFromPlayerCharge += nScore;

        if (m_nScoreFromReset >= SCORE_FOR_SANTE)
        {
            m_nScoreFromReset -= SCORE_FOR_SANTE;

            m_playerCtrl.AddSante();
        }

        if(m_nScoreFromPlayerCharge >= SCORE_FOR_PLAYER_CHARGE && !m_playerCtrl.IsInRecoveryMode() && !m_playerCtrl.HasPlayerCharge())
        {
            m_nScoreFromPlayerCharge -= SCORE_FOR_PLAYER_CHARGE;

            m_playerCtrl.GivePlayerCharge();
        }

        UpdateScore();
    }

    public void PlayerShotCharge()
    {
        m_nScoreFromPlayerCharge = 0;
    }

    #region Canvas et animations

    //Demande au Canvas de mettre à jour les charges du joueurs
    public void UpdatePlayerCharge(int nCurrentCharge)
    {
        m_canvasCtrl.DisplayCharges(m_nMaxPlayerCharge, nCurrentCharge);
    }

    //Demande au Canvas de mettre à jour la santé du joueur
    public void UpdatePlayerSante(int nCurrentSante)
    {
        m_canvasCtrl.DisplaySante(m_nMaxPlayerSante, nCurrentSante);
    }

    //Demande au canvas la mise à jour du score
    public void UpdateScore()
    {
        m_canvasCtrl.UpdateScore(m_nScore);
    }

    #endregion

    #region Fin de partie

    //Lance la séquence de fin de jeu (fondu)
    public void EndScreenFade(System.Func<bool> callbackFunction)
    {
        m_canvasCtrl.UI_HideAll();
        coroutine_endScreen = EndScreenFadeAnimation(callbackFunction);
        StartCoroutine(coroutine_endScreen);
    }

    //Relance la scene en cours
    public bool Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return true;
    }

    public bool EndGameScreen()
    {
        Time.timeScale = 0;
        m_bPause = true;

        StartCoroutine(DownVolume(0.5f));

        LevelManager.GetNextLevel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel, out int nNextWorld, out int nNextLevel);

        LevelSave lvl = DataManagerController.instance.GetLevel(nNextWorld, nNextLevel);

        if (m_playerCtrl.IsDead())
        {
            //Si le prochain niveau est débloqué, le bouton prochain niveau sera visible
            if (lvl.m_bUnlocked && lvl.m_nLevelIndex >= 0)
            {
                m_nPauseButtonSelected = 0;
                m_nMinButtonIndex = 0;
            }
            else
            {
                m_nPauseButtonSelected = 1;
                m_nMinButtonIndex = 1;
            }
        }
        else
        {
            if (lvl.m_nLevelIndex >= 0)
            {
                m_nPauseButtonSelected = 0;
                m_nMinButtonIndex = 0;
            }
            else
            {
                m_nPauseButtonSelected = 1;
                m_nMinButtonIndex = 1;
            }
        }

        m_canvasCtrl.ShowEndGamePanel(m_nMinButtonIndex == 0, m_playerCtrl.IsDead(), m_nScore, m_bNewRecord);

        return true;
    }

    #endregion

    #region Méthode qui lancent le shaking de l'écran

    public void PlayerHasBeenHit()
    {
        m_bIsShaking = true;
        m_fShakeDuration = 0.5f;
    }

    public void NoyauHasBeenHit()
    {
        m_bIsShaking = true;
        m_fShakeDuration = 0.5f;

        //Si on lance une player charge sur le noyau, on augmente le score
        AddScore(SCORE_NOYAU_HIT);
    }

    #endregion

    #region Gestion de la durée

    //La durée restante a changée
    public void NoyauLeftDurationChanged(float fDurationLeft)
    {
        float fLevelCompletion = GetLevelCompletion();
        m_canvasCtrl.UpdateLevelCompletion(fLevelCompletion);

        float musicPitch = 1f + (fLevelCompletion * 0.333f);
        m_audioBackground.pitch = musicPitch;

        CheckLevelComplete(fDurationLeft);
    }

    //Vérifie si le niveau est terminé
    public void CheckLevelComplete(float fNoyauLeftDuration)
    {
        if (fNoyauLeftDuration <= 0)
        {
            if (!m_IsGameEnded)
            {
                LevelSave lvlSave = DataManagerController.instance.GetLevel(DataManagerController.instance.m_nSelectedWorld, DataManagerController.instance.m_nSelectedLevel);

                m_bNewRecord = m_nScore > lvlSave.m_nScore;

                DataManagerController.instance.SetLevelCompleted(m_nScore);
            }

            m_IsGameEnded = true;

            SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_FIN_NOYAU);

            EndScreenFade(EndGameScreen);
        }
    }

    //Retourne la complétion du niveau
    public float GetLevelCompletion(float fValeur = -1)
    {
        float temp;

        if (fValeur == -1)
            temp = (m_levelDuration - m_noyauCtrl.GetDurationLeft()) / m_levelDuration;
        else
            temp = (m_levelDuration - fValeur) / m_levelDuration;

        return temp;
    }

    //Retourne la durée totale du niveau
    public float GetLevelDuration()
    {
        return m_levelDuration;
    }

    #endregion

    #region Tutoriels

    private void ShowTutoPanel(TUTORIEL tuto)
    {
        m_tutoDisplayed = tuto;
        m_canvasCtrl.SetTutoVisibility(m_tutoDisplayed, true);
    }

    private void HideTutoPanel()
    {
        m_canvasCtrl.SetTutoVisibility(m_tutoDisplayed, false);
        m_tutoDisplayed = TUTORIEL.AUCUN;
    }

    #endregion

    #region Coroutines

    //Animation de fondu pour la fin de jeu
    private IEnumerator EndScreenFadeAnimation(System.Func<bool> callbackFunction, float fScale = 1.0f)
    {
        for (float f = 1f / fScale / 2f; f >= 0; f -= Time.deltaTime)
        {
           m_canvasCtrl.ChangeBackgroundColor(new Color(107f / 255f, 107f / 255f, 110f / 255f, 1 - (f * fScale * 2f)));

            yield return null;
        }

        yield return new WaitForSeconds(1);

        callbackFunction();
    }

    private IEnumerator WaitAndAllowPause()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        m_bCanPause = true;
    }

    private IEnumerator BeforeStartAnimation(float fDuration = 1.0f)
    {
        yield return new WaitForSeconds(1.0f);

        float fScale = 1 / fDuration;

        float fScaleToGo = 1;

        for(float f = fDuration; f >= 0; f -= Time.deltaTime)
        {
            float fScaleAdd = Time.deltaTime * fScaleToGo * fScale;

            Vector3 vScale = m_canvasCtrl.GetBeforeStartPanelLocalScale();

            m_canvasCtrl.SetBeforeStartPanelLocalScale(new Vector3(vScale.x - fScaleAdd, vScale.y - fScaleAdd, vScale.z));

            yield return null;
        }

        m_canvasCtrl.SetBeforeStartPanelLocalScale(Vector3.zero);

        m_bAnimationBeforeStart = false;

        m_noyauCtrl.StartPatterns();
        StartPhotoGeneration();
        m_playerCtrl.SetPlayerStop(false);
    }

    //CHangement de position du curseur du menu de pause
    private IEnumerator PauseSelectedButtonChanged(int nSens, float fDuration = 1.0f)
    {
        m_bPauseIsMoving = true;

        float fScale = 1 / fDuration;

        int nNewSelectedButton = m_nPauseButtonSelected + nSens;

        float fXToGo = 50f;
        float fYToGo = 55f;

        float yBouton1;
        float yBouton2;

        int nButton_1;
        int nButton_2;

        if (nSens > 0)
        {
            nButton_1 = m_nPauseButtonSelected;
            nButton_2 = nNewSelectedButton;
        }
        else
        {
            nButton_1 = nNewSelectedButton;
            nButton_2 = m_nPauseButtonSelected;
        }

        yBouton1 = m_canvasCtrl.GetButtonLocalPosition(nButton_1).y;
        yBouton2 = m_canvasCtrl.GetButtonLocalPosition(nButton_2).y;

        for (float f = fDuration; f >= 0; f -= Time.unscaledDeltaTime)
        {
            float fXAdd = Time.unscaledDeltaTime * fXToGo * fScale;
            float fYAdd = Time.unscaledDeltaTime * fYToGo * fScale;

            Vector3 vPosContinuer = m_canvasCtrl.GetButtonLocalPosition(nButton_1);
            Vector3 vPosNiveaux = m_canvasCtrl.GetButtonLocalPosition(nButton_2);
            Vector3 vPosCurseur = m_canvasCtrl.GetCurseurLocalPosition();

            m_canvasCtrl.SetButtonLocalPosition(nButton_1, new Vector3(vPosContinuer.x - (fXAdd * nSens), yBouton1, vPosContinuer.z));
            m_canvasCtrl.SetButtonLocalPosition(nButton_2, new Vector3(vPosNiveaux.x + (fXAdd * nSens), yBouton2, vPosNiveaux.z));

            m_canvasCtrl.SetCurseurLocalPosition(new Vector3(vPosCurseur.x, vPosCurseur.y - (fYAdd * nSens), vPosCurseur.z));

            yield return null;
        }

        Vector3 vPosButton_1 = m_canvasCtrl.GetButtonLocalPosition(nButton_1);
        Vector3 vPosButton_2 = m_canvasCtrl.GetButtonLocalPosition(nButton_2);

        if (nSens > 0)
        {
            m_canvasCtrl.SetButtonLocalPosition(nButton_1, new Vector3(60, yBouton1, vPosButton_1.z));
            m_canvasCtrl.SetButtonLocalPosition(nButton_2, new Vector3(110, yBouton2, vPosButton_2.z));
        }
        else
        {
            m_canvasCtrl.SetButtonLocalPosition(nButton_1, new Vector3(110, yBouton1, vPosButton_1.z));
            m_canvasCtrl.SetButtonLocalPosition(nButton_2, new Vector3(60, yBouton2, vPosButton_2.z));
        }

        m_nPauseButtonSelected = nNewSelectedButton;

        float fCurseurY = ((m_nPauseButtonsCount - 1 - m_nPauseButtonSelected) * 55) + 45;

        Vector3 vCurseur = m_canvasCtrl.GetCurseurLocalPosition();
        m_canvasCtrl.SetCurseurLocalPosition(new Vector3(vCurseur.x, fCurseurY, vCurseur.z));

        m_bPauseIsMoving = false;
    }

    //Diminue le volume
    private IEnumerator DownVolume(float fDuration = 1.0f)
    {
        if(m_audioBackground.volume == 0)
        {
            m_fAudioVolume = 0.025f;
            yield break;
        }

        float fScale = 1 / fDuration;

        float fVolumeToGo = 0.075f;

        for (float f = fDuration; f >= 0; f -= Time.unscaledDeltaTime)
        {
            float fVolAdd = Time.unscaledDeltaTime * fVolumeToGo * fScale;

            GetComponent<AudioSource>().volume -= fVolAdd;

            yield return null;
        }

        GetComponent<AudioSource>().volume = 0.025f;

        m_fAudioVolume = 0.025f;
    }

    //Augmente le volume
    private IEnumerator UpVolume(float fDuration = 1.0f)
    {
        if (m_audioBackground.volume == 0)
        {
            m_fAudioVolume = 0.1f;
            yield break;
        }

        float fScale = 1 / fDuration;

        float fVolumeToGo = 0.075f;

        for (float f = fDuration; f >= 0; f -= Time.unscaledDeltaTime)
        {
            float fVolAdd = Time.unscaledDeltaTime * fVolumeToGo * fScale;

            GetComponent<AudioSource>().volume += fVolAdd;

            yield return null;
        }

        GetComponent<AudioSource>().volume = 0.1f;

        m_fAudioVolume = 0.1f;
    }

    private IEnumerator LoadLevelScene()
    {
        bIsChangingScene = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("menu_level");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    #endregion

    #region Audio

    public void MuteBackgroundMusic()
    {
        DataManagerController.instance.m_bMusicSound = false;
        m_audioBackground.volume = 0;
    }

    public void UnmuteBackgroundMusic()
    {
        DataManagerController.instance.m_bMusicSound = true;
        m_audioBackground.volume = m_fAudioVolume;
    }

    public void UpdateBackgroundAudioState()
    {
        if (DataManagerController.instance.m_bMusicSound)
            UnmuteBackgroundMusic();
        else
            MuteBackgroundMusic();

        SoundManager.PlayAudioSource(m_audioBackground);
    }

    #endregion
}
