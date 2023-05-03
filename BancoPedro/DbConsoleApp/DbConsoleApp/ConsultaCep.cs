using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbConsoleApp
{
    public class ConsultaCep
    {
        private readonly HttpClient _httpClient;

        public ConsultaCep(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Endereco> ObterEnderecoPorCep(string cep)
        {
            var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var endereco = JsonConvert.DeserializeObject<Endereco>(content);
                return endereco;
            }

            return null;
        }
    }
}