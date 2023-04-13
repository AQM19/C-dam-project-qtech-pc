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
    }
}
