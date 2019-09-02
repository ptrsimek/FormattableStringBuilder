namespace FormattableStringBuilder.Tests
{
    using System;
    using Xunit;
    
    public class Tests
    {
        [Fact]
        public void TestBasicUsage()
        {
            var b = new FormattableStringBuilder();
            b.Append($"x = {42}");

            var fs = b.ToFormattableString();

            Assert.Equal(fs.Format, "x = {0}");
            Assert.Equal(fs.ArgumentCount, 1);
            Assert.Equal(fs.GetArgument(0), 42);
        }
        
        [Fact]
        public void TestMultiple()
        {
            var b = new FormattableStringBuilder();
            b.Append($"x = {42}").Append($", y = {43}");

            var fs = b.ToFormattableString();

            Assert.Equal(fs.Format, "x = {0}, y = {1}");
            Assert.Equal(fs.ArgumentCount, 2);
            Assert.Equal(fs.GetArgument(0), 42);
            Assert.Equal(fs.GetArgument(1), 43);
        }
        
        [Fact]
        public void TestRaw()
        {
            var b = new FormattableStringBuilder();
            b.Append($"x = {42}").AppendRaw(" AND ").Append($"y = {43}");

            var fs = b.ToFormattableString();

            Assert.Equal(fs.Format, "x = {0} AND y = {1}");
            Assert.Equal(fs.ArgumentCount, 2);
            Assert.Equal(fs.GetArgument(0), 42);
            Assert.Equal(fs.GetArgument(1), 43);
        }
        
        [Fact]
        public void TestValue()
        {
            var b = new FormattableStringBuilder();
            b.AppendRaw("x = ").Append(42);

            var fs = b.ToFormattableString();

            Assert.Equal(fs.Format, "x = {0}");
            Assert.Equal(fs.ArgumentCount, 1);
            Assert.Equal(fs.GetArgument(0), 42);
        }
    }
}