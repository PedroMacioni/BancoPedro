using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbConsoleApp;
using System.Data;

namespace DbConsoleApp
{
    public class DataContext : IDisposable
    {
        private SqlConnection connection;

        public DataContext()
        {
            connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        public static string CONNECTION_STRING = "Data Source=data-host.via.company,8888;Initial Catalog=BancoDePedro;Persist Security Info=True;User ID=PedroMacioni;Password=1406Ch@ng3";

        public SqlConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(CONNECTION_STRING);
            conn.Open();
            return conn;
        }

        public void CadastrarCliente(Cliente novoCliente)
        {
            novoCliente.Inclusao = DateTime.Now;

            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                string commandSql = @"
            INSERT INTO Customers 
                (NumeroConta, Nome, Email, CPF, Telefone, CEP, Numero, Nascimento, Genero, Senha, Saldo, Inclusao)
            VALUES
                (@NumeroConta, @Nome, @Email, @CPF, @Telefone, @CEP, @Numero, @Nascimento, @Genero, @Senha, @Saldo, GETDATE())";
                SqlCommand command = new SqlCommand(commandSql, conn);

                command.Parameters.AddWithValue("@NumeroConta", novoCliente.NumeroConta);
                command.Parameters.AddWithValue("@Nome", novoCliente.Nome);
                command.Parameters.AddWithValue("@Email", novoCliente.Email);
                command.Parameters.AddWithValue("@CPF", novoCliente.CPF);
                command.Parameters.AddWithValue("@Telefone", novoCliente.Telefone);
                command.Parameters.AddWithValue("@CEP", novoCliente.CEP);
                command.Parameters.AddWithValue("@Numero", novoCliente.Numero);
                command.Parameters.AddWithValue("@Nascimento", novoCliente.Nascimento);
                command.Parameters.AddWithValue("@Genero", novoCliente.Genero);
                command.Parameters.AddWithValue("@Senha", novoCliente.Senha);
                command.Parameters.AddWithValue("@Saldo", novoCliente.Saldo);
                command.Parameters.AddWithValue("@Inclusao", novoCliente.Inclusao);

                int linhasAfetadas = command.ExecuteNonQuery();
                if (linhasAfetadas > 0)
                {
                    Console.WriteLine("Seu cadastro foi realizado com sucesso.");
                }
                else
                {
                    Console.WriteLine("Ocorreu um erro ao inserir os dados.");
                }

                conn.Close();
            }
        }

