using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbConsoleApp
{
    public class Transferencia
    {
        public int IdTransferencia { get; set; }

        public int IdContaOrigem { get; set; }

        public int IdContaDestino { get; set; }

        public decimal Valor { get; set; }

        public DateTime DataTransferencia { get; set; }
    }
}
