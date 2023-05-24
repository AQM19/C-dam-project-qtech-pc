using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgUsuario.xaml
    /// </summary>
    public partial class pgUsuario : Page
    {
        private readonly Usuario _usuario;
        private readonly Frame _mainFrame;

        public pgUsuario()
        {
            InitializeComponent();
        }
        public pgUsuario(long id, Usuario usuario, Frame mainFrame) : this()
        {
            _usuario = usuario;
            _mainFrame = mainFrame;
            DesplegarInformacion(id);
        }

        private async void DesplegarInformacion(long id)
        {
            imgUserPic.Source = new BitmapImage(new Uri(_usuario.FotoPerfil ?? "C:\\Users\\aaron\\OneDrive\\Escritorio\\PROJECTS\\QTECH_PC\\Q-Tech\\Recursos\\Iconos\\MainIcon.png"));
            txbUserName.Text = _usuario.NombreUsuario;
            txbUserTimeline.Text = $"{_usuario.Nombre} {_usuario.Apellido1} {_usuario.Apellido2}";
            btnFollow.Visibility = await Herramientas.ComprobarSeguimiento(id, _usuario.Id) == true ? Visibility.Visible : Visibility.Hidden;

            List<Terrario> terrarios = await Herramientas.GetTerrarios();

            foreach (Terrario t in terrarios)
            {
                ListViewItem item = new ListViewItem
                {
                    Content = t.Nombre
                };

                item.MouseDoubleClick += (o, e) =>
                {
                    pgTerrario pgTerrario = new pgTerrario(t, _usuario.Id);
                    _mainFrame.Content = pgTerrario;
                };

                lvListaTerrarios.Items.Add(item);
            }
        }

        private void btnFollow_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
