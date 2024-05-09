using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePlaySounds
{
    grinding, welding, shooting, painting, paintPickup, grinderPickup, cash, gate, obstacle, woodHit, weldShot, hammerSound, explosion, rocket,
    IceHit, IceCrack, liquidFill, fuse, fireworks, destruction,downgrade, gift,WeaponEquip, WeaponBreak, Thud, Slice, Punch1, Punch2, LandscapeDestroyed,
    Hurt1, Hurt2, Hurt3, Hurt4, BackSlap, OneLandscapeDestroyed, Kick, EnemyLaugh, CrowdCheers
}

public enum UISounds
{
    buttonClick, itemEquip, itemReveal, loadScreenWeld, upgrade, toggle
}


public class AudioLibrary : MonoBehaviour
{
    [Header("Audio Sources")]
    public List<AudioSource> gameplaySource = new List<AudioSource>();
    public List<AudioSource> uISource = new List<AudioSource>();

    [Header("Audio Clips")]
    public List<string> gameplaySounds = new List<string>();
    public List<string> uISounds = new List<string>();

    public List<bool> loopingGameplayBools = new List<bool>();

    List<AudioSource> pausedSounds = new List<AudioSource>();


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////       SETUP        /////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Setup
    void Start()
    {
        GetGameplayBoolSize();
    }

    void GetGameplayBoolSize()
    {
        for (int i = 0; i < gameplaySounds.Count; i++)
        {
            loopingGameplayBools.Add(false);
        }
    }

    #endregion

                
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////    GAMEPLAY SOUNDS      ///////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Gameplay sounds
    public void PlayGameplaySound(GamePlaySounds sound, bool isLooping = false, bool reduceVolume = false)
    {
        switch (isLooping)
        {
            case true:
                for (int i = 0; i < gameplaySounds.Count; i++)
                {
                    switch (gameplaySounds[i] == sound.ToString())
                    {
                        case true:
                            if (loopingGameplayBools[i]) { return; }
                            PlayLoopingGameplaySound(i, isLooping);
                            break;
                        case false:
                            break;
                    }
                }
                break;
            case false:
                for (int i = 0; i < gameplaySounds.Count; i++)
                {
                    switch (gameplaySounds[i] == sound.ToString())
                    {
                        case true:
                            PlayGameplaySound(i);
                            break;
                        case false:
                            break;
                    }
                }
                break;
        }
    }

    public void PlayGameplaySoundAtSource(GamePlaySounds sound, AudioSource audioSource, bool isLooping = false)
    {
        switch (isLooping)
        {
            case true:
                for (int i = 0; i < gameplaySounds.Count; i++)
                {
                    switch (gameplaySounds[i] == sound.ToString())
                    {
                        case true:
                            if (loopingGameplayBools[i]) { return; }
                            PlayLoopingGameplaySoundAtGivenSource(i, isLooping, audioSource);
                            break;
                        case false:
                            break;
                    }
                }
                break;
            case false:
                for (int i = 0; i < gameplaySounds.Count; i++)
                {
                    switch (gameplaySounds[i] == sound.ToString())
                    {
                        case true:
                            PlayGameplaySoundAtGivenSource(i, audioSource);
                            break;
                        case false:
                            break;
                    }
                }
                break;
        }
    }

    void PlayGameplaySound(int soundNumber, float volume = 1)
    {
        for (int j = 0; j < gameplaySource.Count; j++)
        {
            if (!gameplaySource[j].isPlaying)
            {
                gameplaySource[j].loop = false;
                gameplaySource[j].clip = (AudioClip) Resources.Load("Sounds/" + gameplaySounds[soundNumber]) ; gameplaySource[j].Play();
                gameplaySource[j].volume = volume;
                return;
            }
        }
    }

    void PlayGameplaySoundAtGivenSource(int soundNumber, AudioSource source, float volume = 1)
    {
        source.loop = false;
        source.clip = (AudioClip)Resources.Load("Sounds/" + gameplaySounds[soundNumber]);
        source.Play();
        source.volume = volume;
    }

    void PlayLoopingGameplaySound(int soundNumber, bool isLooping, float volume = 1)
    {
        for (int j = 0; j < gameplaySource.Count; j++)
        {
            if (!gameplaySource[j].isPlaying)
            {
                gameplaySource[j].clip = (AudioClip)Resources.Load("Sounds/" + gameplaySounds[soundNumber]); gameplaySource[j].Play(); gameplaySource[j].loop = isLooping;
                loopingGameplayBools[j] = isLooping;
                gameplaySource[j].volume = volume;
                return;
            }
        }
    }

    void PlayLoopingGameplaySoundAtGivenSource(int soundNumber, bool isLooping,  AudioSource audioSource, float volume = 1)
    {
        audioSource.clip = (AudioClip)Resources.Load("Sounds/" + gameplaySounds[soundNumber]);
        audioSource.Play();
        audioSource.loop = isLooping;
        audioSource.volume = volume;
    }

