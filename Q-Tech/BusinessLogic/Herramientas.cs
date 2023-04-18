using AccesData;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Herramientas
    {
        //https://qtechapi.azurewebsites.net/autoterra/v1/

        public static async Task<List<Usuario>> GetUsuarios()
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>("https://qtechapi.azurewebsites.net/autoterra/v1/usuarios");
            return usuarios;
        }

        public static async Task<Usuario> GetUsuario(int id)
        {
            QConsumer qc = new QConsumer();
            Usuario usuario = await qc.GetAsync<Usuario>($"https://qtechapi.azurewebsites.net/autoterra/v1/usuarios/{id}");
            return usuario;
        }

        public static async Task<List<Terrario>> GetTerrarios(long id)
        {
            QConsumer qc = new QConsumer();
            List<Terrario> terrarios = await qc.GetAsync<List<Terrario>>("https://qtechapi.azurewebsites.net/autoterra/v1/terrarios");
            return terrarios.Where( t => t.Idusuario == id ).ToList();
        }

        public static async Task<List<Especie>> GetEspecies()
        {
            QConsumer qc = new QConsumer();
            List<Especie> especies = await qc.GetAsync<List<Especie>>("https://qtechapi.azurewebsites.net/autoterra/v1/especies");
            return especies;
        }

        public static async Task<Especie> GetEspecie(long id)
        {
            QConsumer qc = new QConsumer();
            Especie especie = await qc.GetAsync<Especie>($"https://qtechapi.azurewebsites.net/autoterra/v1/especies/{id}");
            return especie;
        }

        public static async Task<List<EspecieTerrario>> GetEspeciesTerrario(long idTerrario)
        {
            QConsumer qc = new QConsumer();
            List<EspecieTerrario> especiesTerrario = await qc.GetAsync<List<EspecieTerrario>>("https://qtechapi.azurewebsites.net/autoterra/v1/especie-terrario");
            return especiesTerrario.Where(x => x.Idterrario == idTerrario).ToList();
        }

        public static async Task<List<Visita>> GetVisitas(long idTerrario)
        {
            QConsumer qc = new QConsumer();
            List<Visita> visitas = await qc.GetAsync<List<Visita>>("https://qtechapi.azurewebsites.net/autoterra/v1/visitas");
            return visitas.Where(x => x.Idterrario == idTerrario).ToList();
        }

        public static async Task<Usuario> Login(string param, string password)
        {
            QConsumer qc = new QConsumer();
            Usuario usuario = await qc.PostAsync<Usuario>("https://qtechapi.azurewebsites.net/autoterra/v1/login", param, password);
            return usuario;
        }

        public static async void CreateUsuario(Usuario usuario)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync("https://qtechapi.azurewebsites.net/autoterra/v1/usuarios", usuario);
        }
    }
}
