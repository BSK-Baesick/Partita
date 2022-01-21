﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Naninovel.UI
{
    public class RevealableTMProText : TextMeshProUGUI, IRevealableText, IPointerClickHandler, IInputTrigger
    {
        [System.Serializable]
        private class TipClickedEvent : UnityEvent<string> { }

        public virtual string Text { get => assignedText; set => SetTextToReveal(value); }
        public virtual Color TextColor { get => color; set => color = value; }
        public virtual GameObject GameObject => gameObject;
        public virtual float RevealProgress { get => GetRevealProgress(); set => SetRevealProgress(value); }
        public virtual bool Revealing => revealState.InProgress;

        protected virtual int LastRevealedCharIndex { get; private set; }
        protected virtual int LastCharIndex => textInfo.characterCount - 1;
        protected virtual Transform CanvasTransform => canvasTransformCache != null ? canvasTransformCache : (canvasTransformCache = canvas.GetComponent<Transform>());
        protected virtual Canvas TopmostCanvas => ObjectUtils.IsValid(topmostCanvasCache) ? topmostCanvasCache : (topmostCanvasCache = gameObject.FindTopmostComponent<Canvas>());
        protected virtual float SlideProgress => slideClipRect && lastRevealDuration > 0 ? Mathf.Clamp01((Time.time - lastRevealTime) / lastRevealDuration) : 1f;

        protected virtual float RevealFadeWidth => revealFadeWidth;
        protected virtual bool SlideClipRect => slideClipRect;
        protected virtual float ItalicSlantAngle => italicSlantAngle;
        protected virtual string RubyVerticalOffset => rubyVerticalOffset;
        protected virtual float RubySizeScale => rubySizeScale;
        protected virtual bool FixArabicText => fixArabicText;

        [Tooltip("Width (in pixels) of the gradient fade near the reveal border.")]
        [SerializeField] private float revealFadeWidth = 100f;
        [Tooltip("Whether to smoothly reveal the text. Disable for the `typewriter` effect.")]
        [SerializeField] private bool slideClipRect = true;
        [Tooltip("How much to slant the reveal rect when passing over italic characters.")]
        [SerializeField] private float italicSlantAngle = 10f;
        [Tooltip("Vertical line offset to use for the ruby (furigana) text; supported units: em, px, %.")]
        [SerializeField] private string rubyVerticalOffset = "1em";
        [Tooltip("Font size scale (relative to the main text font size) to apply for the ruby (furigana) text.")]
        [SerializeField] private float rubySizeScale = .5f;
        [Tooltip("Whether to automatically unlock associated tip records when text wrapped in <tip> tags is printed.")]
        [SerializeField] private bool unlockTipsOnPrint = true;
        [Tooltip("Template to use when processing text wrapped in <tip> tags. " + tipTemplateLiteral + " will be replaced with the actual tip content.")]
        [SerializeField] private string tipTemplate = $"<u>{tipTemplateLiteral}</u>";
        [Tooltip("Invoked when a text wrapped in <tip> tags is clicked; returned string argument is the ID of the clicked tip. Be aware, that the default behaviour (showing `ITipsUI` when a tip is clicked) won't be invoked when a custom handler is assigned.")]
        [SerializeField] private TipClickedEvent onTipClicked = default;
        [Tooltip("Whether to modify the text to support arabic languages (fix letters connectivity issues).")]
        [SerializeField] private bool fixArabicText = false;
        [Tooltip("When `Fix Arabic Text` is enabled, controls to whether also fix Farsi characters.")]
        [SerializeField] private bool fixArabicFarsi = true;
        [Tooltip("When `Fix Arabic Text` is enabled, controls to whether also fix rich text tags.")]
        [SerializeField] private bool fixArabicTextTags = true;
        [Tooltip("When `Fix Arabic Text` is enabled, controls to whether preserve numbers.")]
        [SerializeField] private bool fixArabicPreserveNumbers = false;

        private const string tipIdPrefix = "NANINOVEL.TIP.";
        private const string tipTemplateLiteral = "%TIP%";
        private static readonly int lineClipRectPropertyId = Shader.PropertyToID("_LineClipRect");
        private static readonly int charClipRectPropertyId = Shader.PropertyToID("_CharClipRect");
        private static readonly int charFadeWidthPropertyId = Shader.PropertyToID("_CharFadeWidth");
        private static readonly int charSlantAnglePropertyId = Shader.PropertyToID("_CharSlantAngle");
        private static readonly TMP_CharacterInfo invalidChar = new TMP_CharacterInfo { lineNumber = -1, index = -1 };
        private static readonly Regex captureRubyRegex = new Regex(@"<ruby=""([\s\S]*?)"">([\s\S]*?)<\/ruby>");
        private static readonly Regex captureTipRegex = new Regex(@"<tip=""([\w]*?)"">([\s\S]*?)<\/tip>");

        private readonly TextRevealState revealState = new TextRevealState();
        private readonly ArabicSupport.FastStringBuilder arabicBuilder = new ArabicSupport.FastStringBuilder(ArabicSupport.RTLSupport.DefaultBufferSize);
        private string assignedText;
        private Transform canvasTransformCache;
        private Material[] cachedFontMaterials;
        private Vector3[] worldCorners = new Vector3[4];
        private Vector3[] canvasCorners = new Vector3[4];
        private Vector4 curLineClipRect, curCharClipRect;
        private float curCharFadeWidth, curCharSlantAngle;
        private TMP_CharacterInfo revealStartChar = invalidChar;
        private float lastRevealDuration, lastRevealTime, lastCharClipPos, lastCharFadeWidth;
        private Vector3 positionLastFrame;
        private Canvas topmostCanvasCache;

        public virtual void RevealNextChars (int count, float duration, CancellationToken cancellationToken)
        {
            revealState.Start(count, duration, cancellationToken);
        }

        public virtual Vector2 GetLastRevealedCharPosition ()
        {
            if (LastRevealedCharIndex < 0) return default;

            UpdateClipRects();

            var currentChar = textInfo.characterInfo[LastRevealedCharIndex];
            var currentLine = textInfo.lineInfo[currentChar.lineNumber];
            var localPos = new Vector2(isRightToLeftText ? curCharClipRect.z : curCharClipRect.x, curCharClipRect.w - currentLine.lineHeight);
            return CanvasTransform.TransformPoint(localPos);
        }

        public virtual char GetLastRevealedChar ()
        {
            if (Text is null || LastRevealedCharIndex < 0 || LastRevealedCharIndex >= Text.Length)
                return default;
            return Text[LastRevealedCharIndex];
        }

        public void OnPointerClick (PointerEventData eventData)
        {
            var renderCamera = TopmostCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : TopmostCanvas.worldCamera;
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(this, eventData.position, renderCamera);
            if (linkIndex == -1) return;

            var linkInfo = textInfo.linkInfo[linkIndex];
            var linkId = linkInfo.GetLinkID();
            if (!linkId.StartsWithFast(tipIdPrefix)) return;

            var tipId = linkId.GetAfter(tipIdPrefix);
            if (onTipClicked?.GetPersistentEventCount() > 0)
            {
                onTipClicked.Invoke(tipId);
                return;
            }

            var tipsUI = Engine.GetService<IUIManager>()?.GetUI<ITipsUI>();
            tipsUI?.Show();
            tipsUI?.SelectTipRecord(tipId);
        }

        public bool CanTriggerInput ()
        {
            var evtSystem = EventSystem.current;
            if (!evtSystem) return true;
            var inputModule = evtSystem.currentInputModule;
            if (!inputModule) return true;
            var input = inputModule.input;
            if (!input) return true;

            var position = input.mousePosition;
            var renderCamera = TopmostCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : TopmostCanvas.worldCamera;
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(this, position, renderCamera);
            return linkIndex == -1;
        }

        protected override void OnRectTransformDimensionsChange ()
        {
            base.OnRectTransformDimensionsChange();

            if (!Application.isPlaying || ObjectUtils.IsEditedInPrefabMode(gameObject)) return; // TextMeshProUGUI has [ExecuteInEditMode]

            // When text layout changes (eg, content size fitter decides to increase height),
            // we need to force-update clip rect; otherwise, the update will be delayed by one frame
            // and user will see incorrectly revealed text for a moment.
            UpdateClipRects();
            Update();
        }

        protected override void Start ()
        {
            base.Start();

            positionLastFrame = transform.position;
        }

        /// <summary>
        /// Given the input text, extracts text wrapped in ruby tags and replace it with expression natively supported by TMPro.
        /// </summary>
        protected virtual string ProcessRubyTags (string content)
        {
            // Replace <ruby> tags with TMPro-supported rich text tags
            // to simulate ruby (furigana) text layout.
            var matches = captureRubyRegex.Matches(content);
            foreach (Match match in matches)
            {
                if (match.Groups.Count != 3) continue;
                var fullMatch = match.Groups[0].ToString();
                var rubyValue = match.Groups[1].ToString();
                var baseText = match.Groups[2].ToString();

                var baseTextWidth = GetPreferredValues(baseText).x;
                var rubyTextWidth = GetPreferredValues(rubyValue).x * rubySizeScale;
                var rubyTextOffset = baseTextWidth / 2f + rubyTextWidth / 2f;
                var compensationOffset = (baseTextWidth - rubyTextWidth) / 2f;
                var replace = $"<space={compensationOffset}><voffset={rubyVerticalOffset}><size={rubySizeScale * 100f}%>{rubyValue}</size></voffset><space=-{rubyTextOffset}>{baseText}";
                content = content.Replace(fullMatch, replace);
            }

            return content;
        }

        /// <summary>
        /// Given the input text, extracts text wrapped in tip tags and replace it with expression natively supported by TMPro.
        /// </summary>
        protected virtual string ProcessTipTags (string content)
        {
            var matches = captureTipRegex.Matches(content);
            foreach (Match match in matches)
            {
                if (match.Groups.Count != 3) continue;
                var fullMatch = match.Groups[0].ToString();
                var tipID = match.Groups[1].ToString();
                var tipContent = match.Groups[2].ToString();

                if (unlockTipsOnPrint)
                    Engine.GetService<IUnlockableManager>()?.UnlockItem($"Tips/{tipID}");

                var replace = $"<link={tipIdPrefix + tipID}>{tipTemplate.Replace(tipTemplateLiteral, tipContent)}</link>";
                content = content.Replace(fullMatch, replace);
            }

            return content;
        }

        private void Update ()
        {
            if (!Application.isPlaying || m_textInfo is null || ObjectUtils.IsEditedInPrefabMode(gameObject)) return; // TextMeshProUGUI has [ExecuteInEditMode]

            UpdateRevealState();

            if (slideClipRect)
            {

                var charClipPos = Mathf.Lerp(lastCharClipPos, isRightToLeftText ? curCharClipRect.z : curCharClipRect.x, SlideProgress);
                var slidedCharClipRect = isRightToLeftText ? new Vector4(curCharClipRect.x, curCharClipRect.y, charClipPos, curCharClipRect.w) :
                                                             new Vector4(charClipPos, curCharClipRect.y, curCharClipRect.z, curCharClipRect.w);
                var slidedFadeWidth = Mathf.Lerp(lastCharFadeWidth, curCharFadeWidth, SlideProgress);
                SetMaterialProperties(curLineClipRect, slidedCharClipRect, slidedFadeWidth, curCharSlantAngle);
            }
            else SetMaterialProperties(curLineClipRect, curCharClipRect, curCharFadeWidth, curCharSlantAngle);

            //Debug.DrawLine(CanvasTransform.TransformPoint(new Vector3(curLineClipRect.x, curLineClipRect.y)), CanvasTransform.TransformPoint(new Vector3(curLineClipRect.z, curLineClipRect.w)), Color.blue);
            //Debug.DrawLine(CanvasTransform.TransformPoint(new Vector3(curCharClipRect.x, curCharClipRect.y)), CanvasTransform.TransformPoint(new Vector3(curCharClipRect.z, curCharClipRect.w)), Color.red);
        }

        private void LateUpdate ()
        {
            if (transform.position != positionLastFrame)
            {
                UpdateClipRects();
                Update();
            }

            positionLastFrame = transform.position;
        }

        private void UpdateRevealState ()
        {
            if (!revealState.InProgress) return;

            if (LastRevealedCharIndex >= LastCharIndex)
            {
                revealState.Reset();
                return;
            }

            // Wait while the clip rects are slided over currently revealed character.
            if (slideClipRect && SlideProgress < 1 && !revealState.CancellationToken.CancelASAP) return;
            if (revealState.CancellationToken.CancelASAP) { revealState.Reset(); return; }

            if (revealState.CharactersRevealed == revealState.CharactersToReveal)
            {
                revealState.Reset();
                return;
            }

            lastRevealDuration = Mathf.Max(revealState.RevealDuration, 0);
            lastRevealTime = Time.time;

            SetLastRevealedCharIndex(LastRevealedCharIndex + 1);

            revealState.CharactersRevealed++;
        }

        private void RevealAll ()
        {
            SetLastRevealedCharIndex(LastCharIndex);
            lastRevealDuration = 0f; // Force the slide to complete instantly.
            revealState.Reset();
        }

        private void HideAll ()
        {
            SetLastRevealedCharIndex(-1);
            lastRevealDuration = 0f; // Force the slide to complete instantly.
            revealStartChar = invalidChar; // Invalidate the reveal start position.
            Update(); // Otherwise the unrevealed yet text could be visible for a moment.
            revealState.Reset();
        }

        private void SetMaterialProperties (Vector4 lineClipRect, Vector4 charClipRect, float charFadeWidth, float charSlantAngle)
        {
            if (cachedFontMaterials is null || cachedFontMaterials.Length != textInfo.materialCount)
                cachedFontMaterials = fontMaterials; // Material count change when using fallback fonts or font variants (weights).

            for (int i = 0; i < cachedFontMaterials.Length; i++)
            {
                cachedFontMaterials[i].SetVector(lineClipRectPropertyId, lineClipRect);
                cachedFontMaterials[i].SetVector(charClipRectPropertyId, charClipRect);
                cachedFontMaterials[i].SetFloat(charFadeWidthPropertyId, charFadeWidth);
                cachedFontMaterials[i].SetFloat(charSlantAnglePropertyId, charSlantAngle);
            }
        }

        private void SetTextToReveal (string value)
        {
            assignedText = value;

            // Pre-process the assigned text handling <ruby> and <tip> tags.
            text = ProcessTipTags(ProcessRubyTags(value));

            if (FixArabicText && !string.IsNullOrWhiteSpace(text))
            {
                arabicBuilder.Clear();
                ArabicSupport.RTLSupport.FixRTL(text, arabicBuilder, fixArabicFarsi, fixArabicTextTags, fixArabicPreserveNumbers);
                arabicBuilder.Reverse();
                text = arabicBuilder.ToString();
            }

            // If visible text content changed...
            if (m_isLayoutDirty)
            {
                // Recalculate all the TMPro properties before rendering next frame, 
                // as the reveal clip rects rely on them. 
                ForceMeshUpdate();
                // Set current last revealed char as the start position for the reveal effect to 
                // prevent it from affecting this char again when resuming the revealing without resetting the text.
                revealStartChar = (LastRevealedCharIndex < 0 || LastRevealedCharIndex >= textInfo.characterInfo.Length) ? invalidChar : textInfo.characterInfo[LastRevealedCharIndex];
            }
        }

        private void SetLastRevealedCharIndex (int charIndex)
        {
            if (LastRevealedCharIndex == charIndex) return;

            var curChar = textInfo.characterInfo.IsIndexValid(LastRevealedCharIndex) ? textInfo.characterInfo[LastRevealedCharIndex] : invalidChar;
            var nextChar = textInfo.characterInfo.IsIndexValid(charIndex) ? textInfo.characterInfo[charIndex] : invalidChar;

            // Skip chars when (while at the same line) the caret is moving back (eg, when using ruby text).
            if (!isRightToLeftText && charIndex > 0 && nextChar.lineNumber == curChar.lineNumber && charIndex > LastRevealedCharIndex)
            {
                while (nextChar.lineNumber == curChar.lineNumber && nextChar.origin < curChar.xAdvance && charIndex < LastCharIndex)
                {
                    charIndex++;
                    nextChar = textInfo.characterInfo[charIndex];
                }

                // Last char is still behind the previous one; use pos. of the previous.
                if (nextChar.origin < curChar.xAdvance)
                    nextChar = curChar;
            }

            lastCharClipPos = isRightToLeftText ? (curChar.lineNumber < 0 ? curLineClipRect.z : curCharClipRect.z) :
                                                  (curChar.lineNumber < 0 ? curLineClipRect.x : curCharClipRect.x);
            lastCharFadeWidth = curCharFadeWidth;

            LastRevealedCharIndex = charIndex;
            UpdateClipRects();

            // Reset the slide when switching lines.
            if (slideClipRect && curChar.lineNumber != nextChar.lineNumber)
            {
                lastCharClipPos = isRightToLeftText ? GetTextCornersInCanvasSpace().z : GetTextCornersInCanvasSpace().x;
                lastCharFadeWidth = curCharFadeWidth;
            }
        }

        private float GetRevealProgress ()
        {
            if (LastCharIndex <= 0) return LastRevealedCharIndex >= 0 ? 1f : 0f;
            if (LastRevealedCharIndex == LastCharIndex) return 1f;
            return Mathf.Clamp01(LastRevealedCharIndex / (float)LastCharIndex);
        }

        private void SetRevealProgress (float revealProgress)
        {
            if (revealProgress >= 1) { RevealAll(); return; }
            else if (revealProgress <= 0) { HideAll(); return; }

            var charIndex = Mathf.CeilToInt(LastCharIndex * revealProgress);
            SetLastRevealedCharIndex(charIndex);
        }

        private void UpdateClipRects ()
        {
            if (textInfo == null || LastRevealedCharIndex >= textInfo.characterInfo.Length) return;

            var fullClipRect = GetTextCornersInCanvasSpace();

            if (LastRevealedCharIndex < 0) // Hide all.
            {
                curLineClipRect = fullClipRect;
                curCharClipRect = fullClipRect;
                return;
            }

            var currentChar = textInfo.characterInfo[LastRevealedCharIndex];
            var currentLine = textInfo.lineInfo[currentChar.lineNumber];
            var lineFirstChar = textInfo.characterInfo[currentLine.firstCharacterIndex];
            var lineLastChar = textInfo.characterInfo[currentLine.lastCharacterIndex];

            var clipPosY = currentLine.ascender + (rectTransform.pivot.y - 1f) * m_marginHeight;
            var clipPosX = currentChar.xAdvance + rectTransform.pivot.x * m_marginWidth;

            curLineClipRect = fullClipRect + new Vector4(0, 0, 0, clipPosY - currentLine.lineHeight);
            curCharClipRect = isRightToLeftText ? fullClipRect + new Vector4(0, 0, clipPosX - lineFirstChar.vertex_BR.position.x, clipPosY) :
                                                  fullClipRect + new Vector4(clipPosX, 0, 0, clipPosY);
            curCharClipRect.y = curLineClipRect.w;

            // We need to limit the fade width, so that it doesn't stretch before the first (startLimit) and last (endLimit) chars in the line.
            // Additionally, we need to handle cases when appending text, so that last revealed char won't get hidden again when resuming (revealStartChar is used instead of lineFirstChar).
            var startLimit = isRightToLeftText ? (currentChar.lineNumber == revealStartChar.lineNumber ? revealStartChar.origin - currentChar.origin : lineFirstChar.origin - currentChar.xAdvance) :
                                                 (currentChar.lineNumber == revealStartChar.lineNumber ? currentChar.origin - revealStartChar.origin : currentChar.xAdvance - lineFirstChar.origin);
            var endLimit = isRightToLeftText ? currentChar.xAdvance - lineLastChar.xAdvance : 
                                               lineLastChar.xAdvance - currentChar.xAdvance;
            var widthLimit = Mathf.Max(0, Mathf.Min(startLimit, endLimit));
            curCharFadeWidth = Mathf.Clamp(revealFadeWidth, 0f, widthLimit);

            curCharSlantAngle = currentChar.style == FontStyles.Italic ? italicSlantAngle : 0f;
        }

        private Vector4 GetTextCornersInCanvasSpace ()
        {
            rectTransform.GetWorldCorners(worldCorners);
            for (int i = 0; i < 4; ++i)
                canvasCorners[i] = CanvasTransform.InverseTransformPoint(worldCorners[i]);

            // Positions of diagonal corners.
            return new Vector4(canvasCorners[0].x, canvasCorners[0].y, canvasCorners[2].x, canvasCorners[2].y);
        }
    }
}
