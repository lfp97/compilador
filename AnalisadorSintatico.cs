using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scanner
{
    public class AnalisadorSintatico
    {
        static string[] mLexemas;
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
                    List<String> listaLexemas= new List<string>();
                    foreach (String item in line.Split("\n\r\n"))
                    {
                        if(item.Equals("")) //a última linha do streamReader sempre é nula, parar iteração nele
                         break;
                        listaLexemas.Add(item.Substring(0, item.IndexOf(','))); //pega somente o lexema do token, ex.: mInt, int    ... pega somente o mInt
                        //Console.WriteLine("adicionando em listaLexemas: " + item.Substring(0, item.IndexOf(',')));
                    }
                    listaLexemas.Add("$"); //adicionando o char de EOF
                    String lexemas = String.Join(",", listaLexemas); //transforma os lexemas numa string separados por ','   ex.: mInt,id,atribuicao
                    Console.WriteLine(lexemas);
                    try
                    {
                        string outputPath3 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\outputSintatico.luc";

                        using (StreamWriter fw = new StreamWriter(outputPath3))
                        {
                            if (lexemas.Equals("chaveEsq,chaveDir,$")) //codigo com corpo vazio: { }
                                Console.WriteLine("Análise sintática completa, não foram encontrados erros");
                            else
                            {
                                List<String> pilhaLexemas= new List<string>();
                                String[] lexema = lexemas.Split(','); //prepara a string com a sequencia de lexemas para ser empilhada e a coloca em um array
                                Array.Reverse(lexema); //inverter a ordem do Array para empilhar corretamente
                                foreach (String item in lexema) //empilhando
                                {
                                    pilhaLexemas.Add(item);
                                }
                                List<String> pilhaGramatica= new List<string>(); //a variavel em si é uma lista, porém será tratada como pilha
                                pilhaGramatica.Add("BLOCO"); //primeiro item da gramatica
                                List<String> pilhaResultado= new List<string>();
                                String lexemaLido = null, variavelLida= null;
                                while (pilhaGramatica.Count > 0) //enquanto tiver variaveis na pilha gramatica
                                {
                                    List<String> result;
                                    variavelLida = pilhaGramatica.Last(); //pega a ultima variavel da gramatica para poder analisar com o lexema lido
                                    Console.WriteLine("variavelLida = " + variavelLida);
                                    pilhaGramatica.RemoveAt(pilhaGramatica.Count-1); //remove efetivamente da pilha, a funcao last() equivale a funcao peek(), pega da pilha(no nosso caso, a lista) sem remover
                                    Console.WriteLine("lexemaLido = " + lexemaLido);
                                    Console.WriteLine("if ((lexemaLido == null))   -> " + (lexemaLido == null));
                                    if ((lexemaLido == null)) //caso nao tenha lexema para analisar, passa para o proximo da lista, assume que o mesmo foi utilizado para liberar a pilha
                                    {
                                        lexemaLido = pilhaLexemas.Last();
                                        pilhaLexemas.RemoveAt(pilhaLexemas.Count-1);
                                        Console.WriteLine("novo lexemaLido = " + lexemaLido);
                                    }
                                    Console.WriteLine("// variavelLida = " + variavelLida + " // lexemaLido = " + lexemaLido);
                                    Console.WriteLine("if (variavelLida == lexemaLido)   -> " + (variavelLida == lexemaLido));
                                    if (variavelLida == lexemaLido) //famoso match da analise top down
                                    {
                                        Console.WriteLine("MATCH!");
                                        lexemaLido = null;
                                        //ja que deu match:
                                        //nao precisa chamar a parser table (entao nao executa o else abaixo), e volta ao flux de:
                                        //pegar o prox elemento da gramatica e o novo lexema para testar tudo novamente
                                    }
                                    else //caso nao dê match, é preciso analisar a parserTable novamente, enviando também o lexema lido para ela decidir a producao a ser retornada
                                    {
                                        //Console.WriteLine("// variavelLida = " + variavelLida + " // lexemaLido = " + lexemaLido);
                                        Console.WriteLine("chamando 'parserTable.cs' com variavelLida = " + variavelLida + " // lexemaLido = " + lexemaLido);
                                        result = ParserTable.buscar(variavelLida, lexemaLido);

                                        pilhaResultado.Add(variavelLida + " -> ");
                                        fw.WriteLine($"{variavelLida} -> ");
                                        foreach (String item in result)
                                        {
                                            Console.WriteLine("retorno da funcao: " + item + "\n");
                                            pilhaResultado.Add(item + " ");
                                            fw.WriteLine($"{item} ");
                                        }
                                        fw.WriteLine($"\n");
                                        pilhaResultado.Add("@");
                                        foreach (String item in pilhaResultado)
                                        {
                                            Console.WriteLine("pilhaResultado: " + item);
                                        }
                                        

                                        if (result[0].Equals("ɛ")) //uma pseudo condição de parada
                                        {}
                                        else
                                        {
                                            result.Reverse(); //preparar para empilhar os resultados da parserTable
                                            foreach (String item in result)
                                            {
                                                Console.WriteLine("pilhaGramatica.Add(" + item + ");");
                                                pilhaGramatica.Add(item);
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine("-------------------------------");
                                Console.WriteLine("pilhaGramatica vazia! \n Resultados:");
                                foreach (String item in pilhaLexemas)
                                {
                                    Console.WriteLine("pilhaLexemas: " + item);
                                }
                                //Console.WriteLine("pilhaLexemas[1] == " + pilhaLexemas[1]);
                                //Console.WriteLine(pilhaLexemas.Count());
                                if (pilhaLexemas.Count() == 1)
                                {
                                    Console.WriteLine("Análise Sintática: OK");
                                    return true;
                                }
                                else
                                {
                                    Console.WriteLine("Análise Sintática: ERRO");
                                    return false;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }//fim
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return (true);
        }
    }
}
