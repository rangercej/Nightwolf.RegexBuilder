namespace Nightwolf
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.RegularExpressions;

    [TestClass]
    public class RegexBuilderTests
    {
        [TestMethod]
        public void TestLiteral1()
        {
            var regex = new RegexBuilder().Literal("cat").ToRegex();

            Assert.IsTrue(regex.IsMatch("The cat sat on the mat."));
        }

        [TestMethod]
        public void TestLiteral2()
        {
            var regex = new RegexBuilder().Literal(".").ToRegex();
            
            var result1 = regex.IsMatch("The cat sat on the mat");
            var result2 = regex.IsMatch("The cat sat on the mat.");

            Assert.IsTrue(result2 && !result1);
        }

        [TestMethod]
        public void TestScopeDefaultIsAnywhere()
        {
            var regex = new RegexBuilder().Literal("cat").ToRegex();
            Assert.IsTrue(
                regex.IsMatch("cat")
                && regex.IsMatch("The cat")
                && regex.IsMatch("The cat sat")
                && regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeAnywhere()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.Anywhere).Literal("cat").ToRegex();
            Assert.IsTrue(
                regex.IsMatch("cat")
                && regex.IsMatch("The cat")
                && regex.IsMatch("The cat sat")
                && regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeStartsWith()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.StartsWith).Literal("cat").ToRegex();
            Assert.IsTrue(
                regex.IsMatch("cat")
                && !regex.IsMatch("The cat")
                && !regex.IsMatch("The cat sat")
                && regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeEndsWith()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.EndsWith).Literal("cat").ToRegex();
            Assert.IsTrue(
                regex.IsMatch("cat")
                && regex.IsMatch("The cat")
                && !regex.IsMatch("The cat sat")
                && !regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeFullLine()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.FullLine).Literal("cat").ToRegex();
            Assert.IsTrue(
                regex.IsMatch("cat")
                && !regex.IsMatch("The cat")
                && !regex.IsMatch("The cat sat")
                && !regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestAnyOfChar()
        {
            var chars = new[] { '^', 'a', 'b', 'c', ']', '-' };
            var regex = new RegexBuilder().AnyOf(chars).ToRegex();

            Assert.IsTrue(
                regex.IsMatch("cab")
                && regex.IsMatch("See this [thing]")
                && regex.IsMatch("Over-excited")
                && !regex.IsMatch("Kitten"));
        }

        [TestMethod]
        public void TestNotAnyOfChar()
        {
            var chars = new[] { '^', 'o', 'a' };
            var regex = new RegexBuilder().Literal("c")
                .Not().AnyOf(chars)
                .Literal("t").ToRegex();

            Assert.IsTrue(
                regex.IsMatch("cut")
                && !regex.IsMatch("cat")
                && !regex.IsMatch("cot")
                && !regex.IsMatch("c^t"));
        }

        [TestMethod]
        public void TestAnyOfString()
        {
            var strings = new[] { "cat", "sat", "mat" };
            var regex = new RegexBuilder().AnyOf(strings).ToRegex();

            Assert.IsTrue(
                regex.IsMatch("The cat sits on the carpet")
                && regex.IsMatch("The kitty sits on the mat")
                && !regex.IsMatch("The kitty sits on the carpet"));
        }

        [TestMethod]
        public void TestNotAnyOfString()
        {
            var strings = new[] { "cat", "kitty", "pussycat" };
            var regex = new RegexBuilder().Literal("The ").Not().AnyOf(strings).ToRegex();

            Assert.IsTrue(
                !regex.IsMatch("The cat sits on the carpet")
                && !regex.IsMatch("The kitty sits on the mat")
                && regex.IsMatch("The dog sits on the carpet"));
        }

        [TestMethod]
        public void TestLimitOne()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.StartsWith)
                .Literal("dbo.").Limit(RegexBuilder.Repeats.One)
                .ToRegex();

            Assert.IsTrue(regex.IsMatch("dbo.TableName") && !regex.IsMatch("TableName"));
        }

        [TestMethod]
        public void TestLimitZeroOrOne()
        {
            var regex = new RegexBuilder(RegexBuilder.Scope.StartsWith)
                .Literal("dbo.").Limit(RegexBuilder.Repeats.ZeroOrOne)
                .ToRegex();

            Assert.IsTrue(regex.IsMatch("dbo.TableName") && regex.IsMatch("TableName"));
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
                    .ToRegex(RegexOptions.IgnoreCase);

            var match1 = regex.Match(sql1);
            var match2 = regex.Match(sql2);

            var match1pass = match1 != null && match1.Groups[1].Value == "Bob";
            var match2pass = match2 != null && match2.Groups[1].Value == "Bob";

            Assert.IsTrue(match1pass && match2pass);
        }
    }
}