        public bool CPFJaCadastrado(string cpf)
        {
            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                using (var comando = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE CPF = @CPF", conn))
                {
                    comando.Parameters.AddWithValue("@CPF", cpf);

                    int count = (int)comando.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public void GerarExtratoConta(string numeroConta)
        {
            string query = @"SELECT NumeroConta, Nome, Saldo FROM Customers WHERE NumeroConta = @NumeroConta";
            using (SqlConnection conn = CreateConnection())
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@NumeroConta", numeroConta);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        Cliente cliente = new Cliente();
                        DataRow row = dataTable.Rows[0];
                        cliente.NumeroConta = row["NumeroConta"].ToString();
                        cliente.Nome = row["Nome"].ToString();
                        cliente.Saldo = Convert.ToDouble(row["Saldo"]);

                        Document doc = new Document();
                        string path = @"C:\Users\Pedro\Documents\Extratos\" + cliente.NumeroConta + ".pdf";
                        PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));

                        doc.Open();
                        doc.Add(new Paragraph("Extrato bancário"));
                        doc.Add(new Paragraph("Nome: " + cliente.Nome));
                        doc.Add(new Paragraph("Número da conta: " + cliente.NumeroConta));
                        doc.Add(new Paragraph("Saldo atual: " + cliente.Saldo.ToString()));
                        doc.Close();

                        Console.WriteLine("Extrato gerado com sucesso em: " + path);
                    }
                    else
                    {
                        Console.WriteLine("Cliente com número de conta " + numeroConta + " não encontrado.");
                    }
                }
            }
        }

        public Cliente BuscarCliente(string numeroConta)
        {
            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                var commandSql = @"
            SELECT *
            FROM Customers
            WHERE NumeroConta = @NumeroConta";

                using (var command = new SqlCommand(commandSql, conn))
                {
                    command.Parameters.AddWithValue("@NumeroConta", numeroConta);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var cliente = new Cliente();
                            cliente.NumeroConta = reader["NumeroConta"].ToString();
                            cliente.Nome = reader["Nome"].ToString();
                            cliente.Email = reader["Email"].ToString();
                            cliente.CPF = reader["CPF"].ToString();
                            cliente.Telefone = reader["Telefone"].ToString();
                            cliente.CEP = reader["CEP"].ToString();
                            cliente.Nascimento = (DateTime)reader["Nascimento"];
                            cliente.Genero = Convert.ToChar(reader["Genero"]);
                            cliente.Senha = reader["Senha"].ToString();
                            cliente.Saldo = (double)Convert.ToDecimal(reader["Saldo"]);

                            return cliente;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }



        public Cliente GetClienteByNumeroConta(string numeroConta)
        {
            using (SqlConnection conn = CreateConnection())
            {
                string commandSql = @"
            SELECT *
            FROM Customers
            WHERE NumeroConta = @NumeroConta";

                SqlCommand command = new SqlCommand(commandSql, conn);
                command.Parameters.AddWithValue("@NumeroConta", numeroConta);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Cliente cliente = new Cliente();
                        cliente.NumeroConta = reader["NumeroConta"].ToString();
                        cliente.Nome = reader["Nome"].ToString();
                        cliente.Email = reader["Email"].ToString();
                        cliente.CPF = reader["CPF"].ToString();
                        cliente.Telefone = reader["Telefone"].ToString();
                        cliente.CEP = reader["CEP"].ToString();
                        cliente.Nascimento = (DateTime)reader["Nascimento"];
                        cliente.Genero = Convert.ToChar(reader["Genero"]);
                        cliente.Senha = reader["Senha"].ToString();
                        cliente.Saldo = (double)Convert.ToDecimal(reader["Saldo"]);

                        return cliente;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void Depositar(string numeroConta, double valor)
        {
            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                var commandSql = @"
        UPDATE Customers
        SET Saldo = Saldo + @ValorDepositado
        WHERE NumeroConta = @NumeroConta;

        INSERT INTO Transactions (NumeroConta, Tipo, Valor, DataHora)
        VALUES (@NumeroConta, 'DEPOSITO', @ValorDepositado, GETDATE());";

                using (var command = new SqlCommand(commandSql, conn))
                {
                    command.Parameters.AddWithValue("@ValorDepositado", valor);
                    command.Parameters.AddWithValue("@NumeroConta", numeroConta);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Depósito realizado com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("Erro ao realizar depósito.");
                    }
                }
            }
        }


        public bool Sacar(string numeroConta, double valor)
        {
            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                var commandSql = @"
        IF (SELECT Saldo FROM Customers WHERE NumeroConta = @NumeroConta) >= @Valor
        BEGIN
            UPDATE Customers
            SET Saldo = Saldo - @Valor
            WHERE NumeroConta = @NumeroConta;

            INSERT INTO Transactions (NumeroConta, Tipo, Valor, DataHora)
            VALUES (@NumeroConta, 'SAQUE', @Valor, GETDATE());

            SELECT CAST(1 AS BIT);
        END
        ELSE
        BEGIN
            SELECT CAST(0 AS BIT);
        END";

                var command = new SqlCommand(commandSql, conn);
                command.Parameters.AddWithValue("@NumeroConta", numeroConta);
                command.Parameters.AddWithValue("@Valor", valor);

                var result = command.ExecuteScalar();

                return Convert.ToBoolean(result);
            }
        }

        private int ObterIdClientePorNumeroConta(string numeroConta)
        {
            int idCliente = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT Id FROM Customers WHERE NumeroConta = @NumeroConta", connection))
                {
                    command.Parameters.AddWithValue("@NumeroConta", numeroConta);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        int.TryParse(reader["Id"].ToString(), out idCliente);
                    }

                    reader.Close();
                }
            }

            return idCliente;
        }

        private double GetSaldoByNumeroConta(string numeroConta)
        {
            double saldo = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(@"SELECT Saldo FROM Customers WHERE NumeroConta = @NumeroConta", connection))
                {
                    command.Parameters.AddWithValue("@NumeroConta", numeroConta);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        double.TryParse(reader["Saldo"].ToString(), out saldo);
                    }

                    reader.Close();
                }
            }

            return saldo;
        }

        public void Transferir(string numeroContaOrigem, string numeroContaDestino, double valor)
        {

            using (var conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                // Obter o ID da conta de origem a partir do número da conta
                var idClienteContaOrigem = ObterIdClientePorNumeroConta(numeroContaOrigem);

                // Obter o ID da conta de destino a partir do número da conta
                var idClienteContaDestino = ObterIdClientePorNumeroConta(numeroContaDestino);

                // Verificar se a conta de destino existe
                if (idClienteContaDestino == 0)
                {
                    Console.WriteLine("Conta de destino não encontrada.");
                    return;
                }

                // Verificar se o saldo da conta de origem é suficiente
                double saldoContaOrigem = GetSaldoByNumeroConta(numeroContaOrigem);

                if (valor > saldoContaOrigem)
                {
                    Console.WriteLine("Saldo insuficiente.");
                    return;
                }

                // Executar a transferência
                using (var command = new SqlCommand("Transferir", conn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdContaOrigem", idClienteContaOrigem);
                    command.Parameters.AddWithValue("@IdContaDestino", idClienteContaDestino);
                    command.Parameters.AddWithValue("@Valor", valor);

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Transferência realizada com sucesso!");
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Erro ao realizar transferência: " + ex.Message);
                    }
                }
            }
        }
    }
}