namespace Oxi.Tests
{
    using System.Linq;
    using Xunit;

    public class ScannerTest
    {
        [Fact]
        public void TestNextToken()
        {
            var input = "=+(){},;";
            var tests = new[]
            {
                (TokenKind.Equal, "="),
                (TokenKind.Plus, "+"),
                (TokenKind.LeftParen, "("),
                (TokenKind.RightParen, ")"),
                (TokenKind.LeftBrace, "{"),
                (TokenKind.RightBrace, "}"),
                (TokenKind.Comma, ","),
                (TokenKind.Semicolon, ";"),
            };

            var scanner = new Scanner();
            var tokens = scanner.Tokenize(input).ToArray();

            for (var i = 0; i < tests.Length; i++)
            {
                var (expectedKind, expectedStringValue) = tests[i];
                var actual = tokens[i];
                Assert.Equal(expectedKind, actual.Kind);
                Assert.Equal(expectedStringValue, actual.ToStringValue());
            }
        }
    }
}
