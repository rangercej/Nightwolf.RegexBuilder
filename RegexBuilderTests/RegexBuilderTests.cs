namespace Nightwolf.Regex
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Text.RegularExpressions;

    [TestClass]
    public class RegexBuilderTests
    {
        [TestMethod]
        public void TestCharClassesHaveDefinition()
        {
            foreach (CharacterClass c in Enum.GetValues(typeof(CharacterClass)))
            {
                Assert.IsTrue(CharacterClassValues.CharClasses.ContainsKey(c));
            }
        }

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
            
            Assert.IsTrue(!regex.IsMatch("The cat sat on the mat"));
            Assert.IsTrue(regex.IsMatch("The cat sat on the mat."));
        }

        [TestMethod]
        public void TestRaw()
        {
            var regex = new RegexBuilder(RegexScope.StartsWith)
                .Literal("The ", true)
                .Raw("c[auo]t")
                .ToRegex();

            Assert.IsTrue(regex.IsMatch("The cat"));
            Assert.IsTrue(regex.IsMatch("The cut"));
            Assert.IsTrue(!regex.IsMatch("The cet"));
        }

        [TestMethod]
        public void TestScopeDefaultIsAnywhere()
        {
            var regex = new RegexBuilder().Literal("cat").ToRegex();

            Assert.IsTrue(regex.IsMatch("cat"));
            Assert.IsTrue(regex.IsMatch("The cat"));
            Assert.IsTrue(regex.IsMatch("The cat sat"));
            Assert.IsTrue(regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeAnywhere()
        {
            var regex = new RegexBuilder(RegexScope.Anywhere).Literal("cat").ToRegex();
            Assert.IsTrue(regex.IsMatch("cat"));
            Assert.IsTrue(regex.IsMatch("The cat"));
            Assert.IsTrue(regex.IsMatch("The cat sat"));
            Assert.IsTrue(regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeStartsWith()
        {
            var regex = new RegexBuilder(RegexScope.StartsWith).Literal("cat").ToRegex();
            Assert.IsTrue(regex.IsMatch("cat"));
            Assert.IsTrue(!regex.IsMatch("The cat"));
            Assert.IsTrue(!regex.IsMatch("The cat sat"));
            Assert.IsTrue(regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeEndsWith()
        {
            var regex = new RegexBuilder(RegexScope.EndsWith).Literal("cat").ToRegex();

            Assert.IsTrue(regex.IsMatch("cat"));
            Assert.IsTrue(regex.IsMatch("The cat"));
            Assert.IsTrue(!regex.IsMatch("The cat sat"));
            Assert.IsTrue(!regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestScopeFullLine()
        {
            var regex = new RegexBuilder(RegexScope.FullLine).Literal("cat").ToRegex();

            Assert.IsTrue(regex.IsMatch("cat"));
            Assert.IsTrue(!regex.IsMatch("The cat"));
            Assert.IsTrue(!regex.IsMatch("The cat sat"));
            Assert.IsTrue(!regex.IsMatch("cat sat"));
        }

        [TestMethod]
        public void TestAnyOfChar()
        {
            var chars = new[] { '^', 'a', 'b', 'c', ']', '-' };
            var regex = new RegexBuilder().AnyOf(chars).ToRegex();

            Assert.IsTrue(regex.IsMatch("cab"));
            Assert.IsTrue(regex.IsMatch("See this [thing]"));
            Assert.IsTrue(regex.IsMatch("Over-excited"));
            Assert.IsTrue(!regex.IsMatch("Kitten"));
        }

        [TestMethod]
        public void TestNotAnyOfChar()
        {
            var chars = new[] { '^', 'o', 'a' };
            var regex = new RegexBuilder().Literal("c")
                .Not().AnyOf(chars)
                .Literal("t").ToRegex();

            Assert.IsTrue(regex.IsMatch("cut"));
            Assert.IsTrue(!regex.IsMatch("cat"));
            Assert.IsTrue(!regex.IsMatch("cot"));
            Assert.IsTrue(!regex.IsMatch("c^t"));
        }

        [TestMethod]
        public void TestAnyOfCharClass()
        {
            var chars = new[] { CharacterClass.AlphaLower, CharacterClass.Digits };
            var regex = new RegexBuilder().AnyOf(chars).ToRegex();

            Assert.IsTrue(regex.IsMatch("Letters"));
            Assert.IsTrue(regex.IsMatch("Letters and 1234"));
            Assert.IsTrue(regex.IsMatch("LETTERS AND 1234"));
            Assert.IsTrue(regex.IsMatch("1234"));
            Assert.IsTrue(!regex.IsMatch("LETTERS"));
        }

        [TestMethod]
        public void TestNotAnyOfCharClass()
        {
            var chars = new[] { CharacterClass.AlphaLower, CharacterClass.Digits };
            var regex = new RegexBuilder().Not().AnyOf(chars).ToRegex();

            Assert.IsTrue(regex.IsMatch("Letters"));
            Assert.IsTrue(regex.IsMatch("LETTERS 1234"));
            Assert.IsTrue(!regex.IsMatch("letters"));
            Assert.IsTrue(!regex.IsMatch("1234"));
        }

        [TestMethod]
        public void TestAnyOfString()
        {
            var strings = new[] { "cat", "sat", "mat" };
            var regex = new RegexBuilder().AnyOf(strings).ToRegex();

            Assert.IsTrue(regex.IsMatch("The cat sits on the carpet"));
            Assert.IsTrue(regex.IsMatch("The kitty sits on the mat"));
            Assert.IsTrue(!regex.IsMatch("The kitty sits on the carpet"));
        }

        [TestMethod]
        public void TestNotAnyOfString()
        {
            var strings = new[] { "cat", "kitty", "pussycat" };
            var regex = new RegexBuilder().Literal("The ").Not().AnyOf(strings).ToRegex();

            Assert.IsTrue(!regex.IsMatch("The cat sits on the carpet"));
            Assert.IsTrue(!regex.IsMatch("The kitty sits on the mat"));
            Assert.IsTrue(regex.IsMatch("The dog sits on the carpet"));
        }

        [TestMethod]
        public void TestLimitOne()
        {
            var regex = new RegexBuilder(RegexScope.StartsWith)
                .Literal("dbo.").Repeat(RegexRepeats.One)
                .ToRegex();

            Assert.IsTrue(regex.IsMatch("dbo.TableName") && !regex.IsMatch("TableName"));
        }

        [TestMethod]
        public void TestLimitZeroOrOne()
        {
            var regex = new RegexBuilder(RegexScope.StartsWith)
                .Literal("dbo.").Repeat(RegexRepeats.ZeroOrOne)
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
                    .Literal("dbo.").Repeat(RegexRepeats.ZeroOrOne)
                    .AnyOf(alphabet).Repeat(RegexRepeats.OneOrMore).Capture()
                    .Literal(" ")
                    .ToRegex(RegexOptions.IgnoreCase);

            var match1 = regex.Match(sql1);
            var match2 = regex.Match(sql2);

            Assert.IsTrue(match1 != null && match1.Groups[1].Value == "Bob");
            Assert.IsTrue(match2 != null && match2.Groups[1].Value == "Bob");
        }

        [TestMethod]
        public void TestOneshotFlag()
        {
            var val = new AutoResetFlag(true);
            Assert.IsTrue(val.Peek());
            Assert.IsTrue(val.Read());
            Assert.IsTrue(!val.Peek());
            Assert.IsTrue(!val.Read());
            Assert.IsTrue(!val.Peek());
            val.Set();
            Assert.IsTrue(val.Peek());
            Assert.IsTrue(val.Read());
            Assert.IsTrue(!val.Peek());
            Assert.IsTrue(!val.Read());
            Assert.IsTrue(!val.Peek());

            val = new AutoResetFlag(false);
            Assert.IsTrue(!val.Peek());
            Assert.IsTrue(!val.Read());
            Assert.IsTrue(!val.Peek());
            val.Set();
            Assert.IsTrue(val.Peek());
            Assert.IsTrue(val.Read());
            Assert.IsTrue(!val.Peek());
            Assert.IsTrue(!val.Read());
            Assert.IsTrue(!val.Peek());
        }
    }
}
