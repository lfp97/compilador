using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Scanner
{
    public class ParserTable
    {
        public static List<String> buscar(String producao, String lexema)
        {
            List<String> resposta= new List<string>();
            if (producao.Equals("BLOCO"))
            {
                if (lexema.Equals("mInt") || lexema.Equals("mFloat") || lexema.Equals("mString") || lexema.Equals("mBool") || lexema.Equals("id"))
                {
                    resposta.Add("DECL");
                    return resposta;
                }
                else if (lexema.Equals("mIf"))
                {
                    resposta.Add("IF");
                    return resposta;
                }
                else if (lexema.Equals("mWhile"))
                {
                    resposta.Add("WHILE");
                    return resposta;
                }
                else if (lexema.Equals("chaveDir"))
                {
                    resposta.Add("ɛ");
                    return resposta;
                }
                else if (lexema.Equals("$"))
                {
                    resposta.Add("ɛ");
                    return resposta;
                }
            }
            else if (producao.Equals("BLOCO'"))
            {
                if (lexema.Equals("mInt") || lexema.Equals("mFloat") || lexema.Equals("mString") || lexema.Equals("mBool") 
                || lexema.Equals("id") || lexema.Equals("mIf") || lexema.Equals("mWhile"))
                {
                    resposta.Add("DECL");
                    return resposta;
                }
                else if (lexema.Equals("chaveDir"))
                {
                    resposta.Add("ɛ");
                    return resposta;
                }
                else if (lexema.Equals("$"))
                {
                    resposta.Add("ɛ");
                    return resposta;
                }
            }
            else if (producao.Equals("DECL"))
            {
                if (lexema.Equals("mInt") || lexema.Equals("mFloat") || lexema.Equals("mString") || lexema.Equals("mBool"))
                {
                    resposta.Add("T");
                    resposta.Add("ATR");
                    return resposta;
                }
                else
                {
                    resposta.Add("T");
                    resposta.Add("I");
                    resposta.Add("pontoVirgula");
                    return resposta;
                }
            }
            else if (producao.Equals("T"))
            {
                if (lexema.Equals("mInt") || lexema.Equals("mFloat") || lexema.Equals("mString") || lexema.Equals("mBool"))
                {
                    resposta.Add("TIPO");
                    return resposta;
                }
            }
            else if (producao.Equals("TIPO"))
            {
                if (lexema.Equals("mInt"))
                {
                    resposta.Add("mInt");
                    return resposta;
                }
                else if (lexema.Equals("mFloat"))
                {
                    resposta.Add("mFloat");
                    return resposta;
                }
                else if (lexema.Equals("mString"))
                {
                    resposta.Add("mString");
                    return resposta;
                }
                else if (lexema.Equals("mBool"))
                {
                    resposta.Add("mBool");
                    return resposta;
                }
            }
            else if (producao.Equals("I"))
            {
                if (lexema.Equals("id"))
                {
                    resposta.Add("id");
                    return resposta;
                }
            }
            else if (producao.Equals("ATR")) //FIXME:
            {
                //if (lexema.Equals("valor"))
                if (true)
                {
                    resposta.Add("A");
                    resposta.Add("valor");
                    resposta.Add("pontoVirgula");
                    return resposta;
                }
                /*else if (lexema.Equals("id"))
                {
                    resposta.Add("A");
                    resposta.Add("id");
                    resposta.Add("pontoVirgula");
                    return resposta;
                }*/
                else
                {
                    resposta.Add("A");
                    resposta.Add("X");
                    resposta.Add("pontoVirgula");
                    return resposta;
                }
            }
            else if (producao.Equals("A"))
            {
                resposta.Add("I");
                resposta.Add("atribuicao");
                return resposta;
            }
            else if (producao.Equals("EXP"))
            {
                if (lexema.Equals("mString") || lexema.Equals("mBool") || lexema.Equals("id") || lexema.Equals("valor")
                || lexema.Equals("null") || lexema.Equals("parenEsq"))
                {
                    resposta.Add("V");
                    resposta.Add("OPERACAO");
                    return resposta;
                }
            }
            else if (producao.Equals("V"))
            {
                if (lexema.Equals("mString") || lexema.Equals("mBool") || lexema.Equals("id") || lexema.Equals("valor")
                || lexema.Equals("null") || lexema.Equals("parenEsq"))
                {
                    resposta.Add("VALOR");
                    return resposta;
                }
            }
            else if (producao.Equals("VALOR"))
            {
                if (lexema.Equals("mString"))
                {
                    resposta.Add("mString");
                    return resposta;
                }
                else if (lexema.Equals("mBool"))
                {
                    resposta.Add("mBool");
                    return resposta;
                }
                else if (lexema.Equals("id"))
                {
                    resposta.Add("id");
                    return resposta;
                }
                else if (lexema.Equals("null"))
                {
                    resposta.Add("null");
                    return resposta;
                }
                else if (lexema.Equals("valor"))
                {
                    resposta.Add("valor");
                    return resposta;
                }
                else if (lexema.Equals("("))
                {
                    resposta.Add("(");
                    return resposta;
                }
            }
            else if (producao.Equals("OPERACAO"))
            {
                if (lexema.Equals("opAlgebrico"))
                {
                    resposta.Add("opAlgebrico");
                    resposta.Add("X");
                    return resposta;
                }
                else if (lexema.Equals("opRel"))
                {
                    resposta.Add("opRel");
                    resposta.Add("X");
                    return resposta;
                }
                else if (lexema.Equals("opLog"))
                {
                    resposta.Add("opLog");
                    resposta.Add("X");
                    return resposta;
                }
            }
            else if (producao.Equals("X"))
            {
                if (lexema.Equals("mBool") || lexema.Equals("id") || lexema.Equals("valor")
                || lexema.Equals("null") || lexema.Equals("parenEsq"))
                {
                    resposta.Add("EXP");
                    return resposta;
                }
            }
            else if (producao.Equals("IF"))
            {
                if (lexema.Equals("mIf"))
                {
                    resposta.Add("mIf");
                    resposta.Add("parenEsq");
                    resposta.Add("X");
                    resposta.Add("parenDir");
                    resposta.Add("chaveEsq");
                    resposta.Add("BLOCO");
                    resposta.Add("chaveDir");
                    resposta.Add("ELSE");
                    return resposta;
                }
            }
            else if (producao.Equals("ELSE"))
            {
                if (lexema.Equals("mElse"))
                {
                    resposta.Add("mElse");
                    resposta.Add("chaveEsq");
                    resposta.Add("BLOCO");
                    resposta.Add("chaveDir");
                    return resposta;
                }
                else
                {
                    resposta.Add("ɛ");
                    return resposta;
                }
            }
            else if (producao.Equals("WHILE"))
            {
                if (lexema.Equals("mWhile"))
                {
                    resposta.Add("mWhile");
                    resposta.Add("parenEsq");
                    resposta.Add("X");
                    resposta.Add("parenDir");
                    resposta.Add("chaveEsq");
                    resposta.Add("BLOCO");
                    resposta.Add("chaveDir");
                    return resposta;
                }
            }
            else // caso nao encontre 
            {
                resposta.Add("erro");
                return resposta;
            }
            resposta.Add("erro");
            return resposta;
        }
    }
}
