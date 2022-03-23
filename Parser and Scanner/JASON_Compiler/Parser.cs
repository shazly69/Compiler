using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node idList()
        {
            Node idList = new Node("idList");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                idList.Children.Add(match(Token_Class.Idenifier));
                idList.Children.Add(idListD());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                idList.Children.Add(match(Token_Class.Number));
                idList.Children.Add(idListD());
            }
            else
            {
                idList.Children.Add(idListD());
            }
            return idList;
        }
        Node idListD()
        {
            Node idListD = new Node("idListD");
            if (TokenStream[InputPointer].lex == ",")
            {
                idListD.Children.Add(match(Token_Class.Comma));
                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    idListD.Children.Add(match(Token_Class.Idenifier));
                    idListD.Children.Add(this.idListD());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    idListD.Children.Add(match(Token_Class.Number));
                    idListD.Children.Add(this.idListD());
                }
            }
            else
            {
                return null;
            }
            return idListD;
        }
        Node functionCall()
        {
            Node functionCall = new Node("functionCall");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            functionCall.Children.Add(idList());
            functionCall.Children.Add(match(Token_Class.RParanthesis));
            return functionCall;
        }
        Node Term()
        {
            Node term = new Node("Term");

            if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                if (InputPointer + 1 < TokenStream.Count)
                {
                    if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                    {
                        term.Children.Add(functionCall());
                    }
                    else
                        term.Children.Add(match(Token_Class.Idenifier));
                }
                else
                    term.Children.Add(match(Token_Class.Idenifier));

            }
            return term;
        }

        Node AOp()
        {
            Node AOp = new Node("AOp");
            if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                AOp.Children.Add(match(Token_Class.DivideOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                AOp.Children.Add(match(Token_Class.MinusOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                AOp.Children.Add(match(Token_Class.MultiplyOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                AOp.Children.Add(match(Token_Class.PlusOp));
            }
            else return null;
            return AOp;
        }

        Node Y()
        {
            Node Y = new Node("Y");
            Y.Children.Add(match(Token_Class.LParanthesis));
            Y.Children.Add(Term());
            Y.Children.Add(X());
            Y.Children.Add(match(Token_Class.RParanthesis));
            Y.Children.Add(Q());
            return Y;
        }

        Node Z()
        {
            Node Z = new Node("Z");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.LParanthesis)
            {
                Z.Children.Add(Term());
                Z.Children.Add(Q());
            }
            else
            {
                Z.Children.Add(Y());
            }
            return Z;
        }

        Node X()
        {
            Node X = new Node("X");

            if (AOp() != null)
            {
                InputPointer--;
                X.Children.Add(AOp());
                X.Children.Add(Z());
            }
            else
                return null;
            return X;
        }

        Node Q()
        {
            Node q = new Node("Q");
            if (InputPointer < TokenStream.Count && AOp() != null)
            {
                InputPointer--;
                q.Children.Add(X());
            }
            else
            {
                return null;
            }
            return q;
        }
        Node Equation()
        {
            Node eq = new Node("Equation");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type != Token_Class.LParanthesis)
            {
                eq.Children.Add(Term());
                eq.Children.Add(X());
            }
            else
                eq.Children.Add(Y());
            return eq;
        }

        Node Expression()
        {
            //val-1
            Node exp = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                exp.Children.Add(match(Token_Class.String));
            }
            else if (InputPointer + 1 < TokenStream.Count)
            {
                InputPointer++;
                if (TokenStream[InputPointer - 1].token_type == Token_Class.Idenifier || TokenStream[InputPointer - 1].token_type == Token_Class.Number)
                {
                    if (AOp() == null)
                    {
                        InputPointer--;
                        exp.Children.Add(Term());
                    }
                    else
                    {
                        InputPointer--;
                        exp.Children.Add(Equation());
                    }

                }
                else if (TokenStream[InputPointer - 1].token_type == Token_Class.LParanthesis)
                {
                    InputPointer--;
                    exp.Children.Add(Equation());
                }
                else
                    return null;
            }
            else
            {
                exp.Children.Add(Term());
            }



            return exp;
        }

        Node Assignment_Statement()
        {
            Node asnmt = new Node("Assignment_statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                asnmt.Children.Add(match(Token_Class.Idenifier));
                asnmt.Children.Add(match(Token_Class.Assign));
                asnmt.Children.Add(Expression());
                return asnmt;
            }

            return null;
        }

        Node DataType()
        {
            Node dt = new Node("DataType");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DataType_INT)
            {
                dt.Children.Add(match(Token_Class.DataType_INT));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DataType_Float)
            {
                dt.Children.Add(match(Token_Class.DataType_Float));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.DataType_String)
            {
                dt.Children.Add(match(Token_Class.DataType_String));
            }
            else
                return null;
            return dt;
        }

        Node B()
        {
            Node B = new Node("B");
            B.Children.Add(Assignment_Statement());
            B.Children.Add(G());
            return B;
        }

        Node G()
        {
            Node G = new Node("G");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                G.Children.Add(match(Token_Class.Comma));
                G.Children.Add(S());
            }
            else
                return null;
            return G;
        }

        Node A()
        {
            Node A = new Node("A");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                A.Children.Add(match(Token_Class.Idenifier));
                A.Children.Add(G());
            }
            else
                return null;
            return A;
        }

        Node S()
        {
            Node S = new Node("S");
            if (InputPointer + 1 < TokenStream.Count)
            {
                if (TokenStream[InputPointer + 1].token_type != Token_Class.Assign)
                {
                    S.Children.Add(A());
                }
                else
                    S.Children.Add(B());
            }
            else
                S.Children.Add(A());
            return S;
        }

        Node declarationStatement()
        {
            Node ds = new Node("declarationStatement");
            int p = InputPointer;
            if (DataType() != null)
            {
                InputPointer = p;
                ds.Children.Add(DataType());
                ds.Children.Add(S());
                ds.Children.Add(match(Token_Class.Semicolon));
                return ds;
            }
            return null;
        }

        // shazly

        Node RepeatStatement()
        {
            Node RS = new Node("RepeatStatement");
            RS.Children.Add(match(Token_Class.Repeat));
            RS.Children.Add(Statements());
            RS.Children.Add(match(Token_Class.Until));
            RS.Children.Add(ConditionStatement());
            return RS;
        }
        Node Parameter()
        {
            Node parameter = new Node("parameter");

            parameter.Children.Add(DataType());
            parameter.Children.Add(match(Token_Class.Idenifier));
            return parameter;
        }
        Node ParamList()
        {
            Node PL = new Node("ParamList");
            int p = InputPointer;
            if (InputPointer < TokenStream.Count && Parameter() != null)
            {
                InputPointer = p;
                PL.Children.Add(Parameter());
                PL.Children.Add(ParamListD());
            }
            else PL.Children.Add(ParamListD());

            return PL;
        }
        Node ParamListD()
        {
            Node PLD = new Node("ParamList");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                PLD.Children.Add(match(Token_Class.Comma));
                PLD.Children.Add(Parameter());
                PLD.Children.Add(ParamListD());
            }
            else return null;

            return PLD;
        }
        Node FunctionDeclaration()
        {
            Node FD = new Node("FunctionDeclaration");

            FD.Children.Add(DataType());
            FD.Children.Add(match(Token_Class.Idenifier));
            FD.Children.Add(match(Token_Class.LParanthesis));
            FD.Children.Add(ParamList());
            FD.Children.Add(match(Token_Class.RParanthesis));
            return FD;
        }

        // end shazly


        // nahed

        Node IfState()
        {
            Node ifstate = new Node("ifstate");
            ifstate.Children.Add(match(Token_Class.If));
            ifstate.Children.Add(ConditionStatement());
            ifstate.Children.Add(match(Token_Class.Then));
            ifstate.Children.Add(Statements());
            ifstate.Children.Add(EndOfScope());
            return ifstate;
        }
        /*int main()
{

return 0;
}*/
        Node Statements()
        {
            Node statements = new Node("statements");
            while (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                statements.Children.Add(match(Token_Class.Comment));
            }
            int p = InputPointer;
            if (State() != null)
            {
                InputPointer = p;
                statements.Children.Add(State());
                statements.Children.Add(Statements());
                return statements;
            }

            return null;
        }
        Node ElseIfState()
        {
            Node elseifstate = new Node("elseifstate");
            elseifstate.Children.Add(match(Token_Class.Elseif));
            elseifstate.Children.Add(ConditionStatement());
            elseifstate.Children.Add(match(Token_Class.Then));
            elseifstate.Children.Add(Statements());
            elseifstate.Children.Add(EndOfScope());
            return elseifstate;
        }
        Node ElseState()
        {
            Node elsestate = new Node("elsestate");
            elsestate.Children.Add(match(Token_Class.Else));
            elsestate.Children.Add(Statements());
            elsestate.Children.Add(match(Token_Class.End));
            return elsestate;
        }
        Node EndOfScope()
        {
            Node endofscope = new Node("endofscope");
            if (TokenStream[InputPointer].token_type == Token_Class.Elseif)
            {
                endofscope.Children.Add(ElseIfState());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {
                endofscope.Children.Add(ElseState());
            }
            else
                endofscope.Children.Add(match(Token_Class.End));
            return endofscope;
        }
        Node State()
        {
            int p = InputPointer;
            Node state = new Node("state");
           
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                state.Children.Add(WriteStatement());
                return state;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                state.Children.Add(ReadStatement());
                return state;
            }
             else if (declarationStatement() != null)
            {
                InputPointer = p;
                state.Children.Add(declarationStatement());
                return state;
            }
            InputPointer = p;
            if (Assignment_Statement() != null)
            {
                InputPointer = p;
                state.Children.Add(Assignment_Statement());
                state.Children.Add(match(Token_Class.Semicolon));
                return state;
            }
            InputPointer = p;
            if (ReturnStatement() != null)
            {
                InputPointer = p;
                state.Children.Add(ReturnStatement());
                return state;
            }
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.If)
            {
                state.Children.Add(IfState());
                return state;
            }
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                state.Children.Add(RepeatStatement());
                return state;
            }

            return null;
        }

        // end nahed

        // mostafa 

        Node WriteStatement()
        {
            Node writeStatement = new Node("WriteStatement");
            if (InputPointer < TokenStream.Count)
            {
                writeStatement.Children.Add(match(Token_Class.Write));
                writeStatement.Children.Add(WriteStatementdash());
                writeStatement.Children.Add(match(Token_Class.Semicolon));

                return writeStatement;
            }
            return null;
        }
        Node WriteStatementdash()
        {
            Node writeStatementdash = new Node("WriteStatementdash");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                writeStatementdash.Children.Add(match(Token_Class.Endl));
            }
            else
            {
                writeStatementdash.Children.Add(Expression());
            }

            return writeStatementdash;
        }

        Node ReadStatement()
        {
            Node readStatement = new Node("ReadStatement");
            if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                readStatement.Children.Add(match(Token_Class.Read));
                readStatement.Children.Add(match(Token_Class.Idenifier));
                readStatement.Children.Add(match(Token_Class.Semicolon));
                return readStatement;
            }
            return null;
        }
        Node ReturnStatement()
        {
            Node returnStatement = new Node("ReturnStatement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                returnStatement.Children.Add(match(Token_Class.Return));
                returnStatement.Children.Add(Expression());
                returnStatement.Children.Add(match(Token_Class.Semicolon));
                return returnStatement;
            }
            return null;
        }
        Node CondtionOP()
        {
            Node condtionOP = new Node("CondtionOP");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                condtionOP.Children.Add(match(Token_Class.LessThanOp));
                return condtionOP;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                condtionOP.Children.Add(match(Token_Class.GreaterThanOp));
                return condtionOP;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                condtionOP.Children.Add(match(Token_Class.EqualOp));
                return condtionOP;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                condtionOP.Children.Add(match(Token_Class.NotEqualOp));
                return condtionOP;
            }
            return null;

        }

        Node Condition()
        {
            Node condition = new Node("Condition");
            condition.Children.Add(match(Token_Class.Idenifier));
            condition.Children.Add(CondtionOP());
            condition.Children.Add(Term());
            return condition;
        }
        Node BooleanOP()
        {
            Node booleanOP = new Node("BooleanOP");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.AndOperator)
            {
                booleanOP.Children.Add(match(Token_Class.AndOperator));
                return booleanOP;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.OROperator)
            {
                booleanOP.Children.Add(match(Token_Class.OROperator));
                return booleanOP;
            }

            return null;

        }
        Node ConditionStatement()
        {
            Node conditionStatement = new Node("ConditionStatement");
            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(ConditionStatementdash());
            return conditionStatement;
        }
        Node ConditionStatementdash()
        {
            Node conditionStatementdash = new Node("ConditionStatementdash");
            Node ret = BooleanOP();
            if (InputPointer < TokenStream.Count && ret != null)
            {
                conditionStatementdash.Children.Add(ret);
                conditionStatementdash.Children.Add(Condition());
                conditionStatementdash.Children.Add(ConditionStatementdash());
                return conditionStatementdash;
            }
            return null;
        }

        //end mostafa

        // mariam

        Node Program()
        {
            Node program = new Node("Program");
            //if (Comments() != null)
            //{
            //    program.Children.Add(Comments());
            //}
            while (InputPointer < TokenStream.Count && TokenStream[InputPointer ].token_type == Token_Class.Comment)
            {
                program.Children.Add(match(Token_Class.Comment));
            }
            Node ret = FunctionStatements();
            while (ret != null)
            {
                program.Children.Add(ret);
                if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    program.Children.Add(match(Token_Class.Comment));
                }
                ret = FunctionStatements();
            }
            while (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                program.Children.Add(match(Token_Class.Comment));
            }
            program.Children.Add(MainFunction());
            while (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                program.Children.Add(match(Token_Class.Comment));
            }
            //program.Children.Add(declarationStatement());
            return program;
        }
        Node MainFunction()
        {
            Node functionmain = new Node("MainFunction");
            functionmain.Children.Add(match(Token_Class.DataType_INT));
            functionmain.Children.Add(match(Token_Class.Idenifier));
            functionmain.Children.Add(match(Token_Class.LParanthesis));
            functionmain.Children.Add(match(Token_Class.RParanthesis));
            functionmain.Children.Add(FunctionBody());
            return functionmain;
        }
        Node FunctionStatements()
        {
            Node functionStatements = new Node("FunctionStatements");

            if (InputPointer+1 < TokenStream.Count && TokenStream[InputPointer + 1].lex != "main"&& TokenStream[InputPointer ].token_type!=Token_Class.Comment)
            {
                functionStatements.Children.Add(FunctionStatement());
                functionStatements.Children.Add(FunctionStatements());

            }
            else
            {
                return null;
            }
            return functionStatements;
        }
        Node FunctionStatement()
        {
            Node functionStatement = new Node("FunctionStatement");
            functionStatement.Children.Add(FunctionDeclaration());
            functionStatement.Children.Add(FunctionBody());

            return functionStatement;
        }
        Node FunctionBody()
        {
            Node functionbody = new Node("FunctionBody");
            functionbody.Children.Add(match(Token_Class.LeftBraces));
            functionbody.Children.Add(Statements()); //not imple yet
            functionbody.Children.Add(ReturnStatement()); //not imple yet
            functionbody.Children.Add(match(Token_Class.RightBraces));
            return functionbody;
        }

        // end mariam
        Node Comments()
        {
            Node comment = new Node("Comments");
            comment.Children.Add(match(Token_Class.Comment));
            comment.Children.Add(Comment());
            return comment;
        }
        Node Comment()
        {
            Node com = new Node("this is Comment");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].token_type == Token_Class.Comment)
            {
                com.Children.Add(Comments());
            }
            else return null;
            return com;
        }
        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
