// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.


namespace Naninovel.UI
{
    public class ControlPanelQuickLoadButton : ScriptableButton
    {
        private IStateManager gameState;

        protected override void Awake ()
        {
            base.Awake();

            gameState = Engine.GetService<IStateManager>();
        }

        protected override void Start ()
        {
            base.Start();

            ControlInteractability();
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            gameState.GameStateSlotManager.OnBeforeLoad += ControlInteractability;
            gameState.GameStateSlotManager.OnLoaded += ControlInteractability;
            gameState.GameStateSlotManager.OnBeforeSave += ControlInteractability;
            gameState.GameStateSlotManager.OnSaved += ControlInteractability;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            gameState.GameStateSlotManager.OnBeforeLoad -= ControlInteractability;
            gameState.GameStateSlotManager.OnLoaded -= ControlInteractability;
            gameState.GameStateSlotManager.OnBeforeSave -= ControlInteractability;
            gameState.GameStateSlotManager.OnSaved -= ControlInteractability;
        }

        protected override void OnButtonClick ()
        {
            UIComponent.interactable = false;
            QuickLoadAsync();
        }

        private async void QuickLoadAsync ()
        {
            await gameState.QuickLoadAsync();
        }

        private void ControlInteractability ()
        {
            UIComponent.interactable = gameState.QuickLoadAvailable && !gameState.GameStateSlotManager.Loading && !gameState.GameStateSlotManager.Saving;
        }
    } 
}
