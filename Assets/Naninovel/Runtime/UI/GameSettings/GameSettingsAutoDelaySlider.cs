// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;

namespace Naninovel.UI
{
    public class GameSettingsAutoDelaySlider : ScriptableSlider
    {
        protected virtual bool PreviewPrinterAvailable => ObjectUtils.IsValid(previewPrinter) && previewPrinter.isActiveAndEnabled;
        protected GameSettingsPreviewPrinter PreviewPrinter => previewPrinter;

        [SerializeField] private GameSettingsPreviewPrinter previewPrinter = default;

        private ITextPrinterManager printerManager;

        protected override void Awake ()
        {
            base.Awake();

            printerManager = Engine.GetService<ITextPrinterManager>();
            UIComponent.value = printerManager.BaseAutoDelay;
        }

        protected override void OnValueChanged (float value)
        {
            printerManager.BaseAutoDelay = value;

            if (PreviewPrinterAvailable)
                previewPrinter.StartPrinting();
        }
    }
}
