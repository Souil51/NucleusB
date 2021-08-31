using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum AUDIO 
    { 
        AUDIO_PHOTON_DISAPPEAR, 
        AUDIO_NUCLEON_DIE, 
        AUDIO_PLAYER_CHARGE_HIT_NUCLEON, 
        AUDIO_RECOVERY,
        AUDIO_FIN_NOYAU,
        AUDIO_PLAYER_DIE,
        AUDIO_CHANGE_COUCHE,
        AUDIO_NOYAU_TAKE_DAMAGE,
        AUDIO_TIR_ELECTRON,
        AUDIO_UI_SHORT,
        AUDIO_UI_LONG,
        AUDIO_UI_CHANGE_WORLD,
        AUDIO_GIVE_PLAYER_CHARGE
    }

    private static AudioClip AudioPhotonDisappear;
    private static AudioClip AudioNucleonDie;
    private static AudioClip AudioPlayerChargeHitNucleon;
    private static AudioClip AudioRecovery;
    private static AudioClip AudioFinNoyau;
    private static AudioClip AudioPlayerDie;
    private static AudioClip AudioChangeCouche;
    private static AudioClip AudioNoyauTakeDamage;
    private static AudioClip AudioTirElectron;
    private static AudioClip AudioUIShort;
    private static AudioClip AudioUILong;
    private static AudioClip AudioUIChangeWorld;
    private static AudioClip AudioGivePlayerCharge;

    public static void MakeSound(AudioClip clip, Vector3 position, float fVolume = 1)
    {
        AudioSource.PlayClipAtPoint(clip, position, fVolume);
    }

    public static void LoadAudioResources()
    {
        AudioPhotonDisappear = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_PHOTON_DISAPPEAR);
        AudioNucleonDie = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_NUCLEON_DIE);
        AudioPlayerChargeHitNucleon = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_PLAYER_CHARGE_HIT_NUCLEON);
        AudioRecovery = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_RECOVERY);
        AudioFinNoyau = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_FIN_NOYAU);
        AudioPlayerDie = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_PLAYER_DIE);
        AudioChangeCouche = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_CHANGE_COUCHE);
        AudioNoyauTakeDamage = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_NOYAU_TAKE_DAMAGE);
        AudioTirElectron = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_TIR_ELECTRON);
        AudioUIShort = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_UI_SHORT);
        AudioUILong = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_UI_LONG);
        AudioUIChangeWorld = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_UI_CHANGE_WORLD);
        AudioGivePlayerCharge = Resources.Load<AudioClip>("Sounds/" + StaticResources.AUDIO_GIVE_PLAYER_CHARGE);
    }

    public static void PlaySound(AUDIO audio, float fVolume = 1)
    {
        if (!DataManagerController.instance.m_bFXSound)
            return;

        switch (audio)
        {
            case AUDIO.AUDIO_PHOTON_DISAPPEAR: MakeSound(AudioPhotonDisappear, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_NUCLEON_DIE: MakeSound(AudioNucleonDie, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_PLAYER_CHARGE_HIT_NUCLEON: MakeSound(AudioPlayerChargeHitNucleon, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_RECOVERY: MakeSound(AudioRecovery, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_FIN_NOYAU: MakeSound(AudioFinNoyau, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_PLAYER_DIE: MakeSound(AudioPlayerDie, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_CHANGE_COUCHE: MakeSound(AudioChangeCouche, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_NOYAU_TAKE_DAMAGE: MakeSound(AudioNoyauTakeDamage, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_TIR_ELECTRON: MakeSound(AudioTirElectron, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_UI_SHORT: MakeSound(AudioUIShort, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_UI_LONG: MakeSound(AudioUILong, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_UI_CHANGE_WORLD: MakeSound(AudioUIChangeWorld, Vector3.zero, fVolume); break;
            case AUDIO.AUDIO_GIVE_PLAYER_CHARGE: MakeSound(AudioGivePlayerCharge, Vector3.zero, fVolume); break;
        }
    }

    public static void PlayAudioSource(AudioSource audio)
    {
        if (!DataManagerController.instance.m_bFXSound)
            return;

        audio.Play();
    }
}
