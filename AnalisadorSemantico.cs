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
                    List<String> pilhaVars= new List<string>(); //é uma lista mas será tratada como pilha

                    String aux = "";
                    Boolean flag_empilhar_id = false; //se identificar uma declaracao de var, seta o flag como true para ele prepara a var aux com "tipo,id" para empilhar
                    Boolean flag_presenca_if = false;
                    List<String> listaCI= new List<string>(); // vai ser convertida dps em CI após a inferência de tipo de o teste de curto-circuito

                    
                    foreach (String item in line.Split("\n\r\n"))
                    { //TODO: percorre a sequência de lexemas p/ criar a pilha de tipo das vars declaradas

                        if(item.Equals("")) //a última linha do streamReader sempre é nula, parar iteração nele
                            break;
                        if (item.Contains("mIf")) //TODO: se achar um if, sinaliza para realizar a análise de curto circuito
                            flag_presenca_if = true;
                        if (flag_empilhar_id) //so vai entrar aqui caso o ultimo par de lexema lido tenha sido uma declaracao
                        {
                            aux+=','; //adiciona virgula, ou seja aux = "mTIPO,"
                            aux += item.Substring(item.IndexOf(',') + 1); //adiciona o tipo, aux = "mTIPO,ID"
                            flag_empilhar_id = false; //reseta o flag para poder repetir a logica para caso haja mais declaracoes
                            pilhaVars.Add(aux);//empilha
                        }
                                //TODO: se encontra uma declaracao armazena o tipo e na próx. iteração guarda o id da var (acima)
                        if (item.Contains("mInt") || item.Contains("mFloat") || item.Contains("mString") || item.Contains("mBool")) //se for uma declaracao
                        {
                            aux = item.Substring(item.IndexOf(',') + 1); //pega o tipo que esta sendo declarado pra var
                            flag_empilhar_id = true; //seta o flag pra entrar no if acima na proxima iteracao (onde tem o nome da var que acabou de ter seu tipo declaradp)
                        }
                        
                    }
                    foreach (String item in pilhaVars) //debug basico mostando o que empilhou
                    {
                        Console.WriteLine("pilha : " + item);
                    }
                    

                    for (int i = 0; i < pilhaVars.Count; i++)
                    {
                        pilhaVars[i] = pilhaVars[i].Substring(0, pilhaVars[i].Length); //limpa a sujeira
                    }
                    //Console.WriteLine("último = " + pilhaVars.Last()); //exibe o ultimo




                    List<String> outputLexico= new List<string>(); //é uma lista mas será tratada como pilha
                    foreach (String item in line.Split("\n\r\n")) //TODO: percorrendo mais uma vez a lista de lexemas 
                    {
                        outputLexico.Add(item);
                    }

                    for (int i = 0; i < outputLexico.Count; i++) //TODO: agora p/ testar a inferência correta de tipo em atribuições
                    {// int x =
                        
                        if (outputLexico[i].Contains("atribuicao")) //TODO: se achar um 'atribuicao' 
                        {
                            if (outputLexico[i-2].Contains("mInt") || outputLexico[i-2].Contains("mFloat") || outputLexico[i-2].Contains("mBool"))
                            {
                                //Console.WriteLine("DECLARACAO"); //TODO: olha se o valor anterior era um tipo, ou seja, se está lendo uma declaração
                            }
                            else //TODO: se nao for uma declaracao tem que inferir o tipo
                            {
                                //Console.WriteLine("ATRIBUICAO mesmo em");
                                //Console.WriteLine("outputLexico[i-1] == " + outputLexico[i-1]);
                                //procurar outputLexico[i-1] na pilha de vars
                                String var_a_procurar = outputLexico[i-1].Substring(outputLexico[i-1].IndexOf(',')+1);
                                String tipo_var_a_procurar = "";
                                bool flag_averiguacao_tipo = false;
                                Console.WriteLine("id a procurar na pilha == " + var_a_procurar);
                                for (int j = 0; j < pilhaVars.Count; j++) //TODO: percorre a pilha de variaveis+tipo preenchida anteriormente
                                {
                                    String var_da_vez = pilhaVars[j].Substring(pilhaVars[j].IndexOf(',')+1);
                                    //Console.WriteLine("var sendo procurada na pilha == " + var_da_vez);
                                    if (var_da_vez.Contains(var_a_procurar))
                                    {
                                        //TODO: guarda o tipo da var que está sendo analisada
                                        tipo_var_a_procurar = pilhaVars[j].Substring(0, pilhaVars[j].IndexOf(','));
                                    }
                                    //var_da_vez = var_da_vez.Substring
                                }
                                Console.WriteLine("tipo da var q esta sendo atribuida == " + tipo_var_a_procurar);
                                if (tipo_var_a_procurar.Contains("bool")) //TODO: se a var sendo testada é bool
                                {
                                    if (outputLexico[i+1].Contains("mTrue") || outputLexico[i+1].Contains("mFalse")) //TODO: ela está recebendo um bool?
                                    {
                                        flag_averiguacao_tipo = true;
                                    }
                                }
                                if (tipo_var_a_procurar.Contains("valor")) //TODO: se o tipo da var é numérica
                                {
                                    if (outputLexico[i+1].Contains("valor")) //TODO: espia o tipo do prox lexema apos o '='
                                    {
                                        //não precisa testar p/ float, já que o conjunto de valores int está contido em float
                                        if (tipo_var_a_procurar.Contains("mInt"))
                                        {
                                            String valor = outputLexico[i-1].Substring(outputLexico[i-1].IndexOf(',')+1);
                                            int ix;
                                            if (int.TryParse(valor, out ix))
                                            {
                                                flag_averiguacao_tipo = true;
                                            }
                                        }
                                    }
                                }
                                if (!flag_averiguacao_tipo)
                                {
                                    Console.WriteLine("ERRO DE ATRIBUICAO!");
                                }
                            }
                        }
                    }

                    
                    /*{
                        String valor = "";
                        Console.WriteLine("item == " + item);
                        Console.WriteLine("item.length == " + item.Length);
                        valor = item.Substring(0, item.Length-1);
                        Console.WriteLine("valor == " + valor);
                        Console.WriteLine("valor.length == " + valor.Length);
                        
                    }*/

                    
                    //CURTO-CIRCUITO
                    if (flag_presenca_if) //TODO: se tem if, testa se pode aplicar curto
                    {
                        List<String> listaExp= new List<string>(); //é uma lista mas será tratada como pilha
                        List<String> listaExpSimbolo= new List<string>(); //é uma lista mas será tratada como pilha
                        
                        String exp = ""; //esta string vai ter por exemplo: mIf, parenEsq, id, opRel, id, parenDir
                        String exp_simbolo = ""; //já essa: if ( a == b )
                        Boolean flag_exp = false;
                        foreach (String item in line.Split("\n\r\n"))
                        { //itera sobre o arquivo dos lexemas
                            if (item.Contains("mIf")) //TODO: se 'mIf' prepara p/ começar a captura da expressão a ser analisada para o curto-circuito
                                flag_exp = true; //if (..) {}
                            if(item.Contains("chaveEsq")) //TODO: condicao de parada para esta captura é o fim da expressão lógica if, ou seja, um '{'
                            {
                                flag_exp = false; //if (..) $ if(..)
                                exp+= "$"; //flag para diferenciar as expressoes de if's caso tenha mais de uma no código
                                listaExp.Add("$");
                                listaExpSimbolo.Add("$");
                                exp_simbolo+= "$";
                            }
                            if(flag_exp) //bloco que captura  o conteudo da expressao
                            {
                                exp += item.Substring(0, item.IndexOf(',')) + " "; //pegamos somente o lexema
                                listaExp.Add(item.Substring(0, item.IndexOf(',')) + " ");
                                listaExpSimbolo.Add(item.Substring(item.IndexOf(',') + 1) + " ");
                                exp_simbolo += item.Substring(item.IndexOf(',') + 1) + " "; //pegamos somente o valor p/ facilitar o entendimento
                            }
                        }

                        
                        //listaExp.RemoveAt(0); //remove o 'mIf'
                        //listaExp.RemoveAt(0); //remove o 'parenEsq'
                        //listaExpSimbolo.RemoveAt(0);
                        //listaExpSimbolo.RemoveAt(0);
                        
                    
                        

                        Console.WriteLine("Expressao if a ser analisada: " + exp);
                        Console.WriteLine("Em simbolos: " + exp_simbolo);
                        
                        //foreach (String item in listaExp)
                        for (int i = 0; i < listaExp.Count; i++)
                        {
                            listaExp[i] = listaExp[i].Substring(0, listaExp[i].Length-1); //TODO: limpa o caractere lixo no fim de cada lexema
                        }

                        bool flag_aplicar_curto = false;
                        String exp_encurtada = "";
                        for (int i = 0; i < listaExp.Count; i++)
                        {   
                            if (listaExp[i].Contains("mFalse") || listaExp[i].Contains("mTrue")) //TODO: se acha um valor booleano na expressão
                            {
                                if (listaExp[i+1].Contains("opLog")) //TODO: seguido de um operador lógico
                                {
                                    flag_aplicar_curto = true; //TODO: temos um curto! if (true)
                                    if (listaExp[i].Contains("mFalse")) //if (true)
                                            exp_encurtada = "mIf parenEsq mFalse parenDir"; //TODO: nova expressão if que seria inserida no CI
                                        else
                                            exp_encurtada = "mIf parenEsq mTrue parenDir";
                                }
                            }
                            else
                            {
                                if (listaExp[i].Contains("opLog")) //TODO: mesma lógica de cima porém começando de um operador lógico
                                { //if (... v true)
                                    if (listaExp[i+1].Contains("mFalse") || listaExp[i+1].Contains("mTrue")) //TODO: e testando se o próximo é bool
                                    {//TODO: ex if (... v true) nao importa o que tinha no '...'
                                        flag_aplicar_curto = true;
                                        if (listaExp[i+1].Contains("mFalse"))
                                            exp_encurtada = "mIf parenEsq mFalse parenDir";
                                        else
                                            exp_encurtada = "mIf parenEsq mTrue parenDir";
                                    }
                                }
                                
                            }  
                        }
                        if (flag_aplicar_curto)
                        {
                            Console.WriteLine("A expressão if sofre curto!");
                        }
                    }
                return true;
            }
        }
    }
}