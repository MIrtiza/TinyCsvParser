using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Parsers;
using TinyCsvParser.Test.Benchmarks;

namespace TinyCsvParser.Test.Parsers
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void ParsersTest()
        {
            var dialect = new Dialect()
            {
                Name = "Unit Test",
                QuoteChar = '"',
                Delimiter = ',',
                DoubleQuote = true,
                EscapeChar = '\\',
                SkipInitialSpace = true,
                Quoting = QuoteStyleEnum.QUOTE_ALL,
                Strict = true
            };

            var parser = new CsvReader(dialect);

            var parser = new CsvParser<LocalWeatherDataBenchmark.LocalWeatherData>();

            using (var stream = new StreamReader(
                stream: File.OpenRead(@"D:\datasets\201503hourly.txt"), 
                detectEncodingFromByteOrderMarks: false,
                encoding: Encoding.ASCII))
            {
                
            }
        }

    }
}
