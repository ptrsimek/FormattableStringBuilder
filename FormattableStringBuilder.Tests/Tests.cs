namespace FormattableStringBuilder.Tests
{
    using System;
    using Xunit;
    
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var b = new FormattableStringBuilder();
            b.Append($"x = {42}");

            var fs = b.ToFormattableString();

            Assert.Equal(fs.Format, "x = {0}");
            Assert.Equal(fs.ArgumentCount, 1);
            Assert.Equal(fs.GetArgument(0), 42);
        }
    }
}