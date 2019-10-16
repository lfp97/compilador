using System;
using System.IO;

namespace Scanner
{
    public class AnalisadorSintatico
    {
        static string[] mLexemas;
        static int i;
        static Fluxo mFluxo = Fluxo.undef;

        enum Fluxo
        {
            undef = -1,
            Declaracao = 0,
            Atribuicao = 1,
            ExpBool = 2,
            ExpAlg = 3,
            If = 4,
            Switch = 5,
            Iteracao = 6,
            Case = 7,
            For = 8,
            While = 9,
            Printf = 10
        }

        public static bool AnalyzeOutput(string outputPath)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(outputPath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    //Console.WriteLine(line);

                    mLexemas = line.Split("\n\r\n");
                }

                for (i = 0; i < mLexemas.Length - 1; i++)
                {
                    //Se checkSyntax retornar falso, indicar onde houve o erro;
                    if (!checkSyntax(Fluxo.undef))
                    {
                        Console.WriteLine($"Erro no token {getLexemaValue(i)},{i}\n");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static string getLexemaName(int mIndex)
        {
            return mLexemas[mIndex].Split(',')[0];   
        }

        private static string getLexemaValue(int mIndex)
        {
            return mLexemas[mIndex].Split(',')[1];
        }

        private static bool checkSyntax(Fluxo fluxo)
        {
            switch (fluxo)
            {
                case Fluxo.undef:
                    switch (Enum.Parse(typeof(Program.lexema), getLexemaName(i)))
                    {
                        case Program.lexema.mIf:
                            i++;
                            return checkSyntax(Fluxo.If);
                        case Program.lexema.id:
                            i++;
                            switch (Enum.Parse(typeof(Program.lexema), getLexemaName(i)))
                            {
                                case Program.lexema.opAlgebrico:
                                    i++;
                                    return checkSyntax(Fluxo.ExpAlg);
                                case Program.lexema.atribuicao:
                                    i++;
                                    return checkSyntax(Fluxo.Atribuicao);
                                default:
                                    i++;
                                    return checkSyntax(Fluxo.ExpBool);
                            }
                        case Program.lexema.mInt:
                            i++;
                            return checkSyntax(Fluxo.Declaracao);
                        case Program.lexema.mFloat:
                            i++;
                            return checkSyntax(Fluxo.Declaracao);
                        case Program.lexema.mString:
                            i++;
                            return checkSyntax(Fluxo.Declaracao);
                        case Program.lexema.mFor:
                            i++;
                            return checkSyntax(Fluxo.For);
                        case Program.lexema.mWhile:
                            i++;
                            return checkSyntax(Fluxo.While);
                        default:
                            return false;
                    }
                case Fluxo.Declaracao:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.pontoVirgula)
                        {
                            return true;
                        }
                        else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.atribuicao)
                        {
                            i++;
                            if (checkSyntax(Fluxo.ExpAlg))
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.pontoVirgula)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Atribuicao:
                    if (checkSyntax(Fluxo.ExpAlg) || checkSyntax(Fluxo.ExpBool))
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.pontoVirgula)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case Fluxo.ExpBool:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.opRel)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mString)
                            {
                                i++;
                                return true;
                            }
                        }
                    }
                    else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mString)
                    {
                        i++;
                        return true;
                    }
                    return false;
                case Fluxo.ExpAlg:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.opAlgebrico)
                        {
                            i++;
                            return checkSyntax(Fluxo.ExpAlg);
                        }
                        else
                        {
                            i--;
                            return true;
                        }
                    }
                    return false;
                case Fluxo.If:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenEsq)
                    {
                        i++;
                        checkSyntax(Fluxo.ExpBool);

                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenDir)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveEsq)
                            {
                                i++;
                                checkSyntax(Fluxo.undef);
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveDir)
                                {
                                    i++;
                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mElse)
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveEsq)
                                        {
                                            i++;
                                            checkSyntax(Fluxo.undef);
                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveDir)
                                            {
                                                return true;
                                            }
                                        }
                                        else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mIf)
                                        {
                                            i++;
                                            checkSyntax(Fluxo.If);
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        i--;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Iteracao:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mFor)
                    {
                        return checkSyntax(Fluxo.For);
                    }
                    else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mWhile)
                    {
                        return checkSyntax(Fluxo.While);
                    }
                    else
                    {
                        return false;
                    }
                case Fluxo.For:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenEsq)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mInt || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                        {
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mInt)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) != Program.lexema.id)
                                {
                                    return false;
                                }
                            }

                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.atribuicao)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor)
                                {
                                    i++;
                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.pontoVirgula)
                                    {
                                        i++;
                                        if (checkSyntax(Fluxo.ExpBool))
                                        {
                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.pontoVirgula)
                                            {
                                                i++;
                                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                                                {
                                                    i++;
                                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.atribuicao)
                                                    {
                                                        i++;
                                                        if (checkSyntax(Fluxo.ExpAlg))
                                                        {
                                                            i++;
                                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenDir)
                                                            {
                                                                i++;
                                                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveEsq)
                                                                {
                                                                    i++;
                                                                    checkSyntax(Fluxo.undef);

                                                                    i++;
                                                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveDir)
                                                                    {
                                                                        return true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    break;
                case Fluxo.While:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenEsq)
                    {
                        i++;
                        if (checkSyntax(Fluxo.ExpBool))
                        {
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.parenDir)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveEsq)
                                {
                                    i++;
                                    if (checkSyntax(Fluxo.undef))
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.chaveDir)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
    }
}
