using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum TOKEN_ENUM
{
    TOKEN_INT,
    TOKEN_STRING,
    TOKEN_FLOAT,
    TOKEN_IDENTIFIER,
    TOKEN_LBRACE,
    TOKEN_RBRACE,
    TOKEN_PLUS_OPERATOR, //+
    TOKEN_MINUS_OPERATOR, //-
    TOKEN_DIVIDE_OPERATOR, // /
    TOKEN_MULTIPLY_OPERATOR, //*
    TOKEN_COMMA, //,
    TOKEN_BINDING, //:=
    TOKEN_EQUALITY_OPERATOR, //=
    TOKEN_LESS_THAN, //<
    TOKEN_ANDING, //&&
    TOKEN_ORING, //||
    TOKEN_IF, //
    TOKEN_THEN,
    TOKEN_ELSE, //
    TOKEN_ElseIf, //
    TOKEN_EndL, //
    TOKEN_READ,
    TOKEN_WRITE,
    TOKEN_REPEAT,
    TOKEN_UNTIL,
    TOKEN_RETURN,
    TOKEN_SEMICOLON,
    TOKEN_DOT,
    TOKEN_GREATER_THAN,
    TOKEN_NOT_EQUAL,
    TOKEN_COMMENT,
    TOKEN_LPAREN,
    TOKEN_RPAREN
}

namespace TinyL_Compiler
{
    public class Token
    {
        public string _lexeme;
        public TOKEN_ENUM _type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, TOKEN_ENUM> ReservedWords = new Dictionary<string, TOKEN_ENUM>();
        Dictionary<string, TOKEN_ENUM> Operators = new Dictionary<string, TOKEN_ENUM>();

        readonly Regex FloatRegex = new Regex(@"^0|([1-9][0-9]*(\.[0-9]+))$", RegexOptions.Compiled);
        readonly Regex IntRegex = new Regex(@"^0|([1-9][0-9]*)$", RegexOptions.Compiled);
        readonly Regex StringRegex = new Regex("^\"[^\"]*\"$", RegexOptions.Compiled);
        readonly Regex CommentRegex = new Regex(@"^/\*([^*]|(\*+[^/]))*\*/$", RegexOptions.Compiled); /**/ /*/**/
        readonly Regex IdentifierRegex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);


        public Scanner()
        {
            ReservedWords.Add("IF", TOKEN_ENUM.TOKEN_IF);
            ReservedWords.Add("ELSE", TOKEN_ENUM.TOKEN_ELSE);
            ReservedWords.Add("ELSEIF", TOKEN_ENUM.TOKEN_ElseIf);
            ReservedWords.Add("ENDL", TOKEN_ENUM.TOKEN_EndL);
            ReservedWords.Add("FLOAT", TOKEN_ENUM.TOKEN_FLOAT);
            ReservedWords.Add("INT", TOKEN_ENUM.TOKEN_INT);
            ReservedWords.Add("STRING", TOKEN_ENUM.TOKEN_STRING);
            ReservedWords.Add("READ", TOKEN_ENUM.TOKEN_READ);
            ReservedWords.Add("WRITE", TOKEN_ENUM.TOKEN_WRITE);
            ReservedWords.Add("THEN", TOKEN_ENUM.TOKEN_THEN);
            ReservedWords.Add("UNTIL", TOKEN_ENUM.TOKEN_UNTIL);
            ReservedWords.Add("REPEAT", TOKEN_ENUM.TOKEN_REPEAT);
            ReservedWords.Add("RETURN", TOKEN_ENUM.TOKEN_RETURN);

            Operators.Add(".", TOKEN_ENUM.TOKEN_DOT);
            Operators.Add(";", TOKEN_ENUM.TOKEN_SEMICOLON);
            Operators.Add(",", TOKEN_ENUM.TOKEN_COMMA);
            Operators.Add("{", TOKEN_ENUM.TOKEN_LBRACE);
            Operators.Add("}", TOKEN_ENUM.TOKEN_RBRACE);
            Operators.Add("(", TOKEN_ENUM.TOKEN_LPAREN);
            Operators.Add(")", TOKEN_ENUM.TOKEN_RPAREN);
            Operators.Add("=", TOKEN_ENUM.TOKEN_EQUALITY_OPERATOR); //comparison
            Operators.Add("<", TOKEN_ENUM.TOKEN_LESS_THAN);
            Operators.Add(">", TOKEN_ENUM.TOKEN_GREATER_THAN);
            Operators.Add("+", TOKEN_ENUM.TOKEN_PLUS_OPERATOR);
            Operators.Add("-", TOKEN_ENUM.TOKEN_MINUS_OPERATOR);
            Operators.Add("*", TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR);
            Operators.Add("/", TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR);

            Operators.Add("<>", TOKEN_ENUM.TOKEN_NOT_EQUAL);
            Operators.Add(":=", TOKEN_ENUM.TOKEN_BINDING); //Assignment
            Operators.Add("&&", TOKEN_ENUM.TOKEN_ANDING);
            Operators.Add("||", TOKEN_ENUM.TOKEN_ORING);
        }

