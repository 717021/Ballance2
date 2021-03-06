﻿using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 代码说明：游戏背景声音播放器
 * 
 * 
 公有操作：
 ********************************************
 * PlaySoundAction
 * StopSoundAction
 * 
*/

public class SoundsManager : MonoBehaviour
{
    private static Dictionary<string, AudioClip> playedCache = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> isPlayingVoice = new Dictionary<string, AudioSource>();
    private Dictionary<string, ManagedVoice> managedVoice = new Dictionary<string, ManagedVoice>();

    private AudioSource AudioPlayerBackGround;
    private AudioSource AudioPlayerBackGroundLoop;
    private AudioSource AudioPlayerVoiceLoop;
    private AudioSource AudioPlayerVoice;

    private struct ManagedVoice
    {
        public ManagedVoice(bool isBg, AudioSource source)
        {
            this.source = source;
            this.isBg = isBg;
        }

        public AudioSource source;
        public bool isBg;
    }

    ActionHandler setActionHandler,
        playActionHandler,
        stopActionHandler,
        addActionHandler,
        removeActionHandler;

    private void Start()
    {
        AudioPlayerBackGround = gameObject.transform.GetChild(1).gameObject.GetComponent<AudioSource>();
        AudioPlayerBackGroundLoop = gameObject.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        AudioPlayerVoiceLoop = gameObject.transform.GetChild(2).gameObject.GetComponent<AudioSource>();
        AudioPlayerVoice = gameObject.transform.GetChild(3).gameObject.GetComponent<AudioSource>();

        float mv = GlobalSettings.MusicVolume, sv = GlobalSettings.SoundVolume;
        AudioPlayerBackGround.volume = mv;
        AudioPlayerBackGroundLoop.volume = mv;
        AudioPlayerVoiceLoop.volume = sv;
        AudioPlayerVoice.volume = sv;

        GlobalMediator.RegisterEventLinster(new OnGameExitLinister(GameExitingActionHandler));

        setActionHandler = GlobalMediator.RegisterAction(new SetSoundMgrSettingsAction(), "SetSoundMgrSettings");
        setActionHandler.AddHandler("SoundsManager", SetActionHandler);
        playActionHandler = GlobalMediator.RegisterAction(new PlaySoundAction(), "PlayItem");
        playActionHandler.AddHandler("SoundsManager", PlayItemActionHandler);
        stopActionHandler = GlobalMediator.RegisterAction(new StopSoundAction(), "StopItem");
        stopActionHandler.AddHandler("SoundsManager", StopItemActionHandler);

        addActionHandler = GlobalMediator.RegisterAction(new AddManagedSoundAction(), "AddManagedSound");
        addActionHandler.AddHandler("SoundsManager", AddManagedVoiceActionHandler);
        removeActionHandler = GlobalMediator.RegisterAction(new RemoveManagedSoundAction(), "RemoveManagedSound");
        removeActionHandler.AddHandler("SoundsManager", RemoveManagedVoiceActionHandler);
    }

    public AudioClip LoadAudioItemInPack(string pkg,string it)
    {
        return GlobalAssetPool.GetResource(pkg, it) as AudioClip;
    }

    private void GameExitingActionHandler()
    {
        playedCache.Clear();
        foreach (AudioSource a in isPlayingVoice.Values)
        {
            if (a.isPlaying)
                a.Stop();
            a.clip = null;
        }
        isPlayingVoice.Clear();
        managedVoice.Clear();
    }
    private void ChangeManagedSounds(bool bg, float v)
    {
        if(bg)
        {
            foreach(ManagedVoice m in managedVoice.Values)
            {
                if (m.isBg)
                {
                    m.source.volume = v;
                }
            }
        }
        else
        {
            foreach (ManagedVoice m in managedVoice.Values)
            {
                if (!m.isBg)
                {
                    m.source.volume = v;
                }
            }
        }
    }

    public bool AddManagedVoiceActionHandler(params object[] datas)
    {
        if (datas.Length >= 2)
        {
            string name = (string)datas[0];
            if (managedVoice.ContainsKey(name))
            {
                managedVoice.Remove(name);
                managedVoice.Add(name, new ManagedVoice((bool)datas[1], datas[2] as AudioSource));
            }
            else managedVoice.Add(name, new ManagedVoice((bool)datas[1], datas[2] as AudioSource));
        }
        return true;
    }
    public bool RemoveManagedVoiceActionHandler(params object[] datas)
    {
        if (datas.Length >= 1)
        {
            string name = (string)datas[0];
            return managedVoice.Remove(name);
        }
        return true;
    }
    public bool SetActionHandler(params object[] datas)
    {
        if (datas.Length >= 1)
        {
            float mv = (float)datas[0];
            if (mv >= 0 && mv <= 1)
            {
                AudioPlayerBackGround.volume = mv;
                AudioPlayerBackGroundLoop.volume = mv;
                ChangeManagedSounds(true, mv);
            }
        }
        else if (datas.Length >= 2)
        {
            float sv = (float)datas[1];
            if (sv >= 0 && sv <= 1)
            {
                AudioPlayerVoiceLoop.volume = sv;
                AudioPlayerVoice.volume = sv;
                ChangeManagedSounds(false, sv);
            }
        }
        return true;
    }
    public bool PlayItemActionHandler(params object[] datas)
    {
        string name = datas[0] as string + datas[1] as string;
        AudioClip a = null;
        if (!playedCache.TryGetValue(name, out a))
        {
            a = LoadAudioItemInPack(datas[0] as string, datas[1] as string);
            playedCache.Add(name, a);
        }
        if (a != null)
        {
            if ((int)datas[2] == 1)
            {
                if (!isPlayingVoice.ContainsKey(name))
                    isPlayingVoice.Add(name, AudioPlayerBackGround);
                AudioPlayerBackGround.clip = a;
                AudioPlayerBackGround.Play();
            }
            else if ((int)datas[2] == 2)
            {
                if (!isPlayingVoice.ContainsKey(name))
                    isPlayingVoice.Add(name, AudioPlayerVoice);
                AudioPlayerVoice.clip = a;
                AudioPlayerVoice.Play();
            }
            else if ((int)datas[2] == 3)
            {
                if (!isPlayingVoice.ContainsKey(name))
                    isPlayingVoice.Add(name, AudioPlayerBackGroundLoop);
                AudioPlayerBackGroundLoop.clip = a;
                AudioPlayerBackGroundLoop.Play();
            }
            else if ((int)datas[2] == 4)
            {
                if (!isPlayingVoice.ContainsKey(name))
                    isPlayingVoice.Add(name, AudioPlayerVoiceLoop);
                AudioPlayerVoiceLoop.clip = a;
                AudioPlayerVoiceLoop.Play();
            }
            else if ((int)datas[2] == 5)
            {
                if (!isPlayingVoice.ContainsKey(name))
                    isPlayingVoice.Add(datas[0] as string + datas[1] as string, datas[3] as AudioSource);
                (datas[3] as AudioSource).clip = a;
                (datas[3] as AudioSource).Play();
            }
        }
        else throw new System.Exception("Cant not load audio resource: \nPacl: " + datas[0] as string + " \nName: " + datas[1] as string);
        return true;
    }
    public bool StopItemActionHandler(params object[] datas)
    {
        string name = datas[0] as string + datas[1] as string;
        AudioSource a;
        if(isPlayingVoice.TryGetValue(name,out a))
        {
            if (a.isPlaying)
                a.Stop();
            a.clip = null;
            isPlayingVoice.Remove(name);
        }
        return true;
    }
}