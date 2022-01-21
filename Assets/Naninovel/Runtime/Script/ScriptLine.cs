﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Represents a single line in a <see cref="Script"/>.
    /// </summary>
    [System.Serializable]
    public abstract class ScriptLine
    {
        /// <summary>
        /// Name of the naninovel script to which the line belongs.
        /// </summary>
        public string ScriptName => scriptName;
        /// <summary>
        /// Index of the line in naninovel script.
        /// </summary>
        public int LineIndex => lineIndex;
        /// <summary>
        /// Number of the line in naninovel script (index + 1).
        /// </summary>
        public int LineNumber => LineIndex + 1;
        /// <summary>
        /// Persistent hash code of the original text line (before any define replacements).
        /// </summary>
        public string LineHash => lineHash;

        [SerializeField] private string scriptName = default;
        [SerializeField] private int lineIndex = default;
        [SerializeField] private string lineHash = default;

        private readonly ICollection<ScriptParseError> errors;

        /// <summary>
        /// Creates new instance by parsing the provided serialized script line text.
        /// </summary>
        /// <param name="scriptName">Name of the script asset which contains the line.</param>
        /// <param name="lineIndex">Index of the line in naninovel script.</param>
        /// <param name="lineText">The script line text to parse.</param>
        /// <param name="errors">When provided and an error occurs while parsing the line, will add the error to the collection.</param>
        protected ScriptLine (string scriptName, int lineIndex, string lineText, ICollection<ScriptParseError> errors = null)
        {
            this.scriptName = scriptName;
            this.lineIndex = lineIndex;
            this.lineHash = CryptoUtils.PersistentHexCode(lineText.TrimFull());
            this.errors = errors;
        }

        protected void AddParseError (string description)
        {
            var errorData = new ScriptParseError(this, description);
            errors?.Add(errorData);
            Debug.LogError(errorData);
        }
    } 
}
