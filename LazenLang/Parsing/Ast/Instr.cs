﻿using LazenLang.Lexing;
using LazenLang.Parsing.Ast.Statements;
using LazenLang.Parsing.Ast.Statements.Functions;
using LazenLang.Parsing.Ast.Statements.Loops;
using LazenLang.Parsing.Ast.Statements.OOP;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazenLang.Parsing.Ast
{
    abstract class Instr
    {
        public virtual string Pretty()
        {
            return "Instr";
        }
    }

    class InstrNode
    {
        public Instr Value;
        public CodePosition Position;

        public InstrNode(Instr instruction, CodePosition position)
        {
            Value = instruction;
            Position = position;
        }

        public static string PrettyMultiple(InstrNode[] instructions)
        {
            string result = "{";
            for (int i = 0; i < instructions.Length; i++)
            {
                Instr instr = instructions[i].Value;
                result += instr.Pretty();
                if (i < instructions.Length - 1) result += ", ";
            }
            return result + "}";
        }

        public static InstrNode Consume(Parser parser)
        {
            CodePosition oldCursor = parser.Cursor;
            
            Instr instr;
            switch (parser.LookAhead().Type)
            {
                case TokenInfo.TokenType.EOL:
                case TokenInfo.TokenType.L_CURLY_BRACKET:
                    instr = parser.TryConsumer((Parser p) => Block.Consume(p));
                    break;

                case TokenInfo.TokenType.WHILE:
                    instr = parser.TryConsumer(WhileLoop.Consume);
                    break;
                
                case TokenInfo.TokenType.FOR:
                    instr = parser.TryConsumer(ForLoop.Consume);
                    break;

                case TokenInfo.TokenType.CONST:
                case TokenInfo.TokenType.VAR:
                    instr = parser.TryConsumer((Parser p) => VarDecl.Consume(p));
                    break;

                case TokenInfo.TokenType.RETURN:
                    instr = parser.TryConsumer(ReturnInstr.Consume);
                    break;

                case TokenInfo.TokenType.BREAK:
                    instr = parser.TryConsumer(BreakInstr.Consume);
                    break;
                
                case TokenInfo.TokenType.CONTINUE:
                    instr = parser.TryConsumer(ContinueInstr.Consume);
                    break;

                default:
                    instr = parser.TryManyConsumers(new Func<Parser, Instr>[] {
                        VarMutation.Consume,
                        ExprInstr.Consume
                    });
                    break;
            }

            if (instr == null)
                throw new ParserError(new FailedConsumer(), parser.Cursor);
            
            return new InstrNode(instr, oldCursor);
        }
    }
}