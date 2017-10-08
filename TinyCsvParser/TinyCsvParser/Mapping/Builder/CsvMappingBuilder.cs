using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCsvParser.Mapping.Property;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping.Builder
{
    public class CsvMappingBuilder<TEntityType>
        where TEntityType : class, new()
    {
        private readonly ITypeConverterProvider typeConverterProvider;
        private readonly IDictionary<string, ICsvPropertyMapping<TEntityType>> csvPropertyMappings;

        public CsvMappingBuilder()
            : this(new TypeConverterProvider())
        {
        }

        public CsvMappingBuilder(ITypeConverterProvider typeConverterProvider)
        {
            this.typeConverterProvider = typeConverterProvider;
            this.csvPropertyMappings = new Dictionary<string, ICsvPropertyMapping<TEntityType>>();
        }

        public CsvMappingBuilder<TEntityType> MapProperty<TProperty>(string columnName, Expression<Func<TEntityType, TProperty>> property)
        {
            return MapProperty(columnName, property, typeConverterProvider.Resolve<TProperty>());
        }

        public CsvMappingBuilder<TEntityType> MapProperty<TProperty>(string columnName, Expression<Func<TEntityType, TProperty>> property, ITypeConverter<TProperty> typeConverter)
        {
            if (csvPropertyMappings.ContainsKey(columnName))
            {
                throw new InvalidOperationException(string.Format("Duplicate mapping for column name '{0}'", columnName));
            }

            csvPropertyMappings[columnName] = new CsvPropertyMapping<TEntityType, TProperty>(property, typeConverter);

            return this;
        }

        public CsvMapping<TEntityType> Build(string[] header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            var mappings = header
                .Select((name, pos) =>
                {
                    return new IndexToPropertyMapping<TEntityType>
                    {
                        ColumnIndex = pos,
                        PropertyMapping = csvPropertyMappings[name]
                    };
                })
                .ToList();

            return new CsvMapping<TEntityType>(mappings);
        }
    }

}
