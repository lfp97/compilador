using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Scanner
{
    public class AnalisadorSemantico
    {
        static string[] mLexemas;
        public static bool analisador_semantico(string outputPath)
        {
            using (StreamReader sr = new StreamReader(outputPath))
            {
                // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();

                    mLexemas = line.Split("\n\r\n");
                    List<String> listaLexemas= new List<string>(); //é uma lista mas será tratada como pilha

                    String aux = "";
                    Boolean flag_empilhar_id = false; //se identificar uma declaracao de var, seta o flag como true para ele prepara a var aux com "tipo,id" para empilhar

                    foreach (String item in line.Split("\n\r\n"))
                    {
                        if(item.Equals("")) //a última linha do streamReader sempre é nula, parar iteração nele
                            break;
                        //Console.WriteLine(item);
                        if (flag_empilhar_id) //so vai entrar aqui caso o ultimo par de lexema lido tenha sido uma declaracao
                        {
                            aux+=','; //adiciona virgula, ou seja aux = "mTIPO,"
                            aux += item.Substring(item.IndexOf(',') + 1); //adiciona o tipo, aux = "mTIPO,ID"
                            flag_empilhar_id = false; //reseta o flag para poder repetir a logica para caso haja mais declaracoes
                            listaLexemas.Add(aux);//empilha
                        }
                        if (item.Contains("mInt") || item.Contains("mFloat") || item.Contains("mString")) //se for uma declaracao
                        {
                            aux = item.Substring(item.IndexOf(',') + 1); //pega o tipo que esta sendo declarado pra var
                            flag_empilhar_id = true; //seta o flag pra entrar no if acima na proxima iteracao (onde tem o nome da var que acabou de ter seu tipo declaradp)
                        }
                    }
                    foreach (String item in listaLexemas) //debug basico mostando o que empilhou
                    {
                        Console.WriteLine("pilha SEMANTICO: " + item);
                    }
                    Console.WriteLine("último = " + listaLexemas.Last()); //exibe o ultimo
                return true;
            }
        }
    }
}