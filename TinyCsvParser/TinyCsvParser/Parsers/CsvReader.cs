using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TinyCsvParser.Parsers
{
    /// <summary>
    /// Based on the Python Parser at https://github.com/python/cpython/blob/master/Modules/_csv.c.
    /// </summary>
    public class CsvReader
    {
        public enum StateEnum
        {
            START_RECORD,
            START_FIELD,
            ESCAPED_CHAR,
            IN_FIELD,
            IN_QUOTED_FIELD,
            ESCAPE_IN_QUOTED_FIELD,
            QUOTE_IN_QUOTED_FIELD,
            EAT_CRNL,
            AFTER_ESCAPED_CRNL
        }

        private readonly Dialect dialect;
        private readonly StringBuilder builder;
        private readonly List<string> fields;

        private StateEnum state;

        public CsvReader(Dialect dialect)
        {
            this.dialect = dialect;
            this.builder = new StringBuilder(1000);
            this.fields = new List<string>(1000);
            this.state = StateEnum.START_RECORD;
        }

        private void ProcessCharacter(char c)
        {
            switch (state)
            {
                case StateEnum.START_RECORD:
                    if (c == '\0')
                    {
                        break;
                    } else if (c == '\n' || c == '\r')
                    {
                        state = StateEnum.EAT_CRNL;
                        break;
                    }

                    state = StateEnum.START_FIELD;

                    goto case StateEnum.START_FIELD;

                case StateEnum.START_FIELD:
                    if (c == '\n' || c == '\r' || c == '\0')
                    {
                        SaveField();

                        state = c == '\0' ? StateEnum.START_RECORD : StateEnum.EAT_CRNL;
                    }
                    else if (c == dialect.QuoteChar && dialect.Quoting != QuoteStyleEnum.QUOTE_NONE)
                    {
                        state = StateEnum.IN_QUOTED_FIELD;
                    }
                    else if (c == dialect.EscapeChar)
                    {
                        state = StateEnum.ESCAPED_CHAR;
                    }
                    else if (c == ' ' && dialect.SkipInitialSpace)
                    {
                        // Ignore Whitespace
                    }
                    else if (c == dialect.Delimiter)
                    {
                        SaveField();
                    }
                    else
                    {
                        AddCharacter(c);
                        state = StateEnum.IN_FIELD;
                    }
                    break;

                case StateEnum.ESCAPED_CHAR:

                    if (c == '\n' || c == '\r')
                    {
                        AddCharacter(c);

                        state = StateEnum.AFTER_ESCAPED_CRNL;
                    }

                    if (c == '\0')
                    {
                        c = '\n';
                    }

                    AddCharacter(c);

                    state = StateEnum.IN_FIELD;

                    break;

                case StateEnum.AFTER_ESCAPED_CRNL:

                    if (c == '\0')
                        break;

                    goto case StateEnum.IN_FIELD;

                case StateEnum.IN_FIELD:
                    if (c == '\n' || c == '\r' || c == '\0')
                    {
                        SaveField();

                        state = c == '\0' ? StateEnum.START_RECORD : StateEnum.EAT_CRNL;
                    }
                    else if (c == dialect.EscapeChar)
                    {
                        state = StateEnum.ESCAPED_CHAR;
                    }
                    else if (c == dialect.Delimiter)
                    {
                        SaveField();

                        state = StateEnum.START_FIELD;
                    }
                    else
                    {
                        AddCharacter(c);
                    }
                    break;

                case StateEnum.IN_QUOTED_FIELD:
                    if (c == '\0')
                    {
                        
                    }
                    else if (c == dialect.EscapeChar)
                    {
                        state = StateEnum.ESCAPE_IN_QUOTED_FIELD;
                    }
                    else if (c == dialect.QuoteChar && dialect.Quoting != QuoteStyleEnum.QUOTE_NONE)
                    {
                        if (dialect.DoubleQuote)
                        {
                            state = StateEnum.QUOTE_IN_QUOTED_FIELD;
                        }
                        else
                        {
                            state = StateEnum.IN_FIELD;
                        }
                    }
                    else
                    {
                        AddCharacter(c);
                    }
                    break;

                case StateEnum.ESCAPE_IN_QUOTED_FIELD:
                    if (c == '\0')
                    {
                        c = '\n';
                    }

                    AddCharacter(c);

                    state = StateEnum.IN_QUOTED_FIELD;

                    break;

                case StateEnum.QUOTE_IN_QUOTED_FIELD:

                    if (dialect.Quoting != QuoteStyleEnum.QUOTE_NONE && c == dialect.QuoteChar)
                    {
                        AddCharacter(c);

                        state = StateEnum.IN_QUOTED_FIELD;
                    }
                    else if (c == dialect.Delimiter)
                    {
                        SaveField();

                        state = StateEnum.START_FIELD;
                    }
                    else if (c == '\n' || c == '\r' || c == '\0')
                    {
                        SaveField();

                        state = c == '\0' ? StateEnum.START_RECORD : StateEnum.EAT_CRNL;
                    }
                    else if (!dialect.Strict)
                    {
                        AddCharacter(c);

                        state = StateEnum.IN_FIELD;
                    }
                    else
                    {
                        throw new Exception($"'{dialect.Delimiter}' expected after '{dialect.QuoteChar}'");
                    }
                    break;
                case StateEnum.EAT_CRNL:
                    if (c == '\r' || c == '\n')
                    {

                    }
                    else if (c == '\0')
                    {
                        state = StateEnum.START_RECORD;
                    }
                    else
                    {
                        throw new Exception("New-Line Character seen in Unquoted Field - do you need to open the file in universal-newline mode?");
                    }
                    break;
            }
        }

        private void SaveField()
        {
            fields.Add(builder.ToString());

            builder.Clear();
        }

        private void AddCharacter(char c)
        {
            builder.Append(c);
        }

        public IEnumerable<string[]> Read(StreamReader reader)
        {
            string currentLine;

            while ((currentLine = reader.ReadLine()) != null)
            {
                foreach (var c in currentLine)
                {
                    if (c == '\0')
                    {
                        throw new Exception("Line contains a NULL Byte.");
                    }

                    ProcessCharacter(c);
                }

                yield return fields.ToArray();

                fields.Clear();
                builder.Clear();
            }
        }
    }
}