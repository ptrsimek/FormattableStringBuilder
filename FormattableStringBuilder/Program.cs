namespace FormattableStringBuilder
{
    using System;

    public static class Program
    {
        public static void Main()
        {
            var x = "42";
            var y = "43";

            var b = new FormattableStringBuilder();
            b.Append($"x = {x}").AppendRaw(" AND ").Append($"y = {y}").Append("xxx");
            var fs = b.ToFormattableString();

            Console.WriteLine(fs);
        }
    }
}