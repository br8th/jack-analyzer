using JackAnalyzer;

namespace JackAnalyzer.Test
{
    [TestClass]
    public class JackTokenizerTests
    {
        private string filePath = "D:\\Code-Personal\\JackAnalyzer\\JackAnalyzer.Test\\Files\\HelloWorld\\Main.jack";

        private string squareDir = "D:\\Code-Personal\\JackAnalyzer\\JackAnalyzer.Test\\Files\\Square";
        private string helloDir = "D:\\Code-Personal\\JackAnalyzer\\JackAnalyzer.Test\\Files\\HelloWorld";

        private IEnumerable<object[]> TestDirs =>
            new List<string[]>
            {
                new string[] { squareDir },
                new string[] { helloDir }
            };

        [TestMethod]
        public void Tokenizer_HasMoreTokens()
        {
            JackTokenizer tokenizer = new JackTokenizer(filePath);
            Assert.IsTrue(tokenizer.HasMoreTokens());
        }

        [TestMethod]
        public void Tokenizer_TokenType_Keyword_Correct()
        {
            JackTokenizer tokenizer = new JackTokenizer(filePath);
            Assert.AreEqual(tokenizer.TokenType(), JackTokenizer.Token.KEYWORD);
        }

        [TestMethod]
        public void Tokenizer_TokenType_Identifier_Correct()
        {
            JackTokenizer tokenizer = new JackTokenizer(filePath);
            tokenizer.Advance();
            Assert.AreEqual(tokenizer.TokenType(), JackTokenizer.Token.IDENTIFIER);
        }

        [TestMethod]
        public void Tokenizer_TokenType_Symbol_Correct()
        {
            JackTokenizer tokenizer = new JackTokenizer(filePath);
            tokenizer.Advance();
            tokenizer.Advance();
            Assert.AreEqual(tokenizer.TokenType(), JackTokenizer.Token.SYMBOL);
        }

        [TestMethod]
        public void Tokenizer_Square_Main_Translates()
        {
            JackTokenizer tokenizer = new JackTokenizer(squareDir + "\\Main.jack");

            CompilationEngine engine = new CompilationEngine(tokenizer, squareDir + "\\Main.xml");
            engine.CompileClass();
            engine.ShutDown();

            var outputFile = $"{squareDir}\\Main.xml";
            var expectedFile = $"{squareDir}\\MainT.xml";

            Assert.IsTrue(TestUtils.FilesAreEqual(outputFile, expectedFile));
        }

        [DataTestMethod]
        [DataRow("D:\\Code-Personal\\JackAnalyzer\\JackAnalyzer.Test\\Files\\Square")]
        [DataRow("D:\\Code-Personal\\JackAnalyzer\\JackAnalyzer.Test\\Files\\ArrayTest")]
        public void Tokenizer_Square_Dir_Translates(string dirName)
        {
            var filesInDir = Directory.GetFiles(dirName, "*.jack");

            foreach (string fileName in filesInDir)
            {
                var outputFileName = fileName.Replace(".jack", ".xml");
                JackTokenizer tokenizer = new JackTokenizer(fileName);
                CompilationEngine engine = new CompilationEngine(tokenizer, outputFileName);
                engine.CompileClass();
                engine.ShutDown();

                var expectedFile = outputFileName.Replace(".xml", "T.xml");

                Assert.IsTrue(TestUtils.FilesAreEqual(outputFileName, expectedFile));
            }
        }
    }
}