using AccesData;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Herramientas
    {
        //https://qtechapi.azurewebsites.net/autoterra/v1/

        #region GET
        #region Usuario
        public static async Task<List<Usuario>> GetUsuarios() //optimizado
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>("https://qtechapi.azurewebsites.net/autoterra/v1/usuarios");
            return usuarios;
        }
        public static async Task<List<Usuario>> GetSocial(long id) //optimizado
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>($"https://qtechapi.azurewebsites.net/autoterra/v1/usuarios/social/{id}");
            return usuarios;
        }
        public static async Task<bool> ComprobarUsuario(string param)
        {
            QConsumer qc = new QConsumer();
            return await qc.GetAsync<bool>($"https://qtechapi.azurewebsites.net/autoterra/v1/usuarios/check/{param}");
        }
        public static async Task<Usuario> GetUsuario(int id)
        {
            QConsumer qc = new QConsumer();
            Usuario usuario = await qc.GetAsync<Usuario>($"https://qtechapi.azurewebsites.net/autoterra/v1/usuarios/{id}");
            return usuario;
        }
        #endregion
        #region Terrarios
        public static async Task<List<Terrario>> GetTerrarios(long id)
        {
            QConsumer qc = new QConsumer();
            List<Terrario> terrarios = await qc.GetAsync<List<Terrario>>("https://qtechapi.azurewebsites.net/autoterra/v1/terrarios");
            return terrarios.Where(t => t.Idusuario == id).ToList();
        }
        #endregion
        #region Especies
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
        #endregion
        #endregion








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

        public static async Task<List<Usuario>> Search(string query)
        {
            QConsumer qc = new QConsumer();

            List<Usuario> usuarios = await GetUsuarios();

            return usuarios.Where(x => x.NombreUsuario.Contains(query)).ToList();
        }

        public static async Task<List<Logro>> GetLogros(long id)
        {
            QConsumer qc = new QConsumer();
            List<Logro> logros = await qc.GetAsync<List<Logro>>("https://qtechapi.azurewebsites.net/autoterra/v1/logros");
            return logros;
        }
    }
}
