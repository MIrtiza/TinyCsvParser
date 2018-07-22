// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;
using TinyCsvParser.Parsers;

namespace TinyCsvParser
{
    public class CsvParser<TEntity> : ICsvParser<TEntity>
        where TEntity : class, new()
    {
        private readonly CsvParserOptions options;
        private readonly CsvMapping<TEntity> mapping;

        public CsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            this.options = options;
            this.mapping = mapping;
        }

        public ParallelQuery<CsvMappingResult<TEntity>> Parse(StreamReader stream)
        {
            var reader = new CsvReader(options.Dialect);

            var query = 
                reader.Read(stream)
                .Select((fields, index) => new TokenizedRow(index, fields))
                .Skip(options.SkipHeader ? 1 : 0)
                .AsParallel();
            
            // If you want to get the same order as in the CSV file, this option needs to be set:
            if (options.KeepOrder)
            {
                query = query.AsOrdered();
            }

            // Add Parallelization Options:
            query = query
                .WithDegreeOfParallelism(options.DegreeOfParallelism);

            return query
                .Where(row => row.Tokens.Length > 0)
                .Select(row => mapping.Map(row));
        }

        public override string ToString()
        {
            return $"CsvParser (Options = {options}, Mapping = {mapping})";
        }
    }
}
