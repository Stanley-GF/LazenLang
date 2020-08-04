﻿using LazenLang.Lexing;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace LazenLang.Parsing.Ast.Expressions.Literals
{
    class NegNum
    {
        public Expr Value;

        public NegNum(Expr value)
        {
            Value = value;
        }

        public static ExprNode Consume(Parser parser)
        {
            var blockedBeforeNegSymbol = new List<TokenInfo.TokenType>()
            {
                TokenInfo.TokenType.IDENTIFIER,
                TokenInfo.TokenType.R_BRACKET,
                TokenInfo.TokenType.R_PAREN,
                TokenInfo.TokenType.INTEGER_LIT,
                TokenInfo.TokenType.DOUBLE_LIT,
                TokenInfo.TokenType.STRING_LIT,
                TokenInfo.TokenType.CHAR_LIT,
                TokenInfo.TokenType.BOOLEAN_LIT,
            };

            if (parser.LastTokenEaten != null && blockedBeforeNegSymbol.Contains(parser.LastTokenEaten.Type))
                throw new ParserError(new FailedConsumer(), parser.Cursor);

            Token prefix = parser.TryManyEats(new TokenInfo.TokenType[] { TokenInfo.TokenType.PLUS, TokenInfo.TokenType.MINUS });
         
            Expr expression;
            try
            {
                expression = parser.TryConsumer(ExprNode.Consume).Value;
            } catch (ParserError ex)
            {
                if (!ex.IsErrorFromParserClass())
                    throw ex;
                throw new ParserError(
                    new ExpectedElementException("Expected expression after PLUS or MINUS prefix"),
                    parser.Cursor
                );
            }

            if (prefix.Type == TokenInfo.TokenType.MINUS)
                return new ExprNode(
                    new NegExpr(expression), parser.Cursor
                );
            else
                return new ExprNode(
                    expression, parser.Cursor
                );
        }
    }
}
