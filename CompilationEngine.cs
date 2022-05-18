using System.IO;
using System.Xml;

namespace JackAnalyzer
{
    internal class CompilationEngine
    {
        private JackTokenizer tokenizer;
        //private StreamReader inputStreamReader; 
        //private StreamWriter outputStreamWriter;
        private XmlWriter xw;

        public CompilationEngine(JackTokenizer tokenizer, string outputFilePath)
        {
            //inputStreamReader = new StreamReader(inputFilePath);
            //outputStreamWriter = new StreamWriter(outputFilePath);

            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                //NewLineChars = "--",
                //NewLineOnAttributes = true,
            };

            xw = XmlWriter.Create(outputFilePath, settings);

            this.tokenizer = tokenizer;
        }

        public void CompileClass()
        {
            xw.WriteStartElement("tokens");

            while (tokenizer.HasMoreTokens())
            {
                switch (tokenizer.TokenType())
                {
                    case JackTokenizer.Token.KEYWORD:
                        xw.WriteStartElement("keyword");
                        xw.WriteString(tokenizer.GetKeyWord());
                        xw.WriteEndElement();
                        break;
                    case JackTokenizer.Token.SYMBOL:
                        xw.WriteStartElement("symbol");
                        xw.WriteString(tokenizer.GetKeyWord());
                        xw.WriteEndElement();
                        break;
                    case JackTokenizer.Token.INT_CONST:
                        xw.WriteStartElement("integerConstant");
                        //xw.WriteValue(tokenizer.IntVal());
                        xw.WriteString(tokenizer.GetKeyWord());
                        xw.WriteEndElement();
                        break;
                    case JackTokenizer.Token.STRING_CONST:
                        xw.WriteStartElement("stringConstant");
                        xw.WriteString(tokenizer.StringVal());
                        //xw.WriteString(tokenizer.GetKeyWord());
                        xw.WriteEndElement();
                        break;
                    default:
                        xw.WriteStartElement("identifier");
                        xw.WriteString(tokenizer.GetKeyWord());
                        xw.WriteEndElement();
                        break;
                }

                tokenizer.Advance();
            }

            xw.WriteEndElement();
        }

        // Compiles a static/field declaration
        private void CompileClassVarDec()
        {

        }

        // Compiles a complete method, function, or constructor
        private void CompileSubRoutine()
        {

        }

        // Compiles a (possibly empty) parameter list. Not includeing the enclosing ()
        private void CompileParameterList()
        {

        }

        // Compiles a var declaration
        private void CompileVarDec()
        {
        
        }

        // Compiles a sequence of statements
        private void CompileStatements()
        {

        }

        // Compiles a 'do' statement
        private void CompileDo()
        {
            
        }

        // Compiles a 'let' statement
        private void CompileLet()
        {

        }

        // Compiles a 'while' statement
        private void CompileWhile()
        {

        }

        // Compiles a 'return' statement
        private void CompileReturn()
        {

        }

        // Compiles an 'if' statement
        // With a possible trailing 'else'
        private void CompileIf()
        {

        }

        private void CompileExpression()
        {

        }

        private void CompileTerm()
        {

        }

        // Compiles a possibly empty comma-separated
        // list of expressions
        private void CompileExpressionList()
        {

        }

        // Closes the output file
        public void ShutDown()
        {
            tokenizer.fs.Close();
            xw.Dispose();
        }
    }
}
