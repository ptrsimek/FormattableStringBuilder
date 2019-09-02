namespace FormattableStringBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class FormattableStringBuilder
    {
        private readonly List<object> arguments = new List<object>();

        private readonly StringBuilder buffer = new StringBuilder();

        private int argumentsOffset;

        public FormattableString ToFormattableString()
        {
            var format = this.buffer.ToString();
            var args = this.arguments.ToArray();

            var result = FormattableStringFactory.Create(format, args);
            return result;
        }

        public override string ToString()
        {
            return this.buffer.ToString();
        }

        public FormattableStringBuilder AppendRaw(string value)
        {
            this.buffer.Append(value);
            return this;
        }

        public FormattableStringBuilder Append(object value)
        {
            this.buffer.Append("{").Append(this.argumentsOffset).Append("}");
            this.arguments.Add(value);
            this.argumentsOffset++;

            return this;
        }

        public FormattableStringBuilder Append(FormattableString value)
        {
            this.AppendFormatHelper(value.Format, value.ArgumentCount);
            this.arguments.AddRange(value.GetArguments());

            this.argumentsOffset += value.ArgumentCount;

            return this;
        }

        // https://referencesource.microsoft.com/#mscorlib/system/text/stringbuilder.cs,2c3b4c2e7c43f5a4
        private void AppendFormatHelper(string format, int argsLength)
        {
            var pos = 0;
            var len = format.Length;
            var ch = '\x0';

            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];
                    pos++;

                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}')
                        {
                            // Treat as escape character for }}
                            pos++;
                        }
                        else
                        {
                            FormatError(format, pos);
                        }
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{')
                        {
                            pos++;
                        }
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    this.buffer.Append(ch);
                }

                if (pos == len)
                {
                    break;
                }

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                {
                    FormatError(format, pos);
                }

                var parameterIndex = 0;
                do
                {
                    parameterIndex = parameterIndex * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                    {
                        FormatError(format, pos);
                    }

                    ch = format[pos];
                }
                while (ch >= '0' && ch <= '9' && parameterIndex < 1000000);

                if (parameterIndex >= argsLength)
                {
                    throw new FormatException(
                        $"Index must be greater than or equal to zero and less than the size of the argument list. "
                        + $"Index: {parameterIndex}, format: '{format}', argument list size: {argsLength}.");
                }

                this.buffer.Append("{").Append(this.argumentsOffset + parameterIndex);

                // skip spaces
                while (pos < len && (ch = format[pos]) == ' ')
                {
                    pos++;
                }

                if (ch == ',')
                {
                    this.buffer.Append(',');

                    // format {1,5}
                    var width = 0;
                    pos++;

                    // skip spaces
                    while (pos < len && format[pos] == ' ')
                    {
                        pos++;
                    }

                    if (pos == len)
                    {
                        FormatError(format, pos);
                    }

                    ch = format[pos];

                    if (ch == '-')
                    {
                        this.buffer.Append('-');
                        pos++;

                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                    }

                    if (ch < '0' || ch > '9')
                    {
                        FormatError(format, pos);
                    }

                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                    }
                    while (ch >= '0' && ch <= '9' && width < 1000000);

                    this.buffer.Append(width);
                }

                // skip spaces
                while (pos < len && (ch = format[pos]) == ' ')
                {
                    pos++;
                }

                if (ch == ':')
                {
                    // {0:D}
                    pos++;

                    while (true)
                    {
                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                        pos++;

                        if (ch == '{')
                        {
                            if (pos < len && format[pos] == '{')
                            {
                                // Treat as escape character for {{
                                pos++;
                            }
                            else
                            {
                                FormatError(format, pos);
                            }
                        }
                        else if (ch == '}')
                        {
                            if (pos < len && format[pos] == '}')
                            {
                                // Treat as escape character for }}
                                pos++;
                            }
                            else
                            {
                                pos--;
                                break;
                            }
                        }

                        this.buffer.Append(ch);
                    }
                }

                if (ch != '}')
                {
                    FormatError(format, pos);
                }

                pos++;

                this.buffer.Append('}');
            }
        }

        private static void FormatError(string format, int position)
        {
            throw new FormatException($"Input string was not in a correct format. Format: '{format}', position: {position}.");
        }
    }
}