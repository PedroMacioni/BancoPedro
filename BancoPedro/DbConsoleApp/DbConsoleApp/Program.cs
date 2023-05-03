using DbConsoleApp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;

namespace DbConsoleApp;

public class Program
{

    private static string GerarNumeroContaAleatorio()
    {
        Random random = new Random();
        string numeroAleatorio = random.Next(10000, 99999).ToString();
        string letraAleatoria = ((char)random.Next('A', 'Z')).ToString();
        return letraAleatoria + numeroAleatorio;
    }

    static async Task Main(string[] args)
    {
        var dataContext = new DataContext();

        Console.WriteLine("Bem-vindo ao Banco de Pedro!");
        Console.WriteLine("Escolha uma das opções abaixo:");
        Console.WriteLine(" 1 - Cadastrar cliente");
        Console.WriteLine(" 2 - Entrar na sua conta");
        Console.WriteLine(" 3 - Cotação moedas");

        var opcao = Convert.ToInt32(Console.ReadLine());



        switch (opcao)
        {

            case 1:
                {
                    var novoCliente = new Cliente();

                    Console.WriteLine(" - Digite seu nome Completo:");
                    novoCliente.Nome = Console.ReadLine();

                    Console.WriteLine(" - Digite seu email:");
                    novoCliente.Email = Console.ReadLine();

                    string cpf;
                    bool cpfValido = false;

                    while (!cpfValido)
                    {
                        Console.WriteLine(" - Digite seu CPF:");
                        cpf = Console.ReadLine();

                        if (dataContext.CPFJaCadastrado(cpf))
                        {
                            Console.WriteLine("CPF já cadastrado. Digite novamente.");
                        }
                        else
                        {
                            novoCliente.CPF = cpf;
                            cpfValido = true;
                        }
                    }

                    var viaCepService = new ConsultaCep(new HttpClient());

                    while (true)
                    {
                        Console.WriteLine(" - Digite o seu CEP:");
                        var cep = Console.ReadLine();

                        var endereco = await viaCepService.ObterEnderecoPorCep(cep);

                        if (endereco != null)
                        {
                            Console.WriteLine(" - Endereço encontrado:");
                            Console.WriteLine($"CEP: {endereco.Cep}");
                            Console.WriteLine($"Localidade: {endereco.Localidade}");
                            Console.WriteLine($"UF: {endereco.Uf}");
                            Console.WriteLine($"DDD: {endereco.Ddd}");

                            Console.Write(" - O endereço está correto? (S/N): ");
                            var resposta = Console.ReadLine().ToUpper();

                            if (resposta == "S")
                            {
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($" - CEP inválido ou não encontrado: {cep}");
                            continue;
                        }
                    }

                    Console.WriteLine(" - Continuando com as próximas perguntas...");

                    Console.WriteLine(" - Numero:");
                    novoCliente.Numero = Console.ReadLine();

                    Console.WriteLine(" - Digite seu Telefone: (XX) XXXX-XXXX");
                    novoCliente.Telefone = Console.ReadLine();

                    Console.WriteLine(" - Senha:");
                    novoCliente.Senha = Console.ReadLine();

                    ValidacaoDataNascimento validacao = new ValidacaoDataNascimento();
                    DateTime dataNascimento = validacao.ObterDataNascimentoValida();
                    novoCliente.Nascimento = dataNascimento;

                    Console.WriteLine(" - Digite o seu Genero: - (M/F)");
                    novoCliente.Genero = Convert.ToChar(Console.ReadLine());

                    novoCliente.Inclusao = DateTime.Now;

                    novoCliente.NumeroConta = GerarNumeroContaAleatorio();

                    dataContext.CadastrarCliente(novoCliente);

                    break;
                }

            case 2:
                {
                    Console.WriteLine(" - Digite o seu Numero de Conta:");
                    var numeroConta = Console.ReadLine();

                    Console.WriteLine(" - Digite a sua senha:");
                    var senha = Console.ReadLine();

                    using (var context = new DataContext())
                    {
                        var cliente = dataContext.GetClienteByNumeroConta(numeroConta);

                        if (cliente != null && cliente.Senha == senha)
                        {
                            Console.WriteLine($"Bem-vindo(a), {cliente.Nome}!");
                            Console.WriteLine($"Seu saldo é de: {cliente.Saldo:C}");

                            do // Abre loop do-while aqui
                            {
                                Console.WriteLine("Escolha uma das opções abaixo:");
                                Console.WriteLine(" 1 - Sacar");
                                Console.WriteLine(" 2 - Depositar");
                                Console.WriteLine(" 3 - Transferência");
                                Console.WriteLine(" 4 - Extrato Bancario");


                                var opcaoCliente = Convert.ToInt32(Console.ReadLine());

                                switch (opcaoCliente)
                                {
                                    case 1:
                                        {
                                            while (true)
                                            {
                                                Console.WriteLine("Digite o valor do saque:");
                                                var ValorSacado = Convert.ToDouble(Console.ReadLine());

                                                if (ValorSacado > cliente.Saldo)
                                                {
                                                    Console.WriteLine("Saldo Insuficiente, tente novamente");
                                                    continue;
                                                }

                                                dataContext.Sacar(numeroConta, ValorSacado);
                                                Console.WriteLine($"Novo saldo: {cliente.Saldo:C}");
                                                break;
                                            }
                                            break;
                                        }

                                    case 2:
                                        {
                                            while (true)
                                            {
                                                Console.WriteLine("Digite o valor a ser depositado:");
                                                var ValorDepositado = Convert.ToDouble(Console.ReadLine());

                                                if (ValorDepositado <= 0)
                                                {
                                                    Console.WriteLine("Operação invalida, não se pode inserir um valor negativo.");
                                                    continue;
                                                }

                                                dataContext.Depositar(numeroConta, ValorDepositado);
                                                Console.WriteLine($"Novo saldo: {cliente.Saldo + ValorDepositado:C}");
                                                break; // sai do loop while interno quando a operação é concluída
                                            }
                                            break;
                                        }

                                    case 3:
                                        {
                                            while (true)
                                            {
                                                Console.WriteLine("Digite o número da conta de destino:");
                                                string numeroContaDestino = Console.ReadLine();

                                                Console.WriteLine("Digite o valor da transferência:");
                                                double valor = Convert.ToDouble(Console.ReadLine());

                                                try
                                                {
                                                    dataContext.Transferir(numeroConta, numeroContaDestino, valor);
                                                    Console.WriteLine("Transferência realizada com sucesso!");
                                                    break;
                                                }
                                                catch (SqlException ex)
                                                {
                                                    Console.WriteLine("Erro ao realizar transferência: " + ex.Message);
                                                }
                                            }
                                            break;
                                        }

                                    case 4:
                                        {
                                            Console.WriteLine("Deseja gerar o seu extrato bancário? (S/N)");
                                            var Resposta = Console.ReadLine().ToUpper();

                                            if (Resposta == "S")
                                            {
                                                dataContext.GerarExtratoConta(cliente.NumeroConta);
                                                Console.WriteLine("Extrato gerado com sucesso!");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Operação cancelada.");
                                            }
                                            break;
                                        }

                                    default:
                                        Console.WriteLine("Opção inválida!");
                                        break;
                                }

                                Console.Write("Deseja fazer outra transação? (S/N) ");
                                var resposta = Console.ReadLine().ToUpper();
                                if (resposta != "S") // Sai do loop do-while se a resposta for "não"
                                {
                                    return;
                                }

                            } while (true);
                        }
                        else
                        {
                            Console.WriteLine("Dados incorretos. Tente novamente.");
                        }
                        break;
                    }
                }
            case 3:
                {
                    var cotacaoMoeda = new CotacaoMoeda();
                    var cotacaoDolar = await cotacaoMoeda.ObterValorCotacao(Moeda.Dolar);
                    var cotacaoEuro = await cotacaoMoeda.ObterValorCotacao(Moeda.Euro);

                    Console.WriteLine("Taxas de câmbio:");
                    Console.WriteLine("USD/BRL: {0:C2}", cotacaoDolar);
                    Console.WriteLine("EUR/BRL: {0:C2}", cotacaoEuro);
                    break;
                }
        }
    }
}






