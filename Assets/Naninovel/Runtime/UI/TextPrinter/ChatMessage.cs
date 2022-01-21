// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Naninovel.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ChatMessage : ScriptableUIBehaviour
    {
        [System.Serializable]
        public struct State
        {
            public string PrintedText;
            public string AuthorId;
        }

        [System.Serializable]
        private class MessageTextChangedEvent : UnityEvent<string> { }

        public virtual string MessageText { get => messageText; set { messageText = value; onMessageTextChanged?.Invoke(value); } }
        public virtual string AuthorId { get; set; }
        public virtual Color MessageColor { get => messageFrameImage.color; set => messageFrameImage.color = value; }
        public virtual string ActorNameText { get => actorNamePanel.Text; set => actorNamePanel.Text = value; }
        public virtual Color ActorNameTextColor { get => actorNamePanel.TextColor; set => actorNamePanel.TextColor = value; }
        public virtual Texture AvatarTexture { get => avatarImage.texture; set { avatarImage.texture = value; avatarImage.gameObject.SetActive(value); } }

        protected AuthorNamePanel ActorNamePanel => actorNamePanel;
        protected Image MessageFrameImage => messageFrameImage;
        protected RawImage AvatarImage => avatarImage;

        [SerializeField] private AuthorNamePanel actorNamePanel = default;
        [SerializeField] private Image messageFrameImage = default;
        [SerializeField] private RawImage avatarImage = default;
        [Tooltip("Invoked when the message text is changed.")]
        [SerializeField] private MessageTextChangedEvent onMessageTextChanged = default;

        private string messageText;

        public virtual State GetState () => new State { PrintedText = MessageText, AuthorId = AuthorId };

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(actorNamePanel, messageFrameImage, avatarImage);
        }
    }
}