    public void StopSpecificGameplaySound(GamePlaySounds sound)
    {
        for (int i = 0; i < gameplaySounds.Count; i++)
        {
            if (gameplaySounds[i] == sound.ToString())
            {
                for (int j = 0; j < gameplaySource.Count; j++)
                {
                    if (gameplaySource[j].clip == (AudioClip)Resources.Load("Sounds/" + gameplaySounds[i]))
                    {
                        gameplaySource[j].Stop();
                        gameplaySource[j].loop = false;
                        if (loopingGameplayBools[i]) loopingGameplayBools[i] = false;
                    }
                }
            }
        }
    }

    public void StopGameplaySounds()
    {
        for (int i = 0; i < gameplaySource.Count; i++)
        {
            gameplaySource[i].Stop();
            gameplaySource[i].clip = null;
            gameplaySource[i].loop = false;
            gameplaySource[i].playOnAwake = false;
            if (loopingGameplayBools[i]) loopingGameplayBools[i] = false;
        }
    }

    public AudioClip RequestForGameplaySound(GamePlaySounds sound)
    {
        AudioClip toSend = null;
        for (int i = 0; i < gameplaySounds.Count; i++)
        {
            switch (gameplaySounds[i] == sound.ToString()){
                case true: toSend = (AudioClip)Resources.Load("Sounds/" + gameplaySounds[i]); break;
                case false: break;
            }
        }
        return toSend;
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////         UI SOUNDS         ///////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region UI Sounds
    public void PlayUISound(UISounds sound)
    {
        for (int i = 0; i < uISounds.Count; i++)
        {
            switch (uISounds[i] == sound.ToString()) {
                case true: PlayUISounds(i);break;
                case false: break;
            }
        }
    }

    public void PlayUISounds(int soundNumber)
    {
        for (int j = 0; j < uISource.Count; j++)
        {
            if (!uISource[j].isPlaying)
            {
                uISource[j].loop = false;
                uISource[j].clip = (AudioClip)Resources.Load("Sounds/" + uISounds[soundNumber]);
                uISource[j].Play();
                return;
            }
        }
    }

    public void StopUISounds()
    {
        for (int i = 0; i < uISource.Count; i++)
        {
            uISource[i].Stop();
            uISource[i].clip = null;
            uISource[i].loop = false;
            uISource[i].playOnAwake = false;
        }
    }

    public void StopASpecificUISound(UISounds sound)
    {
        for (int i = 0; i < uISounds.Count; i++)
        {
            if (uISounds[i] == sound.ToString())
            {
                for (int j = 0; j < uISource.Count; j++)
                {
                    if (uISource[j].clip == (AudioClip)Resources.Load("Sounds/" + uISounds[i]))
                    {
                        uISource[j].Stop();
                    }
                }
            }
        }
    }

    public AudioClip RequestForUISound(UISounds sound)
    {
        AudioClip toSend = null;
        for (int i = 0; i < gameplaySounds.Count; i++)
        {
            switch (uISounds[i] == sound.ToString())
            {
                case true: toSend = (AudioClip)Resources.Load("Sounds/" + uISounds[i]); break;
                case false: break;
            }
        }
        return toSend;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////         FUNCTIONALITY         ///////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Functionalities

    public void PauseGamePlaySounds()
    {
        foreach(AudioSource gameplaySound in gameplaySource)
        {
            switch (gameplaySound.isPlaying)
            {
                case true:
                    if (!pausedSounds.Contains(gameplaySound)) pausedSounds.Add(gameplaySound);
                    gameplaySound.Pause();
                    gameplaySound.enabled = false;
                    break;
                case false:
                    break;
            }
        }

        // Stopping all sounds on the player
        foreach (AudioSource gameplaySound in CommandCentre.Instance.PlayerControls_.Playerbody.GetComponentsInChildren<AudioSource>())
        {
            switch (gameplaySound.isPlaying)
            {
                case true:
                    if (!pausedSounds.Contains(gameplaySound)) pausedSounds.Add(gameplaySound);
                    gameplaySound.Pause();
                    gameplaySound.enabled = false;
                    break;
                case false:
                    break;
            }
        }

        // Control
        foreach(AudioSource pausedSound in pausedSounds)
        {
            switch (pausedSound.isPlaying)
            {
                case true:
                    pausedSound.enabled = false;
                    break;
                case false:
                    break;
            }
        }

    }

    public void UnpauseGameplaySounds()
    {
        foreach (AudioSource pausedSound in pausedSounds)
        {
            switch (pausedSound.enabled == false)
            {
                case true:
                    pausedSound.enabled = true;
                    pausedSound.Play();
                    if (pausedSounds.Contains(pausedSound)) pausedSounds.Remove(pausedSound);
                    break;
                case false:
                    break;
            }
        }
    }

    #endregion
}
