using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class StaticResources
{
    public enum KeyboardLayout { AZERTY = 0, QWERTY = 1}

    public static string TAG_PHOTON = "photon";
    public static string TAG_ELECTRON = "Electron";
    public static string TAG_GAME_CONTROLLER = "GameController";
    public static string TAG_CANVAS = "Canvas";
    public static string TAG_PLAYER = "Player";
    public static string TAG_BARRIERE = "Barriere";
    public static string TAG_PLAYER_CHARGE = "PlayerCharge";
    public static string TAG_LASER = "Laser";
    public static string TAG_CERCLE_PLAYER = "CerclePlayer";
    public static string TAG_NOYAU = "Noyau";
    public static string TAG_LEVELS = "Levels";
    public static string TAG_COUCHE = "couche";

    public static string RESOURCE_CHARGE = "charge_UI";
    public static string RESOURCE_SANTE = "sante_UI";
    public static string RESOURCE_PHOTON = "photon";
    public static string RESOURCE_ELECTRON = "electron";
    public static string RESOURCE_BARRIERE = "Barriere";
    public static string RESOURCE_LASER = "Laser";
    public static string RESOURCE_PARTICULE_NOYAU_OBJET = "particule_noyau_object";
    public static string RESOURCE_NUCELON = "Nucleon_Holder";
    public static string RESOURCE_TRAIT_FOND = "trait_fond";
    public static string RESOURCE_COUCHE = "couche";
    public static string RESOURCE_PLAYER_CHARGE = "PlayerCharge";
    public static string RESOURCE_LEVEL_OXYGEN = "oxygen";
    public static string RESOURCE_LEVEL_HYDROGEN = "hydrogen";
    public static string RESOURCE_LEVEL_CARBON = "carbon";
    public static string RESOURCE_LEVEL_SULFUR = "sulfur";
    public static string RESOURCE_LEVEL_POINTILLES = "pointilles";

    public static string ANIMATION_GET_PHOTON_RED = "player_get_photon_red";
    public static string ANIMATION_GET_PHOTON_BLUE = "player_get_photon_blue";
    public static string ANIMATION_WIN_CHARGE = "win_charge";
    public static string ANIMATION_LOSE_CHARGE = "lose_charge";
    public static string ANIMATION_WIN_SANTE = "win_sante";
    public static string ANIMATION_LOSE_SANTE = "lose_sante";
    public static string ANIMATION_IDLE = "Idle";
    public static string ANIMATION_ANIMATION = "animation";
    public static string ANIMATION_COUCHE_WARNING = "couche_warning";
    public static string ANIMATION_NUCLEON_TO_PLAYER = "nucleon_to_player";
    public static string ANIMATION_ELECTRON_STOPPED = "electron_stopped";
    public static string ANIMATION_PLAYER_CHARGE_HAS_BEEN_HIT = "player_charge_has_been_hit";
    public static string ANIMATION_RECOVERY_MODE = "recovery_mode";
    public static string ANIMATION_DISPARITION = "disparition";
    public static string ANIMATION_GOD_MODE_HAS_BEEN_HIT = "god_mode_has_been_hit";
    public static string ANIMATION_LEVEL_LABEL_DISAPPEAR = "level_label_disappear";
    public static string ANIMATION_LEVEL_LABEL_APPEAR = "level_label_appear";
    public static string ANIMATION_PLAYER_MENU_2_UNSCALE = "PlayerMenu_2Unscale";
    public static string ANIMATION_PLAYER_MENU_2_UPSCALE = "PlayerMenu_2Upscale";

    public static string AUDIO_PHOTON_DISAPPEAR = "photon_disappear";
    public static string AUDIO_NUCLEON_DIE = "nucleon_die";
    public static string AUDIO_PLAYER_CHARGE_HIT_NUCLEON = "player_charge_hit_nucleon";
    public static string AUDIO_RECOVERY = "recovery";
    public static string AUDIO_FIN_NOYAU = "fin_noyau";
    public static string AUDIO_PLAYER_DIE = "player_die";
    public static string AUDIO_CHANGE_COUCHE = "change_couche";
    public static string AUDIO_NOYAU_TAKE_DAMAGE = "noyau_take_damage";
    public static string AUDIO_TIR_ELECTRON = "tir_electron";
    public static string AUDIO_UI_SHORT = "UI_SHORT";
    public static string AUDIO_UI_LONG = "UI_LONG";
    public static string AUDIO_UI_CHANGE_WORLD = "UI_CHANGE_WORLD";
    public static string AUDIO_GIVE_PLAYER_CHARGE = "give_player_charge";

    public static string TRANSFORM_PANEL_PAUSE = "pnlPause";
    public static string TRANSFORM_PANEL_BOUTONS = "pnlBoutons";
    public static string TRANSFORM_PANEL_SCORE = "pnlScore";
    public static string TRANSFORM_LABEL_SCORE = "txtScore";
    public static string TRANSFORM_SLIDER_COMPLETION = "SliderLevelCompletion";
    public static string TRANSFORM_PAUSE = "Pause";
    public static string TRANSFORM_RESUME = "Resume";
    public static string TRANSFORM_LABEL_NEW_RECORD = "text_new_record";
    public static string TRANSFORM_PANEL_BEFORE_START = "BeforeStartPanel";
    public static string TRANSFORM_CHARGE_PLEINE = "charge_UI_pleine";
    public static string TRANSFORM_CHARGE_CONTOUR = "charge_UI_contour";
    public static string TRANSFORM_COUCHE_COLLIDER = "couche_collider";
    public static string TRANSFORM_LABEL_FPS = "FPS_text";
    public static string TRANSFORM_FLECHE = "fleche";
    public static string TRANSFORM_FLECHE_MASK = "fleche_mask";
    public static string TRANSFORM_TEXT = "Text";
    public static string TRANSFORM_PANEL = "Panel";
    public static string TRANSFORM_PANEL_TUTO_PROGRESSION = "pnlTutoProgression";
    public static string TRANSFORM_PANEL_TUTO_PLAYER = "pnlTutoPlayer";
    public static string TRANSFORM_PANEL_TUTO_POINTS = "pnlTutoPoints";
    public static string TRANSFORM_PANEL_TUTO_PHOTON = "pnlTutoPhoton";
    public static string TRANSFORM_PANEL_TUTO_VIE = "pnlTutoVie";
    public static string TRANSFORM_PANEL_TUTO_ELECTRON = "pnlTutoElectron";
    public static string TRANSFORM_PANEL_TUTO_ENERGIE = "pnlTutoEnergie";
    public static string TRANSFORM_PANEL_TUTO_COUCHE = "pnlTutoCouche";
    public static string TRANSFORM_PANEL_TUTO = "pnlTuto";
    public static string TRANSFORM_PANEL_TUTO_PLAYER_CHARGE = "pnlTutoPlayerCharge";
    public static string TRANSFORM_FOND = "Fond";
    public static string TRANSFORM_BACKGROUND = "background";
    public static string TRANSFORM_NUCLEON = "Nucleon";
    public static string TRANSFORM_PART_SUSTEM = "PartSystem";
    public static string TRANSFORM_PHOTON_QUEUE = "photon_queue";
    public static string TRANSFORM_CERCLE_PLAYER_CHARGE = "cerlce_player_charge";
    public static string TRANSFORM_PLAYER_CHARGE_HOLDER = "player_charge_holder";
    public static string TRANSFORM_PLAYER_CHARGE = "player_charge";
    public static string TRANSFORM_CERCLE_GOD_MODE = "cerlce_player_god_mode";
    public static string TRANSFORM_QUEUE = "queue";
    public static string TRANSFORM_SANTE_PLEINE = "sante_UI_Pleine";
    public static string TRANSFORM_SANTE_CONTOUR = "sante_UI_Contour";
    public static string TRANSFORM_LABEL_WORLD = "world_label";
    public static string TRANSFORM_LABEL_COMPLETED = "completed_label";
    public static string TRANSFORM_CIRCLE_HIDING = "CircleHiding";
    public static string TRANSFORM_PLAYER = "Player";
    public static string TRANSFORM_BOUTON_CONTINUE = "Continuer";
    public static string TRANSFORM_BOUTON_NIVEAUX = "Niveaux";
    public static string TRANSFORM_BOUTON_OPTIONS = "Options";
    public static string TRANSFORM_BOUTON_QUITTER = "Quitter";
    public static string TRANSFORM_MENU_CURSEUR = "Curseur";
    public static string TRANSFORM_CIRCLE_IMAGE = "CircleImage";
    public static string TRANSFORM_LABEL_LEVEL = "level_label";
    public static string TRANSFORM_LABEL_LOCKED = "locked_label";
    public static string TRANSFORM_PLAYER_SPRITE = "PlayerSprite";
    public static string TRANSFORM_LEVEL = "level";
    public static string TRANSFORM_SOUND_ON = "sound_on";
    public static string TRANSFORM_SOUND_OFF = "sound_off";
    public static string TRANSFORM_PANEL_MUSIC = "music";
    public static string TRANSFORM_PANEL_FX = "fx";
    public static string TRANSFORM_LABEL_SOUND_ON = "text_sound_on";
    public static string TRANSFORM_BOUTON_RECOMMENCER = "Recommancer";
    public static string TRANSFORM_LABEL_ALL_UNLOCKED = "lblAllUnlocked";

    public static KeyCode KEY_UP = KeyCode.Z;
    public static KeyCode KEY_DOWN = KeyCode.S;
    public static KeyCode KEY_LEFT = KeyCode.Q;
    public static KeyCode KEY_RIGHT = KeyCode.D;

    public static KeyCode KEY_UP_QWERTY = KeyCode.W;
    public static KeyCode KEY_DOWN_QWERTY = KeyCode.S;
    public static KeyCode KEY_LEFT_QWERTY = KeyCode.A;
    public static KeyCode KEY_RIGHT_QWERTY = KeyCode.D;

    public static KeyCode KEY_ACTION = KeyCode.E;
    public static KeyCode Key_ACTION_2 = KeyCode.Space;
    public static KeyCode KEY_MENU = KeyCode.Escape;
    public static KeyCode KEY_MUTE_MUSIC = KeyCode.M;
    public static KeyCode KEY_MUTE_FX = KeyCode.L;
    public static KeyCode KEY_UP_2 = KeyCode.UpArrow;
    public static KeyCode KEY_DOWN_2 = KeyCode.DownArrow;
    public static KeyCode KEY_LEFT_2 = KeyCode.LeftArrow;
    public static KeyCode KEY_RIGHT_2 = KeyCode.RightArrow;
    public static KeyCode KEY_SECRET_A = KeyCode.A;
    public static KeyCode KEY_SECRET_B = KeyCode.B;
    public static KeyCode KEY_LAYOUT = KeyCode.K;

    public static KeyCode GetKeyCode(KeyCode keyCode, StaticResources.KeyboardLayout layout)
    {
        switch(keyCode)
        {
            case KeyCode.Z: return layout == KeyboardLayout.QWERTY ? KEY_UP_QWERTY : keyCode;
            case KeyCode.S: return layout == KeyboardLayout.QWERTY ? KEY_DOWN_QWERTY : keyCode;
            case KeyCode.Q: return layout == KeyboardLayout.QWERTY ? KEY_LEFT_QWERTY : keyCode;
            case KeyCode.D: return layout == KeyboardLayout.QWERTY ? KEY_RIGHT_QWERTY : keyCode;
            default: return keyCode;
        }
    }
}
