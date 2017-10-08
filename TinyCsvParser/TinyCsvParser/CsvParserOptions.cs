// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser
{
    public class CsvParserOptions
    {
        public readonly ITokenizer Tokenizer;

        public readonly bool TrimFields;

        public readonly bool SkipHeader;

        public readonly string CommentCharacter;

        public readonly int DegreeOfParallelism;

        public readonly bool KeepOrder;

        public CsvParserOptions(bool skipHeader, char fieldsSeparator)
            : this(skipHeader, new QuotedStringTokenizer(fieldsSeparator))
        {
        }

        public CsvParserOptions(bool skipHeader, char fieldsSeparator, int degreeOfParallelism, bool keepOrder)
            : this(skipHeader, string.Empty, new QuotedStringTokenizer(fieldsSeparator), degreeOfParallelism, keepOrder, false)
        {
        }

        public CsvParserOptions(bool skipHeader, ITokenizer tokenizer)
            : this(skipHeader, string.Empty, tokenizer)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer)
            : this(skipHeader, commentCharacter, tokenizer, Environment.ProcessorCount, true, false)
        {
        }

        public CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer, int degreeOfParallelism, bool keepOrder, bool trimFields)
        {
            SkipHeader = skipHeader;
            CommentCharacter = commentCharacter;
            Tokenizer = tokenizer;
            DegreeOfParallelism = degreeOfParallelism;
            KeepOrder = keepOrder;
            TrimFields = trimFields;
        }

        public static CsvParserOptionsBuilder Builder { get { return new CsvParserOptionsBuilder(); } }

        public class CsvParserOptionsBuilder
        {
            private char fieldsSeparator;
            private char quoteCharacter;
            private char quoteEscape;
            private ITokenizer tokenizer;
            private bool trimFields;
            private bool skipHeader;
            private string commentCharacter;
            private int degreeOfParallelism;
            private bool keepOrder;

            public CsvParserOptionsBuilder()
            {
                fieldsSeparator = ',';
                quoteCharacter = '"';
                quoteEscape = '"';
                skipHeader = true;
                trimFields = false;
                degreeOfParallelism = Environment.ProcessorCount;
                keepOrder = false;
                commentCharacter = string.Empty;
                tokenizer = null;
            }

            public CsvParserOptionsBuilder FieldsSeparator(char fieldsSeparator)
            {
                this.fieldsSeparator = fieldsSeparator;

                return this;
            }

            public CsvParserOptionsBuilder QuoteCharacter(char quoteCharacter)
            {
                this.quoteCharacter = quoteCharacter;

                return this;
            }

            public CsvParserOptionsBuilder QuoteEscape(char quoteEscape)
            {
                this.quoteEscape = quoteEscape;

                return this;
            }

            public CsvParserOptionsBuilder SkipHeader(bool skipHeader)
            {
                this.skipHeader = skipHeader;

                return this;
            }

            public CsvParserOptionsBuilder KeepOrder(bool keepOrder)
            {
                this.keepOrder = keepOrder;

                return this;
            }

            public CsvParserOptionsBuilder CommentCharacter(string commentCharacter)
            {
                this.commentCharacter = commentCharacter;

                return this;
            }

            public CsvParserOptionsBuilder Tokenizer(ITokenizer tokenizer)
            {
                this.tokenizer = tokenizer;

                return this;
            }

            public CsvParserOptionsBuilder TrimField(bool trimFields)
            {
                this.trimFields = trimFields;

                return this;
            }

            public CsvParserOptionsBuilder DegreeOfParallelism(int degreeOfParallelism)
            {
                this.degreeOfParallelism = degreeOfParallelism;

                return this;
            }

            public CsvParserOptions Build()
            {
                if (tokenizer == null)
                {
                    tokenizer = new RFC4180Tokenizer(new Options(quoteCharacter, quoteEscape, fieldsSeparator));
                }
                return new CsvParserOptions(skipHeader, commentCharacter, tokenizer, degreeOfParallelism, keepOrder, trimFields);
            }
        }

        public override string ToString()
        {
            return string.Format("CsvParserOptions (Tokenizer = {0}, SkipHeader = {1}, DegreeOfParallelism = {2}, KeepOrder = {3}, CommentCharacter = {4}, TrimFields = {5})",
                Tokenizer, SkipHeader, DegreeOfParallelism, KeepOrder, CommentCharacter, TrimFields);
        }
    }
}
