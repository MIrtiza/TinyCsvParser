// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Mapping.Builder;
using TinyCsvParser.Model;

namespace TinyCsvParser.Test.Integration
{
    [TestFixture]
    public class DynamicMappingTest
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }
        }

        [Test]
        public void PersonDynamicMappingTest()
        {
            // The Options:
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });

            // The Dynamic Mapping:
            CsvMappingBuilder<Person> builder = new CsvMappingBuilder<Person>()
                .MapProperty("FirstName", x => x.FirstName)
                .MapProperty("LastName", x => x.LastName)
                .MapProperty("BirthDate", x => x.BirthDate);

            // The CSV Data:
            string csvDataString = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986/05/12")
                .AppendLine("Max;Mustermann;2014/01/01")
                .ToString();

            // Turn it into an Enumerable using Split:
            string[] csvDataArray = csvDataString
                .Split(csvReaderOptions.NewLine, StringSplitOptions.None);

            // Get the Column Names:
            string[] columnNames = csvDataArray[0].Split(';');

            // Build the Parser:
            CsvMapping<Person> csvMapping = builder.Build(columnNames);
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapping);

            // Enumerate the Rows:
            IEnumerable<Row> rowData = csvDataArray.AsEnumerable()
                .Skip(1)
                .Select((line, pos) => new Row(pos, line));

            var results = csvParser
                .Parse(rowData)
                .ToList();

            // Get the header:

            Assert.AreEqual(2, results.Count());

            // Asserts ...
        }


    }
}
