// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace TinyCsvParser.Mapping.Property
{
    public class IndexToPropertyMapping<TEntityType>
        where TEntityType : class, new()
    {
        public int ColumnIndex { get; set; }

        public ICsvPropertyMapping<TEntityType> PropertyMapping { get; set; }

        public override string ToString()
        {
            return string.Format("IndexToPropertyMapping (ColumnIndex = {0}, PropertyMapping = {1}", ColumnIndex,
                PropertyMapping);
        }
    }
}