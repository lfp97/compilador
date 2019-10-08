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
            mIf = 0, //if
            parenEsq = 1, //(
            parenDir = 2, //) 
            opRel = 3, // ==|>=|<=|=!| < | >
            chaveEsq = 5, //{
            chaveDir = 6, //}
            id = 7, //Qualquer caractere que inicia com letra
            pontoVirgula = 9, // ;
            mInt = 10, // int
            mElse = 11, // else
            valor = 17, // numericos
            atribuicao = 18, // =
            opAlgebrico = 19, // + - * /
            mString = 20, // "((a-z)*(0-9)*)*"
        }

        static void Main(string[] args)
        {


            if (args.Length <= 0)
            {
                Console.WriteLine("Os Argumentos são Scanner [Arquivo] \n ex: Scanner file");
            }
            else
            {
                string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + args[0] + ".luc";
                string outputPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.luc";
                string token = string.Empty;
                try
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        if (new FileInfo(filePath).Length != 0)
                        {
                            using (StreamWriter fw = new StreamWriter(outputPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    lexema lexemas = lexema.indef;
                                    lexema aux = lexema.chaveDir;
                                    //Console.WriteLine(aux);
                                    lexemas = CheckChar(sr, ref token);

                                    fw.WriteLine($"{lexemas.ToString()},{token}\n");
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

            char ch = (char)sr.Read();
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
            else if (ch == '{')
            {
                lexemas = lexema.chaveEsq;
            }
            else if (ch == '}')
            {
                lexemas = lexema.chaveDir;
            }
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

                lexemas = FindLex(token);
            }
            else if (char.IsDigit(ch))
            {
                lexemas = lexema.valor;

                while (!char.IsWhiteSpace((char)sr.Peek()))
                {
                    if (char.IsLetter((char)sr.Peek()))
                    {
                        lexemas = lexema.indef;
                    }
                    else if (!char.IsNumber((char)sr.Peek()))
                    {
                        break;
                    }

                    ch = (char)sr.Read();
                    token += ch.ToString();
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
                case "if":
                    return lexema.mIf;
                case "int":
                    return lexema.mInt;
                case "else":
                    return lexema.mElse;
                default:
                    return lexema.id;
            }

        }
    }
}