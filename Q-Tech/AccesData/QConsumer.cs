using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AccesData
{
    public class QConsumer
    {
        private HttpClient _httpClient;

        public QConsumer()
        {
            _httpClient = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(content);
                return result;
            }
            else
            {
                throw new ApplicationException($"Error al obtener el recurso: {response.StatusCode}");
            }
        }
        public async Task<T> PostAsync<T>(string url, string user, string contra)
        {
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "param", user },
                { "password", contra }
            };

            string json = JsonConvert.SerializeObject(data);
            HttpClient client = new HttpClient();
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                   HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                T responseObject = JsonConvert.DeserializeObject<T>(responseJson);
                return responseObject;
            }
            else
            {
                return default;
            }
        }

        //public async Task<T> PostAsync<T>(string url, string user, string contra)
        //{
        //    Dictionary<string, string> data = new Dictionary<string, string>
        //    {
        //        { "param", user },
        //        { "password", contra }
        //    };

        //    try
        //    {
        //        string json = JsonConvert.SerializeObject(data);
        //        HttpClient client = new HttpClient();
        //        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response = await client.PostAsync(url, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseJson = await response.Content.ReadAsStringAsync();
        //            T responseObject = JsonConvert.DeserializeObject<T>(responseJson);
        //            return responseObject;
        //        }
        //        else
        //        {
        //            return default;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejar la excepción aquí, por ejemplo, imprimir el mensaje de error en la consola.
        //        Console.WriteLine(ex.Message);
        //        return default;
        //    }
        //}


        public async Task<T> CreateAsync<T>(string url, T data)
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();

            return default;
        }
        public async Task<T> UpdateAsync<T>(string url, T data)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            T genericObject = await response.Content.ReadAsAsync<T>();
            return genericObject;
        }
        public async Task<T> DeleteAsync<T>(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.DeleteAsync(url);
            return default;
        }
    }
}
