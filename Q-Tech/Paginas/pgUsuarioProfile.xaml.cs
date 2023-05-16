using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Q_Tech.Prop;
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
    public partial class pgUsuarioProfile : Page
    {
        private Usuario _usuario;
        private string _filename;
        public pgUsuarioProfile()
        {
            InitializeComponent();
        }
        public pgUsuarioProfile(Usuario usuario) : this()
        {
            this._usuario = usuario;
            CargarUsuario();
        }

        private void CargarUsuario()
        {
            //imgProfilePic.Source = _usuario.FotoPerfil;
            txtUsername.Text = _usuario.NombreUsuario;
            txbEmail.Text = _usuario.Email;
            pswPassword.Password = _usuario.Contrasena;
            txbName.Text = _usuario.Nombre;
            txbSurname.Text = $"{_usuario.Apellido1} {_usuario.Apellido2}";
            txbTelephone.Text = _usuario.Telefono;
            dtpBirth.Text = _usuario.FechaNacimiento.ToString();
            cboRol.Text = _usuario.Perfil == "CLIENTE" ? "Cliente" : "Admin";
            CargarTerrarios();

            if (_usuario.Perfil.Equals("ADMIN")) stpDoABarrerRoll.Visibility = Visibility.Visible;
        }

        private async void CargarTerrarios()
        {
            lvListaTerrarios.Items.Clear();
            List<Terrario> terrarios = await Herramientas.GetTerrarios();

            foreach (Terrario t in terrarios)
            {

                TextBlock text = new TextBlock
                {
                    Text = t.Nombre
                };

                ListViewItem item = new ListViewItem
                {
                    Content = text,
                    Tag = t.Id
                };

                item.MouseDoubleClick += async (sender, e) =>
                {
                    frmTerraMaker terraMaker = new frmTerraMaker(_usuario, t);

                    if (terraMaker.ShowDialog() == true)
                    {
                        await Herramientas.UpdateTerrario(t.Id, t);
                        CargarTerrarios();
                    }
                };

                lvListaTerrarios.Items.Add(item);
            }
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            spPassword.Visibility = Visibility.Visible;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                _usuario.NombreUsuario = txtUsername.Text;
                _usuario.Email = txbEmail.Text;
                _usuario.Contrasena = pswPassword.Password;
                _usuario.Nombre = txbName.Text;
                string[] apellidos = txbSurname.Text.Split(' '); // error no se por qué
                _usuario.Apellido1 = apellidos[0];
                _usuario.Apellido2 = apellidos[1];
                _usuario.Telefono = txbTelephone.Text;
                if (DateTime.TryParse(dtpBirth.Text, out DateTime fecha))
                {
                    _usuario.FechaNacimiento = fecha;
                }
                _usuario.Perfil = cboRol.Text == "Cliente" ? "CLIENTE" : "ADMIN";

                Herramientas.UpdateUsuario(_usuario.Id, _usuario);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => CargarUsuario();

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(txbEmail.Text))
            {
                MessageBox.Show("El campo nombre de correo no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbEmail.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(pswPassword.Password))
            {
                MessageBox.Show("El campo de contraseña no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                spPassword.Visibility = Visibility.Visible;
                pswPassword.Focus();
                return false;
            }
            return true;
        }

        private void txbTelephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != ' ' && e.Text[0] != '\b' && e.Text[0] != '\t' || (txbTelephone.Text + e.Text).Length > 12)
            {
                e.Handled = true; // Detener la propagación del evento
            }
        }

        private void imgProfilePic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Subir imagen";
            openFileDialog.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                _filename = openFileDialog.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_filename);
                bitmap.EndInit();

                imgProfilePic.Source = bitmap;
            }
        }

        private async void btnAddTerra_Click(object sender, RoutedEventArgs e)
        {
            Terrario terra = new Terrario();

            frmTerraMaker terraMaker = new frmTerraMaker(_usuario, terra);

            if (terraMaker.ShowDialog() == true)
            {
                await Herramientas.CreateTerrario(terra);
                CargarTerrarios();
            }
        }

        private async void btnDelTerra_Click(object sender, RoutedEventArgs e)
        {
            long id = (long)((ListViewItem)sender).Tag;

            MessageBoxResult result = MessageBox.Show("¿Estás seguro de querer borrar este terrario?", "Aviso", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                await Herramientas.DeleteTerrario(id);
            }
        }
    }
}
