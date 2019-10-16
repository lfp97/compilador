using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Scanner
{
    public class Program
    {
        public enum lexema
        {
            indef = -1,
            mIf = 0, //if           Expressão Regular em C#: @"\b(if)\b"
            parenEsq = 1, //(       Expressão Regular em C#: @"\b(\()\b"           \( vai procurar o char '(' na expressao, alguns carcteres precisam do '\' antes
            parenDir = 2, //)       Expressão Regular em C#: @"\b(\))\b"
            opRel = 3, // ==|>=|<=|=!| < | >      Expressão Regular em C#: @"\b(==|>=|<=|=!|\<|\>)\b"
            opLog = 4, // ou == v | e == ^        Expressão Regular em C#: @"\b(v|\^)\b"
            chaveEsq = 5, //{       Expressão Regular em C#: @"\b(\{\b"
            chaveDir = 6, //}       Expressão Regular em C#: @"\b(\}\b"
            id = 7, //Qualquer caractere que inicia com letra       Expressão Regular em C#: @"\b((a-z))\b"
            mWhile = 8, // while    Expressão Regular em C#: @"\b(while)\b"
            pontoVirgula = 9, // ;  Expressão Regular em C#: @"\b(;)\b"
            mInt = 10, // int       Expressão Regular em C#: @"\b(int)\b"
            mElse = 11, // else     Expressão Regular em C#: @"\b(else)\b"
            mFloat = 12, // float   Expressão Regular em C#: @"\b(float)\b"
            negacao = 13, // !      Expressão Regular em C#: @"\b(while)\b"
            mFor = 16, // for       Expressão Regular em C#: @"\b(for)\b"
            valor = 17, // numericos    Expressão Regular em C#: @"\b(0-9)\b"
            atribuicao = 18, // =       Expressão Regular em C#: @"\b(=)\b"
            opAlgebrico = 19, // + - * /    Expressão Regular em C#: @"\b(+|-|*|\/)\b"
            mString = 20, // "((a-z)*(0-9)*)*"
        }

        static void Main(string[] args)
        {


            if (args.Length <= 0)
            {
                Console.WriteLine("Os Argumentos são Scanner [Arquivo] \n ex: Scanner file"); //caso nao especifique o arquivo a ser lido
            }
            else
            {
                string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + args[0] + ".luc";
                string outputPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.luc";
                string token = string.Empty;
                try
                {
                    using (StreamReader sr = new StreamReader(filePath)) //sr vai ler o arquivo no caminho definido acima que é onde está o arquivo de entrada
                    {
                        if (new FileInfo(filePath).Length != 0)
                        {
                            using (StreamWriter fw = new StreamWriter(outputPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    lexema lexemas = lexema.indef;
                                    lexemas = CheckChar(sr, ref token); //aqui chama a função que realiza a comparação da leitura com os lexemas

                                    fw.WriteLine($"{lexemas.ToString()},{token}\n"); //como será escrito no arquivo de saída "LEXEMA,TOKEN"
                                }

                                Console.WriteLine("Output criado.");

                            }
                        }
                        else
                        {
                            Console.WriteLine("O Arquivo está vazio.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static lexema CheckChar(StreamReader sr, ref string token)
        {
            lexema lexemas;
            //Caracteres indesejados
            List<char> charLixos = new List<char>();
            charLixos.Add('\n');
            charLixos.Add('\r');
            charLixos.Add('\0');
            charLixos.Add('\f');
            charLixos.Add('\t');

            char ch = (char)sr.Read(); //o caractere da vez
            lexemas = lexema.indef;

            //Evita characteres indesejados como fim de linha e etc
            while (charLixos.Contains(ch) || char.IsWhiteSpace(ch))
            {
                ch = (char)sr.Read();
            }

            token = ch.ToString();
            if (ch == ';')
            {
                lexemas = lexema.pontoVirgula;
            }
            else if (ch == '(')
            {
                lexemas = lexema.parenEsq;
            }
            else if (ch == ')')
            {
                lexemas = lexema.parenDir;
            }
            else if (ch == 'v')
            {
                lexemas = lexema.opLog;
            }
            else if (ch == '^')
            {
                lexemas = lexema.opLog;
            }
            else if (ch == '!')
            {
                lexemas = lexema.negacao;
            }
            else if (ch == '{')
            {
                lexemas = lexema.chaveEsq;
            }
            else if (ch == '}')
            {
                lexemas = lexema.chaveDir;
            } //abaixo ele "espia" com o método Peek() qual o próximo caractere a ser lido sem consumí-lo. Ex: ==, como o char dessa iteração é o '=', ele espia se o próximo é '=' ou '>' etc
            else if ((ch == '=' && ((char)sr.Peek() == '!' || (char)sr.Peek() == '=')) || ((ch == '>' || ch == '<') && (char)sr.Peek() == '=') || (ch == '!' && (char)sr.Peek() == '=') || ch == '>' || ch == '<')
            {
                ch = (char)sr.Read();
                token += ch.ToString();
                lexemas = lexema.opRel;
            }
            else if (ch == '=')
            {
                lexemas = lexema.atribuicao;
            }
            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
            {
                lexemas = lexema.opAlgebrico;
            }
            else if (ch == '"')
            {
                do
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                } while (ch != '"');

                lexemas = lexema.mString;
            }
            else if (char.IsLetter(ch))
            {
                while (char.IsLetterOrDigit((char)sr.Peek()))
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                }

                lexemas = FindLex(token); //se reconhece que é uma letra ele vai analisar a palavra digitada nesta função
            }
            else if (char.IsDigit(ch)) //se for um númeor
            {
                lexemas = lexema.valor;

                while (!char.IsWhiteSpace((char)sr.Peek())) //enquanto não for um espaço em branco
                {
                    if (char.IsLetter((char)sr.Peek())) //"espia" o próx char, se for letra o lexema não é válido
                    {
                        lexemas = lexema.indef;
                    }
                    else if (char.IsNumber((char)sr.Peek()) || (char)sr.Peek() == '.' ) //"espia" se o próx. char é um nº ou '.'
                    {
                        // evita executar o break abaixo
                    }
                    else if (!char.IsNumber((char)sr.Peek())) // se não for, o break tira do loop
                    {
                        break;
                    }

                    ch = (char)sr.Read();
                    token += ch.ToString(); //acumula o valor para retornar o nº completo no arquivo de saída ex: 12.3 ou 12541
                }
            }
            else
            {
                while (char.IsLetterOrDigit((char)sr.Peek()))
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                }
            }

            return lexemas;
        }

        private static lexema FindLex(string token)
        {
            switch (token)
            {
                case "while":
                    return lexema.mWhile;
                case "if":
                    return lexema.mIf;
                case "int":
                    return lexema.mInt;
                case "float":
                    return lexema.mFloat;
                case "else":
                    return lexema.mElse;
                case "for":
                    return lexema.mFor;
                default:
                    return lexema.id;
            }

        }
    }
}