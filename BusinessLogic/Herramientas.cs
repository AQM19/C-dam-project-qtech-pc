using AccesData;
using Entities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class Herramientas
    {
        readonly static string baseEndPoint = ConfigurationManager.AppSettings["urlApiLocal"].ToString();


        #region Usuario
        public static async Task<List<Usuario>> GetUsuarios()
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>($"{baseEndPoint}/usuarios");
            return usuarios;
        }
        public static async Task<List<Usuario>> GetSocial(long id)
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>($"{baseEndPoint}/usuarios/social/{id}");
            return usuarios;
        }
        public static async Task<bool> ComprobarUsuario(string param)
        {
            QConsumer qc = new QConsumer();
            return await qc.GetAsync<bool>($"{baseEndPoint}/usuarios/c={param}");
        }
        public static async Task<Usuario> GetUsuario(long id)
        {
            QConsumer qc = new QConsumer();
            Usuario usuario = await qc.GetAsync<Usuario>($"{baseEndPoint}/usuarios/{id}");
            return usuario;
        }
        public static async Task<Usuario> Login(string param, string password)
        {
            QConsumer qc = new QConsumer();
            Usuario usuario = await qc.LoginAsync<Usuario>($"{baseEndPoint}/login", param, password);
            return usuario;
        }
        public static async void CreateUsuario(Usuario usuario)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync($"{baseEndPoint}/usuarios", usuario);
        }
        public static async Task<List<Usuario>> Search(string query)
        {
            QConsumer qc = new QConsumer();
            List<Usuario> usuarios = await qc.GetAsync<List<Usuario>>($"{baseEndPoint}/usuarios/q={query}");
            return usuarios;
        }
        public static async void UpdateUsuario(long id, Usuario usuario)
        {
            QConsumer qc = new QConsumer();
            _ = await qc.UpdateAsync($"{baseEndPoint}/usuarios/{id}", usuario);
        }
        public static async Task<bool> ComprobarSeguimiento(long idusuario, long idcontacto)
        {
            QConsumer qc = new QConsumer();
            return await qc.GetAsync<bool>($"{baseEndPoint}/usuarios/{idusuario}/check/{idcontacto}");
        }
        public static async Task FollowUser(UsuarioUsuario follow)
        {
            QConsumer qc = new QConsumer();
            await qc.PostAsync<UsuarioUsuario>($"{baseEndPoint}/usuarioUsuarios/follow", follow);
        }
        public static async Task UnfollowUser(long idUsuario, long idContacto)
        {
            QConsumer qc = new QConsumer();
            await qc.DeleteAsync<UsuarioUsuario>($"{baseEndPoint}/usuarioUsuarios/unfollow/usuario/{idUsuario}/contacto/{idContacto}");
        }
        #endregion


        #region Terrarios
        public static async Task<List<Terrario>> GetTerrarios() //op
        {
            QConsumer qc = new QConsumer();
            List<Terrario> terrarios = await qc.GetAsync<List<Terrario>>($"{baseEndPoint}/terrarios");
            return terrarios;
        }
        public static async Task<List<Terrario>> GetTerrariosUsuario(long id)
        {
            QConsumer qc = new QConsumer();
            List<Terrario> terrarios = await qc.GetAsync<List<Terrario>>($"{baseEndPoint}/terrarios/usuario/{id}");
            return terrarios;
        }
        public static async Task<List<Terrario>> GetTerrariosSocial(long id)
        {
            QConsumer qc = new QConsumer();
            List<Terrario> terrarios = await qc.GetAsync<List<Terrario>>($"{baseEndPoint}/terrarios-social/{id}");
            return terrarios;
        }
        public static async Task CreateTerrario(Terrario terrario)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Terrario>($"{baseEndPoint}/terrarios", terrario);
        }
        public static async Task UpdateTerrario(long id, Terrario terrario)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<Terrario>($"{baseEndPoint}/terrarios/{id}", terrario);
        }
        public static async Task<float> GetPuntuacionTerrario(long id)
        {
            QConsumer qc = new QConsumer();
            return await qc.GetAsync<float>($"{baseEndPoint}/terrarios/{id}/puntuacion");
        }
        public static async Task DeleteTerrario(long id)
        {
            QConsumer qc = new QConsumer();
            await qc.DeleteAsync<Terrario>($"{baseEndPoint}/terrarios/{id}");
        }
        #endregion


        #region Especies
        public static async Task<List<Especie>> GetEspecies() // op
        {
            QConsumer qc = new QConsumer();
            List<Especie> especies = await qc.GetAsync<List<Especie>>($"{baseEndPoint}/especies");
            return especies;
        }
        public static async Task<Especie> GetEspecie(long id) // op
        {
            QConsumer qc = new QConsumer();
            Especie especie = await qc.GetAsync<Especie>($"{baseEndPoint}/especies/{id}");
            return especie;
        }
        public static async Task<List<Especie>> GetEspeciesTerrario(long idTerrario)
        {
            QConsumer qc = new QConsumer();
            List<Especie> especiesTerrario = await qc.GetAsync<List<Especie>>($"{baseEndPoint}/especies/q={idTerrario}");
            return especiesTerrario;
        } // op
        public static async Task<bool> DeleteEspecie(long id)
        {
            QConsumer qc = new QConsumer();
            await qc.DeleteAsync<Task>($"{baseEndPoint}/especies/{id}");
            return true;
        }
        public static async Task AddEspecie(Especie especie)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Especie>($"{baseEndPoint}/especies", especie);
        }
        public static async Task UpdateEspecie(long id, Especie especie)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<Especie>($"{baseEndPoint}/especies/{id}", especie);
        }
        public static async Task<List<Especie>> GetEspeciesPosibles(List<Especie> list)
        {
            QConsumer qc = new QConsumer();
            return await qc.PostAsync($"{baseEndPoint}/especies/posibles", list);
        }
        #endregion


        #region Visitas
        public static async Task<List<Visita>> GetVisitas(long idTerrario) // op
        {
            QConsumer qc = new QConsumer();
            List<Visita> visitas = await qc.GetAsync<List<Visita>>($"{baseEndPoint}/visitas/q={idTerrario}");
            return visitas;
        }

        public static async Task<Visita> GetVisita(long idTerrario, long idUsuario)
        {
            QConsumer qc = new QConsumer();
            Visita visita = await qc.GetAsync<Visita>($"{baseEndPoint}/visitas/{idTerrario}/{idUsuario}");
            return visita;
        }
        public static async Task CreateVisita(Visita visita)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Visita>($"{baseEndPoint}/visitas", visita);
        }
        public static async Task UpdateVisita(long id, Visita visita)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<Visita>($"{baseEndPoint}/visitas/{id}", visita);
        }
        #endregion


        #region Logros
        public static async Task<List<Logro>> GetLogros()
        {
            QConsumer qc = new QConsumer();
            List<Logro> logros = await qc.GetAsync<List<Logro>>($"{baseEndPoint}/logros");
            return logros;
        }
        public static async Task UpdateLogro(Logro logro)
        {
            QConsumer qc = new QConsumer();
            _ = await qc.UpdateAsync($"{baseEndPoint}/logros/{logro.Id}", logro);
        }
        public static async Task<List<Logro>> GetLogrosUsuario(long id)
        {
            QConsumer qc = new QConsumer();
            List<Logro> logros = await qc.GetAsync<List<Logro>>($"{baseEndPoint}/logros-usuario/{id}");
            return logros;
        } // op
        public static async Task CreateLogro(Logro logro)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Logro>($"{baseEndPoint}/logros", logro);
        }
        #endregion


        #region Notificaciones
        public static async Task<List<Notificacion>> GetNotificacionesByUserId(long id)
        {
            QConsumer qc = new QConsumer();
            return await qc.GetAsync<List<Notificacion>>($"{baseEndPoint}/notificaciones/usuario/{id}");
        }
        public static async Task UpdateNotificacion(Notificacion notificacion, long id)
        {
            QConsumer qc = new QConsumer();
            _ = await qc.UpdateAsync($"{baseEndPoint}/notificacions/{id}", notificacion);
        }
        #endregion


        #region EspeciesTerrario
        public static async Task CreateEspeciesTerrario(EspecieTerrario especieTerrario)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync($"{baseEndPoint}/especie-terrario", especieTerrario);
        }
        public static async Task UpdateEspeciesOfTerrario(long id, List<EspecieTerrario> especiesTerrario)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<List<EspecieTerrario>>($"{baseEndPoint}/especie-terrario/list/{id}", especiesTerrario);
        }
        #endregion


        #region Observaciones
        public static async Task<List<Observacion>> GetObservacionesByTerra(long id)
        {
            QConsumer qc = new QConsumer();
            List<Observacion> observacions = await qc.GetAsync<List<Observacion>>($"{baseEndPoint}/observaviones/terrario/{id}");
            return observacions;
        }
        public static async Task UpdateObservacion(long id, Observacion observacion)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<Observacion>($"{baseEndPoint}/observacions/{id}", observacion);
        }
        public static async Task CreateObservacion(Observacion observacion)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Observacion>($"{baseEndPoint}/observaciones", observacion);
        }
        #endregion


        #region Tareas
        public static async Task<List<Tarea>> GetTareasByTerra(long id)
        {
            QConsumer qc = new QConsumer();
            List<Tarea> tareas = await qc.GetAsync<List<Tarea>>($"{baseEndPoint}/tareas/terrario/{id}");
            return tareas;
        }
        public static async Task UpdateTarea(long id, Tarea tarea)
        {
            QConsumer qc = new QConsumer();
            await qc.UpdateAsync<Tarea>($"{baseEndPoint}/tareas/{id}", tarea);
        }
        public static async Task CreateTarea(Tarea tarea)
        {
            QConsumer qc = new QConsumer();
            await qc.CreateAsync<Tarea>($"{baseEndPoint}/tareas", tarea);
        }
        #endregion


        #region Lecturas
        public static async Task<Lectura> GetLecturaActual(long id)
        {
            QConsumer qc = new QConsumer();
            Lectura lectura = await qc.GetAsync<Lectura>($"{baseEndPoint}/lecturas/terrario/{id}");
            return lectura;
        }
        #endregion
    }
}
