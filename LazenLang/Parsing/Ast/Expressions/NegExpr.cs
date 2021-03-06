﻿using LazenLang.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LazenLang.Parsing.Ast.Expressions
{
    class NegExpr : Expr
    {
        public Expr Value;

        public NegExpr(Expr value)
        {
            Value = value;
        }

        public static Expr Consume(Parser parser)
        {
            parser.Eat(TokenInfo.TokenType.NEG);

            ExprNode expression;

            try
            {
                expression = parser.TryConsumer(ExprNode.Consume);
            } catch
            {
                throw new ParserError(
                    new ExpectedElementException("Expected expression after NEG token"),
                    parser.Cursor
                );
            }

            return new NegExpr(expression.Value);
        }

        public override string Pretty()
        {
            return $"NegExpr({Value.Pretty()})";
        }
    }
}
