﻿using System.IO;
using System.Xml;

namespace JackAnalyzer
{
    public class CompilationEngine
    {
        private JackTokenizer tokenizer;
        private XmlWriter xw;

        public CompilationEngine(JackTokenizer tokenizer, string outputFilePath)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                //IndentChars = "",
                Encoding = new System.Text.UTF8Encoding(false),
                //NewLineHandling = NewLineHandling.None,
            };

            xw = XmlWriter.Create(outputFilePath, settings);

            this.tokenizer = tokenizer;
        }

        public void CompileClass()
        {
            xw.WriteStartElement("class");
            CompileKeyword("class");
            CompileIdentifier(tokenizer.currentToken); // className
            CompileSymbol("{");

            // classVarDec*
            CompileClassVarDec();
            CompileSubRoutineDec();

            CompileSymbol("}");
            xw.WriteEndElement(); // end class
            xw.WriteString("\n");
        }

        // Compiles a static/field declaration (classVarDec*)
        private void CompileClassVarDec()
        {
            if (tokenizer.currentToken != "static" &&
                tokenizer.currentToken != "field")
            {
                return;
            }

            xw.WriteStartElement("classVarDec");
            CompileKeyword(tokenizer.currentToken); // static | field
            CompileType(tokenizer.currentToken);
            CompileIdentifier(tokenizer.currentToken); // varName

            // field int x, y;
            while (tokenizer.currentToken == ",")
            {
                CompileSymbol(",");
                CompileIdentifier(tokenizer.currentToken); // varName2, etc
            }

            CompileSymbol(";");
            xw.WriteEndElement();
            CompileClassVarDec();
        }

        // Compiles a complete method, function, or constructor
        private void CompileSubRoutineDec()
        {
            // e.g. function void main()
            if (tokenizer.currentToken != "constructor" &&
                tokenizer.currentToken != "function" &&
                tokenizer.currentToken != "method")
            {
                return;
            }

            xw.WriteStartElement("subroutineDec");

            CompileKeyword(tokenizer.currentToken);

            // method Employee getEmployee()
            if (tokenizer.TokenType() == JackTokenizer.Token.KEYWORD)
            {
                CompileKeyword("void");
            } else
            {
                CompileType(tokenizer.currentToken);
            }

            CompileIdentifier(tokenizer.currentToken); // subRoutineName
            CompileSymbol("(");
            CompileParameterList();
            CompileSymbol(")");

            xw.WriteStartElement("subroutineBody");

            CompileSymbol("{");

            // varDec*
            CompileVarDec();

            CompileStatements();
            CompileSymbol("}");

            xw.WriteEndElement(); // end subRoutineBody
            xw.WriteEndElement(); // end subRoutineDec
            CompileSubRoutineDec();
        }
        // Compiles a (possibly empty) parameter list. Not including the enclosing ()
        // int aX, int aY
        private void CompileParameterList()
        {
            xw.WriteStartElement("parameterList");

            while (tokenizer.currentToken != ")")
            {
                CompileType(tokenizer.currentToken);
                CompileIdentifier(tokenizer.currentToken);

                if (tokenizer.currentToken == ",")
                {
                    CompileSymbol(",");
                }
            }

            xw.WriteFullEndElement(); // end parameterList
        }

        // Compiles a var declaration
        private void CompileVarDec()
        {
            if (tokenizer.currentToken != "var")
            {
                return;
            }

            xw.WriteStartElement("varDec");
            CompileKeyword("var");
            CompileType(tokenizer.currentToken);
            CompileIdentifier(tokenizer.currentToken);

            while (tokenizer.currentToken == ",")
            {
                CompileSymbol(",");
                CompileIdentifier(tokenizer.currentToken);
            }

            CompileSymbol(";");
            xw.WriteEndElement(); // endVarDec
            CompileVarDec();
        }

        // Compiles a sequence of statements
        private void CompileStatements()
        {
            // TODO: Recursive approach
            //if (currToken == ";" || currToken == "}")
            //    return;
            //else
            xw.WriteStartElement("statements");

            // loop through all statements
            while (tokenizer.currentToken != ";" && tokenizer.currentToken != "}")
            {
                switch (tokenizer.currentToken)
                {
                    case "let":
                        CompileLet();
                        break;
                    case "if":
                        CompileIf();
                        break;
                    case "while":
                        CompileWhile();
                        break;
                    case "do":
                        CompileDo();
                        break;
                    case "return":
                        CompileReturn();
                        break;
                    default:
                        throw new InvalidDataException("Not a valid statement");
                }
            }

            xw.WriteFullEndElement(); // end statements
        }

        // Compiles a 'do' statement
        private void CompileDo()
        {
            xw.WriteStartElement("doStatement");
            CompileKeyword("do");
            // TODO: Subroutine call
            CompileSubroutineCall();
            CompileSymbol(";");
            xw.WriteEndElement(); // end doStatement
        }

        // Compiles a 'let' statement
        private void CompileLet()
        {
            xw.WriteStartElement("letStatement");
            CompileKeyword("let");
            CompileIdentifier(tokenizer.currentToken);

            // array [ i + 1 ]
            if (tokenizer.currentToken == "[")
            {
                CompileSymbol("[");
                CompileExpression();
                CompileSymbol("]");
            }

            CompileSymbol("=");
            CompileExpression();
            CompileSymbol(";");
            xw.WriteEndElement(); // end letStatement
        }

        // Compiles a 'while' statement
        private void CompileWhile()
        {
            xw.WriteStartElement("whileStatement");
            CompileKeyword("while");
            CompileSymbol("(");
            CompileExpression();
            CompileSymbol(")");
            CompileSymbol("{");
            CompileStatements();
            CompileSymbol("}");
            xw.WriteEndElement();
        }

        // Compiles a 'return' statement
        private void CompileReturn()
        {
            xw.WriteStartElement("returnStatement");
            CompileKeyword("return");

            if (tokenizer.currentToken != ";")
            {
                CompileExpression();
            }

            CompileSymbol(";");
            xw.WriteEndElement(); // end returnStatement
        }

        // Compiles an 'if' statement
        // With a possible trailing 'else'
        private void CompileIf()
        {
            xw.WriteStartElement("ifStatement");
            CompileKeyword("if");
            CompileSymbol("(");
            CompileExpression();
            CompileSymbol(")");
            CompileSymbol("{");
            CompileStatements();
            CompileSymbol("}");

            // If there's an else block...
            if (tokenizer.currentToken == "else")
            {
                CompileKeyword("else");
                CompileSymbol("{");
                CompileStatements();
                CompileSymbol("}");
            }

            xw.WriteEndElement(); // end ifStatement
        }

        private void CompileExpression()
        {
            xw.WriteStartElement("expression");
            CompileTerm();

            while (tokenizer.currentToken.IsValidOperand())
            {
                // op
                CompileSymbol(tokenizer.currentToken);
                CompileTerm();
            }

            xw.WriteEndElement();
        }

        private void CompileTerm()
        {
            var tokenType = tokenizer.TokenType();

            xw.WriteStartElement("term");

            // Compile '('expression')' e.g (500 / 10)
            if (tokenizer.currentToken == "(")
            {
                CompileSymbol("(");
                CompileExpression();
                CompileSymbol(")");
                xw.WriteEndElement(); // end term
                return;
            }

            // Compile UnaryOp term =>  ~(10 > 2)
            if (tokenizer.currentToken == "-" ||
                tokenizer.currentToken == "~")
            {
                CompileSymbol(tokenizer.currentToken);
                CompileTerm();
            }

            // TODO: No default ?
            switch (tokenType)
            {
                case JackTokenizer.Token.INT_CONST:
                    xw.WriteStartElement("integerConstant");
                    xw.WriteString(tokenizer.GetKeyWord());
                    xw.WriteEndElement();
                    tokenizer.Advance();
                    break;
                case JackTokenizer.Token.STRING_CONST:
                    xw.WriteStartElement("stringConstant");
                    xw.WriteString(tokenizer.StringVal());
                    xw.WriteEndElement();
                    tokenizer.Advance();
                    break;
                case JackTokenizer.Token.KEYWORD:
                    xw.WriteStartElement("keyword");
                    xw.WriteString(tokenizer.GetKeyWord());
                    xw.WriteEndElement();
                    tokenizer.Advance();
                    break;
                // subroutineCall(expression) / varName[expression]
                case JackTokenizer.Token.IDENTIFIER:
                    CompileSubroutineCallOrArray();
                    break;
            }

            xw.WriteEndElement(); // end term
        }

        // Compile subroutine call e.g.
        // add(3 + 5, 2) | Math.add(3, 7) | square.moveUp()

        // Compile array e.g.
        // names[0 + 1], fruits[i]

        // Helper method
        private void CompileSubroutineCall()
        {
            CompileIdentifier(tokenizer.currentToken);

            if (tokenizer.currentToken == ".")
            {
                CompileSymbol(".");
                CompileIdentifier(tokenizer.currentToken);
            }

            if (tokenizer.currentToken == "(")
            {
                CompileSymbol("(");
                CompileExpressionList();
                CompileSymbol(")");
            }
        }

        // Compile subroutine call e.g.
        // add(3 + 5, 2) | Math.add(3, 7) | square.moveUp()

        // Compile array e.g.
        // names[0 + 1], fruits[i]

        // Helper method
        private void CompileSubroutineCallOrArray()
        {
            CompileIdentifier(tokenizer.currentToken);

            if (tokenizer.currentToken == ".")
            {
                CompileSymbol(".");
                CompileIdentifier(tokenizer.currentToken);
            }

            if (tokenizer.currentToken == "(")
            {
                CompileSymbol("(");
                CompileExpressionList();
                CompileSymbol(")");
            }

            if (tokenizer.currentToken == "[")
            {
                CompileSymbol("[");
                CompileExpression();
                CompileSymbol("]");
            }
        }

        // Compiles a possibly empty comma-separated
        // list of expressions
        private int CompileExpressionList()
        {
            xw.WriteStartElement("expressionList");

            int i = 0;

            if (tokenizer.currentToken == ")")
            {
                xw.WriteFullEndElement(); // end expressionList
                return i;
            }

            CompileExpression();

            // Go through the rest of the expressions
            while (tokenizer.currentToken == ",")
            {
                CompileSymbol(",");
                CompileExpression();
                i++;
            }

            xw.WriteFullEndElement(); // end expressionList
            return i;
        }

        // Helper method
        private void CompileIdentifier(string identifier)
        {
            if (!identifier.IsValidIdentifier())
            {
                throw new System.Exception($"{identifier} is not a valid identifier.");
            }

            xw.WriteStartElement("identifier");
            xw.WriteString(tokenizer.GetKeyWord());
            xw.WriteEndElement();

            tokenizer.Advance();
        }

        // Helper method
        private void CompileType(string type)
        {
            bool isCustomType;

            if (!type.IsValidType(out isCustomType))
            {
                throw new System.Exception($"{type} is not a valid type.");
            }

            if (isCustomType)
            {
                CompileIdentifier(type);
                return;
            }

            xw.WriteStartElement("keyword");
            xw.WriteString(tokenizer.GetKeyWord());
            xw.WriteEndElement();

            tokenizer.Advance();
        }

        // Helper method
        private void CompileSymbol(string symbol)
        {
            if (tokenizer.currentToken != symbol)
            {
                throw new System.Exception($"Invalid token! Expected {symbol} got {tokenizer.currentToken}");
            }

            if (tokenizer.TokenType() != JackTokenizer.Token.SYMBOL)
            {
                throw new System.Exception($"{symbol} is not a valid symbol");
            }

            xw.WriteStartElement("symbol");
            xw.WriteString(tokenizer.GetKeyWord());
            xw.WriteEndElement();

            tokenizer.Advance();
        }

        // Helper method
        private void CompileKeyword(string kw)
        {
            if (tokenizer.currentToken != kw)
            {
                throw new System.Exception($"Invalid token! Expected {kw} got {tokenizer.currentToken}");
            }

            if (tokenizer.TokenType() != JackTokenizer.Token.KEYWORD)
            {
                throw new System.Exception($"{kw} is not a valid keyword");
            }

            xw.WriteStartElement("keyword");
            xw.WriteString(tokenizer.GetKeyWord());
            xw.WriteEndElement();

            tokenizer.Advance();
        }

        // Closes the output file
        public void ShutDown()
        {
            tokenizer.fs.Close();
            xw.Close();
        }
    }
}
