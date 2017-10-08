// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Sprache;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer
    {
        /// <summary>
        /// https://github.com/sprache/Sprache/blob/master/test/Sprache.Tests/Scenarios/CsvTests.cs
        /// </summary>
        private static class ParserBuilder
        {
            public static Parser<IEnumerable<string>> Build(Options options)
            {
                Parser<char> cellSeparator = Parse.Char(options.DelimiterCharacter);

                Parser<char> quotedCellDelimiter = Parse.Char(options.QuoteCharacter);

                Parser<char> quoteEscape = Parse.Char(options.EscapeCharacter);

                Parser<char> quotedCellContent = Parse.AnyChar.Except(quotedCellDelimiter).Or(Escaped(quotedCellDelimiter, quoteEscape));

                Parser<char> literalCellContent = Parse.AnyChar.Except(cellSeparator).Except(Parse.String(Environment.NewLine));

                Parser<string> quotedCell = from open in quotedCellDelimiter
                    from content in quotedCellContent.Many().Text()
                    from end in quotedCellDelimiter
                    select content;

                Parser<string> newLine = Parse.String(Environment.NewLine).Text();

                Parser<string> recordTerminator = Parse.Return("").End().XOr(
                    newLine.End()).Or(
                    newLine);

                Parser<string> cell = quotedCell.XOr(literalCellContent.XMany().Text());

                return
                    from leading in cell
                    from rest in cellSeparator.Then(_ => cell).Many()
                    from terminator in recordTerminator
                    select Cons(leading, rest);
            }

            private static Parser<T> Escaped<T>(Parser<T> following, Parser<char> quoteEscape)
            {
                return from escape in quoteEscape
                    from f in following
                    select f;
            }

            private static IEnumerable<T> Cons<T>(T head, IEnumerable<T> rest)
            {
                yield return head;
                foreach (var item in rest)
                    yield return item;
            }
        }

        private readonly Options options;

        private readonly Parser<IEnumerable<string>> parser;
        
        public RFC4180Tokenizer(Options options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
            this.parser = ParserBuilder.Build(options);
        }
        
        public string[] Tokenize(string input)
        {
            if (options.TrimFields)
            {
                return parser.Parse(input.Trim()).ToArray();
            }
            return parser.Parse(input).ToArray();
        }

        public override string ToString()
        {
            return string.Format("RFC4180Tokenizer (Options = {0})", options);
        }
    }
}