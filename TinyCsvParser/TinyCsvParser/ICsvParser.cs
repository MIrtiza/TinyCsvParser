// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public interface ICsvParser<TEntity>
        where TEntity : class, new()
    {
        ParallelQuery<CsvMappingResult<TEntity>> Parse(StreamReader stream);
    }
}
