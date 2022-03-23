using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Number, RightBraces, LeftBraces, String, Comment, DataType_INT, DataType_String, DataType_Float, Repeat, Return, Endl, DoubleDot
    , AndOperator, OROperator, Assign, Elseif,MAIN

}
namespace JASON_Compiler
{

    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("IF", Token_Class.If);
            ReservedWords.Add("BEGIN", Token_Class.Begin);
            ReservedWords.Add("CALL", Token_Class.Call);
            ReservedWords.Add("DECLARE", Token_Class.Declare);
            ReservedWords.Add("END", Token_Class.End);
            ReservedWords.Add("DO", Token_Class.Do);
            ReservedWords.Add("ELSE", Token_Class.Else);
            ReservedWords.Add("ENDIF", Token_Class.EndIf);
            ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            ReservedWords.Add("INTEGER", Token_Class.Integer);
            ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("PROGRAM", Token_Class.Program);
            ReservedWords.Add("READ", Token_Class.Read);
            ReservedWords.Add("REAL", Token_Class.Real);
            ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);
            ReservedWords.Add("INT", Token_Class.DataType_INT);
            ReservedWords.Add("FLOAT", Token_Class.DataType_Float);
            ReservedWords.Add("STRING", Token_Class.DataType_String);
            ReservedWords.Add("REPEAT", Token_Class.Repeat);
            ReservedWords.Add("RETURN", Token_Class.Return);
            ReservedWords.Add("ENDL", Token_Class.Endl);
            ReservedWords.Add("ELSEIF", Token_Class.Elseif);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);
            Operators.Add("&&", Token_Class.AndOperator);
            Operators.Add("||", Token_Class.OROperator);
            // Operators.Add(":", Token_Class.DoubleDot);

        }

        public void StartScanning(string SourceCode)
        {
            // i: Outer loop to check on lexemes.
            for (int i = 0; i < SourceCode.Length; i++)
            {
                bool flag = true;
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (char.IsLetter(CurrentChar))
                {
                    //integer
                    if (SourceCode.Length > j + 1)
                    {
                        while (char.IsLetterOrDigit(SourceCode[j + 1]))
                        {
                            flag = true;
                            CurrentLexeme += SourceCode[j + 1].ToString();
                            j++;
                            if (SourceCode.Length == j + 1)
                                break;
                        }
                        i = j;
                    }
                }
                else if (char.IsDigit(CurrentChar))
                {

                    if (SourceCode.Length > j + 1)
                    {
                        int cnt = 0;
                        while (char.IsLetterOrDigit(SourceCode[j + 1]) || SourceCode[j + 1] == '.')
                        {
                            if (char.IsLetter(SourceCode[j + 1]))
                                flag = false;
                            if (SourceCode[j + 1] == '.')
                                cnt++;
                            CurrentLexeme += SourceCode[j + 1].ToString();
                            j++;
                            if (SourceCode.Length == j + 1)
                                break;
                        }
                        i = j;
                        if (cnt > 1 || !flag)
                        {
                            Errors.Error_List.Add(CurrentLexeme);
                            continue;
                        }
                    }
                }
                else if (CurrentChar == '\"')
                {
                    bool s = true;
                    if (SourceCode.Length > j + 1)
                    {
                        while (SourceCode[j + 1] != '\"')
                        {

                            CurrentLexeme += SourceCode[j + 1].ToString();
                            j++;
                            if (SourceCode.Length == j + 1)
                            {
                                s = false;
                                break;
                            }
                        }
                        if (s)
                            CurrentLexeme += SourceCode[j + 1].ToString();
                        i = j + 1;
                    }
                }
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    bool s = true;
                    if (SourceCode.Length > j + 1)
                    {
                        while (SourceCode[j + 1] != '/')
                        {

                            CurrentLexeme += SourceCode[j + 1].ToString();
                            j++;
                            if (SourceCode.Length == j + 1)
                            {
                                s = false;
                                break;
                            }
                        }
                        if (s)
                            CurrentLexeme += SourceCode[j + 1].ToString();
                        i = j + 1;
                    }
                }
                else if (CurrentChar == ':' && SourceCode[i + 1] == '=')
                {
                    CurrentLexeme += SourceCode[j + 1].ToString();
                    i = j + 1;
                }
                else if (CurrentChar == '&' && SourceCode[i + 1] == '&')
                {
                    CurrentLexeme += SourceCode[j + 1].ToString();
                    i = j + 1;
                }
                else if (CurrentChar == '|' && SourceCode[i + 1] == '|')
                {
                    CurrentLexeme += SourceCode[j + 1].ToString();
                    i = j + 1;
                }
                else if (CurrentChar == '<' && SourceCode[i + 1] == '>')
                {
                    CurrentLexeme += SourceCode[j + 1].ToString();
                    i = j + 1;
                }
                FindTokenClass(CurrentLexeme);
            }

            JASON_Compiler.TokenStream = Tokens;
        }

        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex.ToUpper()))
            {
                Tok.token_type = ReservedWords[Lex.ToUpper()];
                Tokens.Add(Tok);
            }
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Tok.lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }
            //Is it a Constant?
            else if (isConstant(Tok.lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else if (isString(Tok.lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            else if (isComment(Tok.lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            else if (Tok.lex != "\t")
            {
                Errors.Error_List.Add(Tok.lex);

            }
        }

        bool isIdentifier(string lex)
        {
            bool isValid = true;
            // Check if the lex is an identifier or not.
            Regex re = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9])*", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            Regex re = new Regex(@"^[0-9]+(\.([0-9])+)?", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }
        bool isComment(string lex)
        {
            bool isValid = true;
            // Check if the lex is a Comment or not.

            Regex re = new Regex(@"^\/\*.*\*\/$", RegexOptions.Compiled);
            if (re.IsMatch(lex.TrimEnd()))
                return isValid;
            return false;
        }
        bool isArithmaticOperation(string lex)
        {
            bool isValid = true;
            // Check if the lex is a Arithmatic Operation or not.
            Regex re = new Regex(@"\+ | \- | \* | \/", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }
        bool isBooleanOperation(string lex)
        {
            bool isValid = true;
            // Check if the lex is a Boolean Operation or not.
            Regex re = new Regex(@"\&\& | \|\|", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }
        bool isconditionOperation(string lex)
        {
            bool isValid = true;
            // Check if the lex is a condition Operation or not.
            Regex re = new Regex(@"\< | \= | \> | \<\>", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }
        bool isString(string lex)
        {
            bool isValid = true;
            // Check if the lex is a constant (Number) or not.
            Regex re = new Regex("^\".*\"$", RegexOptions.Compiled);
            if (re.IsMatch(lex))
                return isValid;
            return false;
        }

    }
}