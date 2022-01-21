﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.


namespace Naninovel.UI
{
    public class GameSettingsSfxVolumeSlider : ScriptableSlider
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

            UIComponent.value = audioManager.SfxVolume;
        }

        protected override void OnValueChanged (float value)
        {
            audioManager.SfxVolume = value;
        }
    }
}
