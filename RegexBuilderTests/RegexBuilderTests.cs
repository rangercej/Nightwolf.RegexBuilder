using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nightwolf
{
    [TestClass]
    public class RegexBuilderTests
    {
        [TestMethod]
        public void TestLiteral1()
        {
            var regex = new RegexBuilder().Literal("Hello").ToRegex();

            var s = "This is a Hello world";

            Assert.IsTrue(regex.IsMatch(s));
        }

        [TestMethod]
        public void TestLiteral2()
        {
            var regex = new RegexBuilder().Literal(".").ToRegex();
            
            var result1 = regex.IsMatch("This is a hello world");
            var result2 = regex.IsMatch("This is a hello world.");

            Assert.IsTrue(result2 && !result1);
        }

        [TestMethod]
        public void TestRegEx()
        {
            var sql1 = "CREATE TABLE dbo.Bob (a INT IDENTITY(1,1))";
            var sql2 = "CREATE TABLE Bob (a INT IDENTITY(1,1))";
            char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

            var regex = new RegexBuilder()
                    .Literal("CREATE TABLE ", true)
                    .Literal("dbo.").Limit(RegexBuilder.Repeats.ZeroOrOne)
                    .AnyOf(alphabet).Limit(RegexBuilder.Repeats.OneOrMore).Capture()
                    .Literal(" ")
                    .ToRegex(System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var match1 = regex.Match(sql1);
            var match2 = regex.Match(sql2);

            var match1pass = match1 != null && match1.Groups[1].Value == "Bob";
            var match2pass = match2 != null && match2.Groups[1].Value == "Bob";

            Assert.IsTrue(match1pass && match2pass);
        }
    }
}
