using BusinessLogic;
using Entities;
using Microsoft.Win32;
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
        private Usuario _usuario;
        private string _filename;
        public pgUsuario()
        {
            InitializeComponent();
        }
        public pgUsuario(Usuario usuario) : this()
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

            if (_usuario.Perfil.Equals("ADMIN")) stpDoABarrerRoll.Visibility = Visibility.Visible;
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
    }
}
