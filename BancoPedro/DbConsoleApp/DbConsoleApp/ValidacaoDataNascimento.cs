using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConsoleApp
{
    public class ValidacaoDataNascimento
    {
        public DateTime ObterDataNascimentoValida()
        {
            DateTime dataNascimento;

            do
            {
                Console.WriteLine(" - Digite o seu ano de Nascimento - (YYYY-MM-DD):");
            } while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento));

            return dataNascimento;
        }
    }
}
