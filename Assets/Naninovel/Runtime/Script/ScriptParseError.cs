// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;

namespace Naninovel
{
    /// <summary>
    /// Represent a <see cref="ScriptLine"/> parse error.
    /// </summary>
    public readonly struct ScriptParseError : IEquatable<ScriptParseError>
    {
        /// <summary>
        /// Name of the naninovel script to which the line belongs.
        /// </summary>
        public readonly string ScriptName;
        /// <summary>
        /// Index of the line in naninovel script.
        /// </summary>
        public readonly int LineIndex;
        /// <summary>
        /// Number of the line in naninovel script (index + 1).
        /// </summary>
        public int LineNumber => LineIndex + 1;
        /// <summary>
        /// Description of the parse error.
        /// </summary>
        public readonly string ErrorDescription;

        public ScriptParseError (string scriptName, int lineIndex, string errorDescription)
        {
            ScriptName = scriptName;
            LineIndex = lineIndex;
            ErrorDescription = errorDescription;
        }

        public ScriptParseError (ScriptLine line, string errorDescription) 
            : this(line.ScriptName, line.LineIndex, errorDescription) { }

        public override string ToString () => $"Error parsing `{ScriptName}` script at line #{LineNumber}{(ErrorDescription == string.Empty ? "." : $": {ErrorDescription}")}";
        
        public bool Equals (ScriptParseError other) => ScriptName == other.ScriptName && LineIndex == other.LineIndex && ErrorDescription == other.ErrorDescription;
        public override bool Equals (object obj) => obj is ScriptParseError other && Equals(other);
        public static bool operator == (ScriptParseError left, ScriptParseError right) => left.Equals(right);
        public static bool operator != (ScriptParseError left, ScriptParseError right) => !left.Equals(right);
        public override int GetHashCode ()
        {
            unchecked
            {
                var hashCode = (ScriptName != null ? ScriptName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LineIndex;
                hashCode = (hashCode * 397) ^ (ErrorDescription != null ? ErrorDescription.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
