// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Options
    {
        public readonly char QuoteCharacter;
        public readonly char EscapeCharacter;
        public readonly char DelimiterCharacter;
        public readonly bool TrimFields;

        public Options(char quoteCharacter, char escapeCharacter, char delimiterCharacter)
            : this(quoteCharacter, escapeCharacter, delimiterCharacter, false)
        {
        }

        public Options(char quoteCharacter, char escapeCharacter, char delimiterCharacter, bool trimFields)
        {
            QuoteCharacter = quoteCharacter;
            EscapeCharacter = escapeCharacter;
            DelimiterCharacter = delimiterCharacter;
            TrimFields = trimFields;
        }

        public override string ToString()
        {
            return string.Format("Options (QuoteCharacter = {0}, EscapeCharacter = {1}, DelimiterCharacter = {2}, TrimFields = {3})",
                QuoteCharacter, EscapeCharacter, DelimiterCharacter, TrimFields);
        }
    }
}
