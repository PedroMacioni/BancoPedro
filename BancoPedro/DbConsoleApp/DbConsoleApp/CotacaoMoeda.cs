using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DbConsoleApp
{
    public class CotacaoMoeda
    {
        public async Task<decimal> ObterValorCotacao(Moeda moeda)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("https://economia.awesomeapi.com.br/last/USD-BRL,EUR-BRL");
                var content = await response.Content.ReadAsStringAsync();
                var cotacaoMoeda = JsonConvert.DeserializeObject<JObject>(content);

                switch(moeda)
                {
                    case Moeda.Dolar:
                        return Convert.ToDecimal(cotacaoMoeda["USDBRL"]["bid"]);

                    case Moeda.Euro:
                        return Convert.ToDecimal(cotacaoMoeda["EURBRL"]["bid"]);

                    default:
                        throw new Exception("Moeda não implementada");
                }
            }
        }
    }

    public enum Moeda
    {
        Dolar = 1,
        Euro = 2,
    }

}
