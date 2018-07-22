// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Parsers;

namespace TinyCsvParser
{
    public enum QuoteStyleEnum
    {
        QUOTE_ALL,
        QUOTE_NONE
    }

    public class Dialect
    {
        public string Name { get; set; }

        public char Delimiter { get; set; }

        public char QuoteChar { get; set; }

        public char EscapeChar { get; set; }

        public bool DoubleQuote { get; set; }

        public bool SkipInitialSpace { get; set; }

        public QuoteStyleEnum Quoting { get; set; }

        public bool Strict { get; set; }
    }

    public class CsvParserOptions
    {
        public readonly Dialect Dialect;

        public bool SkipHeader { get; set; }

        public readonly int DegreeOfParallelism;

        public readonly bool KeepOrder;
        
        public CsvParserOptions(Dialect dialect, bool skipHeader, int degreeOfParallelism, bool keepOrder)
        {
            Dialect = dialect;
            SkipHeader = skipHeader;
            DegreeOfParallelism = degreeOfParallelism;
            KeepOrder = keepOrder;
        }

        public override string ToString()
        {
            return string.Format($"CsvParserOptions (Dialect = {Dialect}, DegreeOfParallelism = {DegreeOfParallelism}, KeepOrder = {KeepOrder})");
        }
    }
}
