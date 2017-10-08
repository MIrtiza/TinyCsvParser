// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingError
    {
        public int ColumnIndex { get; set; }

        public ErrorReasonEnum Reason { get; set; }

        public string Value { get; set; }

        public string Message { get; set; }
        
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return string.Format("CsvMappingError (ColumnIndex = {0}, Value = {1}, Exception = {2})", ColumnIndex, Value, Exception);
        }
    }
}
