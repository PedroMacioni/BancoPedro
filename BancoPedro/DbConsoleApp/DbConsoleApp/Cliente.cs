using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConsoleApp
{
    public class Cliente
    {
        public long Id { get; set; }

        public string? Nome { get; set; }

        public string? Usuario { get; set; }

        public string? Email { get; set; }

        public string? CPF { get; set; }

        public string? Telefone { get; set; }

        public string? CEP { get; set; }

        public string? Cidade { get; set; }

        public string? Logradouro { get; set; }

        public string? Bairro { get; set; }

        public string? UF { get; set; }

        public string? DDD { get; set; }

        public string? Numero { get; set; }

        public DateTime? Nascimento { get; set; }

        public char? Genero { get; set; }

        public string? Senha { get; set; }

        public DateTime? Inclusao { get; set; }

        public DateTime? Alteracao { get; set; }

        public string? NumeroConta { get; set; }

        public double? Saldo { get; set; }

        public void Depositar(double valor)
        {
            if (valor > 0)
            {
                Saldo += valor;
            }
        }
    }
}
