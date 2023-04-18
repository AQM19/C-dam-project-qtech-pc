using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using Azure.Storage.Blobs;
using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Q_Tech.Modales;

namespace Q_Tech
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _showPassword;
        private bool _showRegisterPassword;
        private string _filename;

        public MainWindow()
        {
            InitializeComponent();

            _showPassword = false;
        }

        private void PasswordViewer_Click(object sender, RoutedEventArgs e)
        {
            _showPassword = !_showPassword;
            PasswordUnbind.Visibility = _showPassword ? Visibility.Visible : Visibility.Hidden;
            PasswordBind.Visibility = _showPassword ? Visibility.Hidden : Visibility.Visible;
        }

        private void btnShowRegisterPass_Click(object sender, RoutedEventArgs e)
        {
            _showRegisterPassword = !_showRegisterPassword;
            txbRegisterPass.Visibility = _showRegisterPassword ? Visibility.Visible : Visibility.Hidden;
            pwbRegisterPass.Visibility = _showRegisterPassword ? Visibility.Hidden : Visibility.Visible;
        }

        private void brdUser_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUser.Focus();
        }

        private void PasswordBind_KeyUp(object sender, KeyEventArgs e)
        {
            PasswordUnbind.Text = PasswordBind.Password;
        }

        private void PasswordUnbind_KeyUp(object sender, KeyEventArgs e)
        {
            PasswordBind.Password = PasswordUnbind.Text;
        }

        private void txbRegisterPass_KeyUp(object sender, KeyEventArgs e)
        {
            pwbRegisterPass.Password = txbRegisterPass.Text;
        }

        private void pwbRegisterPass_KeyUp(object sender, KeyEventArgs e)
        {
            txbRegisterPass.Text = pwbRegisterPass.Password;
        }

        private void txtRegistro_MouseDown(object sender, MouseButtonEventArgs e)
        {
            brdInicio.Visibility = Visibility.Hidden;
            brdRegistro.Visibility = Visibility.Visible;
        }

        private void txtVerInicio_MouseDown(object sender, MouseButtonEventArgs e)
        {
            brdRegistro.Visibility = Visibility.Hidden;
            brdInicio.Visibility = Visibility.Visible;
        }

        private void txtRecordar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void pthImage_MouseDown(object sender, MouseButtonEventArgs e)
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

                imgBrush.ImageSource = bitmap;
            }
        }

        private async void brdLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidLoginData())
            {
                Usuario usuario = await Herramientas.Login(txtUser.Text, PasswordBind.Password);

                if (usuario == null)
                {
                    MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUser.Text = string.Empty;
                    PasswordBind.Password = string.Empty;
                    PasswordUnbind.Text = string.Empty;
                    return;
                }

                FrmDashboard dashboard = new FrmDashboard(usuario);
                this.Hide();
                dashboard.ShowDialog();
                this.Close();
            }
        }

        private void brdRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(ValidRegisterData())
            {
                byte[] salt = GenerarSalt();
                string password = GenerarContra(salt);
                string imageSource = GenerarImagenAzure();

                Usuario newRegisterUser = new Usuario
                {
                    NombreUsuario = txbUsername.Text,
                    Email = txbEmail.Text,
                    Salt = salt,
                    Contrasena = password,
                    FotoPerfil = 
                };

                Herramientas.CreateUsuario(newRegisterUser);
            }
        }

        private string GenerarImagenAzure()
        {
            if(!string.IsNullOrEmpty(_filename))
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=<qtechstorage>;AccountKey=<FlndiCy8EE6+LS8VJp7r2p5gysm6dZG7AQrfHBJnjB0qiOOJh/pja6TwcxTWNhb66nGKcNnlT8/d+AStv7ldAA==>;EndpointSuffix=core.windows.net";
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                string containerName = $"<{txbUsername.Text}>"; // El nombre del contenedor
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();

                string blobName = "<blob-name>"; // El nombre del blob
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                MemoryStream stream = new MemoryStream(); // Crea un MemoryStream para la imagen

                BitmapImage image = new BitmapImage();

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(_filename);
                image.EndInit();

                image.Save(stream, image.Format); // Guarda la imagen en el MemoryStream en formato PNG

                stream.Seek(0, SeekOrigin.Begin); // Reinicia el puntero de posición del MemoryStream al inicio

                blobClient.Upload(stream); // Sube el MemoryStream al blob
            }

            return "";
        }

        private string GenerarContra(byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pwbRegisterPass.Password);

            byte[] inputBytes = passwordBytes.Concat(salt).ToArray();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passHash = sha256.ComputeHash(inputBytes);
                string hash = Convert.ToBase64String(passHash);
                return hash;
            }
        }

        private byte[] GenerarSalt()
        {
            int saltLength = 16;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[saltLength];
            rng.GetBytes(saltBytes);
            return saltBytes;
        }

        private bool ValidRegisterData()
        {
            if (string.IsNullOrEmpty(txbUsername.Text))
            {
                MessageBox.Show("El campo nombre de usuario no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbUsername.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbEmail.Text))
            {
                MessageBox.Show("El campo email no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbEmail.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(pwbRegisterPass.Password))
            {
                MessageBox.Show("El campo contraseña no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                _ = _showRegisterPassword == true ? txbRegisterPass.Focus() : pwbRegisterPass.Focus();
                return false;
            }
            return true;
        }

        private bool ValidLoginData()
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("El campo usuario no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUser.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(PasswordBind.Password))
            {
                MessageBox.Show("El campo contraseña no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                _ = _showPassword ? PasswordBind.Focus() : PasswordUnbind.Focus();
                return false;
            }
            return true;
        }
    }
}
