﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.


namespace Naninovel.UI
{
    public class GameSettingsBgmVolumeSlider : ScriptableSlider
    {
        private IAudioManager audioManager;

        protected override void Awake ()
        {
            base.Awake();

            audioManager = Engine.GetService<IAudioManager>();
        }

        protected override void Start ()
        {
            base.Start();

            UIComponent.value = audioManager.BgmVolume;
        }

        protected override void OnValueChanged (float value)
        {
            audioManager.BgmVolume = value;
        }
    }
}