        public void StartScanning(string SourceCode)
        {
            SourceCode += " ";
            SourceCode.ToLower();
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();  //id num op comm "str"

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '<' ||
                    CurrentChar == '.' || CurrentChar == '>' || CurrentChar == '=' || CurrentChar == ',' ||
                    CurrentChar == ';' || CurrentChar == '{' || CurrentChar == '}' || CurrentChar == '&' ||
                    CurrentChar == '|' || CurrentChar == ':' || CurrentChar == '(' || CurrentChar == ')')
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                        {
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                        ++j;
                    }
                    FindTokenClass(CurrentLexeme.Trim());
                    i = j -1;
                }
                else if (Char.IsLetter(CurrentChar)) //if you read a character
                {
                    ++j;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (!Char.IsLetterOrDigit(CurrentChar))
                        {
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                        ++j;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }

                else if (Char.IsDigit(CurrentChar))
                {
                    ++j;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t' || CurrentChar == ';')
                        {
                            break;
                        }
                        CurrentLexeme += CurrentChar;
                        ++j;
                    }
                    string Trimmed_CurrentLexeme = CurrentLexeme.TrimStart(new Char[] { '0' });
                    if (Trimmed_CurrentLexeme == "")
                    {
                        FindTokenClass("0");
                    }
                    FindTokenClass(Trimmed_CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '"')
                {
                    ++j;
                    while (j < SourceCode.Length)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar;
                        ++j;
                        if (CurrentChar == '"')
                        {

                            break;
                        }

                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '/')
                {
                    ++j;
                    char exCurrentChar = CurrentChar;
                    CurrentChar = SourceCode[j];
                    if (CurrentChar == '*')
                    {
                        while (j < SourceCode.Length)
                        {
                            exCurrentChar = CurrentChar;
                            CurrentChar = SourceCode[j];
                            CurrentLexeme += CurrentChar;
                            if (exCurrentChar == '*' && CurrentChar == '/')
                            {
                                break;
                            }
                            ++j;
                        }
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                    else if(CurrentChar == ' ')
                    {
                        FindTokenClass(CurrentLexeme);
                        i = j - 1;
                    }
                }
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok._lexeme = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok._type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok._type = TOKEN_ENUM.TOKEN_IDENTIFIER;
                Tokens.Add(Tok);
            }
            else if (isFloat(Lex))
            {
                Tok._type = TOKEN_ENUM.TOKEN_FLOAT;
                Tokens.Add(Tok);
            }
            else if (isInt(Lex))
            {
                Tok._type = TOKEN_ENUM.TOKEN_INT;

                Tokens.Add(Tok);
            }
            
            else if (isDigitError(Lex))
            {
                Errors.Error_List.Add(Lex);
            }
            else if (isComment(Lex))
            {

                Tok._type = TOKEN_ENUM.TOKEN_COMMENT;
                Tokens.Add(Tok);
            }
            else if (isCommentError(Lex))
            {
                Errors.Error_List.Add(Lex);
            }
            else if (isString(Lex))
            {
                Tok._type = TOKEN_ENUM.TOKEN_STRING;
                Tokens.Add(Tok);
            }
            else if (isStringError(Lex))
            {
                Errors.Error_List.Add(Lex);
            }
            else if (isOperator(Lex))
            {
                Tokens.Add(new Token
                {
                    _lexeme = Lex,
                    _type = Operators[Lex]
                });
            }
            else
            {
                Errors.Error_List.Add(Lex);
            }
        }
        bool isIdentifier(string Lexeme)
        {
            // Check if the lex is an identifier or not.
            return IdentifierRegex.IsMatch(Lexeme);
        }

        bool isInt(string Lexeme)
        {
            return IntRegex.IsMatch(Lexeme);
        }

        bool isFloat(string Lexeme)
        {
            return FloatRegex.IsMatch(Lexeme);
        }

        bool isComment(string Lexeme)
        {
            return CommentRegex.IsMatch(Lexeme);
        }
        bool isCommentError(string Lexeme)
        {
            return Lexeme.Length > 1 && Lexeme.Substring(Lexeme.Length - 2).Equals("*/");
        }
        bool isDigitError(string Lexeme)
        {
            foreach(char c in Lexeme){
                if (c > 9) return false;
            }
            return true;
        }
        bool isOperator(string Lexeme)
        {
            if (Operators.ContainsKey(Lexeme))
            {
                return true;
            }

            return false;
        }

        bool isString(string Lexeme)
        {
            return StringRegex.IsMatch(Lexeme);
        }
        bool isStringError(string Lexeme)
        {
            return Lexeme.Length > 1 && Lexeme.Substring(Lexeme.Length - 1).Equals("\"");
        }

    }
}