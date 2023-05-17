using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Azure.Storage;
using Azure.Storage.Blobs;
using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Q_Tech.Modales;
using Q_Tech.Paginas;

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
            _showRegisterPassword = false;
        }

        private void PasswordViewer_Click(object sender, RoutedEventArgs e)
        {
            _showPassword = !_showPassword;
            PasswordUnbind.Visibility = _showPassword ? Visibility.Visible : Visibility.Collapsed;
            PasswordBind.Visibility = _showPassword ? Visibility.Collapsed : Visibility.Visible;
            imgLoginShowPassword.Source = new BitmapImage(new Uri($"/Recursos/Iconos/{(_showPassword ? "hide" : "show")}.png", UriKind.Relative));
        }

        private void btnShowRegisterPass_Click(object sender, RoutedEventArgs e)
        {
            _showRegisterPassword = !_showRegisterPassword;
            txbRegisterPass.Visibility = _showRegisterPassword ? Visibility.Visible : Visibility.Collapsed;
            pwbRegisterPass.Visibility = _showRegisterPassword ? Visibility.Collapsed : Visibility.Visible;
            imgRegisterShowPassword.Source = new BitmapImage(new Uri($"/Recursos/Iconos/{(_showRegisterPassword ? "hide" : "show")}.png", UriKind.Relative));
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
            Animacion(brdRegistro, true);
            Animacion(brdInicio, false);
        }

        private void txtVerInicio_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Animacion(brdInicio, true);
            Animacion(brdRegistro, false);
        }

        private void Animacion(Border border, bool entrada)
        {
            DoubleAnimation anim = new DoubleAnimation
            {
                From = entrada ? 0 : 1,
                To = entrada ? 1 : 0,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            border.BeginAnimation(Border.VisibilityProperty, new ObjectAnimationUsingKeyFrames
            {
                KeyFrames = new ObjectKeyFrameCollection
                {
                    new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))),
                }
            });

            border.BeginAnimation(Border.OpacityProperty, anim);
        }

        private void txtRecordar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void pthImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Subir imagen",
                Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png"
            };

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
                dpLoader.Visibility = Visibility.Visible;

                Usuario usuario;

                try
                {
                    usuario = await Herramientas.Login(txtUser.Text, PasswordBind.Password);
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo establecer una conexión con el servidor. Por favor, inténtelo de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                    dpLoader.Visibility = Visibility.Collapsed;
                    return;
                }

                if (usuario == null)
                {
                    MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtUser.Text = string.Empty;
                    PasswordBind.Password = string.Empty;
                    PasswordUnbind.Text = string.Empty;
                    dpLoader.Visibility = Visibility.Collapsed;
                    return;
                }

                IngresarAplicacion(usuario);
            }
        }

        private async void brdRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (await ValidRegisterData())
            {
                dpLoader.Visibility = Visibility.Visible;

                byte[] salt = GenerarSalt();
                string password = GenerarContra(salt);
                string username = txbUsername.Text.ToLower();

                await CrearContainerBlobAzure(username);

                string fotoPerfil = await CargarImagenPerfilAzure(username);

                Usuario newRegisterUser = new Usuario
                {
                    NombreUsuario = username,
                    Email = txbEmail.Text,
                    Salt = salt,
                    Contrasena = password,
                    FotoPerfil = fotoPerfil,
                    Perfil = "CLIENTE"
                };

                try
                {
                    Herramientas.CreateUsuario(newRegisterUser);
                    IngresarAplicacion(newRegisterUser);
                }
                catch (Exception)
                {
                    MessageBox.Show("No se pudo establecer una conexión con el servidor. Por favor, inténtelo de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                    dpLoader.Visibility = Visibility.Collapsed;
                    return;
                }
            }
        }

        private void IngresarAplicacion(Usuario usuario)
        {
            FrmDashboard dashboard = new FrmDashboard(usuario);
            this.Hide();
            if (dashboard.ShowDialog() == false)
            {
                this.Close();
            }
            else
            {
                dpLoader.Visibility = Visibility.Collapsed;
                this.Show();
            }
        }

        private async Task<string> CargarImagenPerfilAzure(string username)
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                Uri blobUri = new Uri($"https://qtechstorage.blob.core.windows.net/{username}/profile_pic{Path.GetExtension(_filename)}");
                StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential("qtechstorage", ConfigurationManager.AppSettings["qtechstorage"].ToString());
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

                FileStream fileStream = File.OpenRead(Path.GetFullPath(_filename));
                await blobClient.UploadAsync(fileStream, true);
                fileStream.Close();

                if (await Task.FromResult(true))
                {
                    return blobUri.ToString();
                }
            }

            return string.Empty;
        }

        private async Task<BlobContainerClient> CrearContainerBlobAzure(string username)
        {
            string connectionString = ConfigurationManager.AppSettings["azureacc"].ToString();
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            string containerName = username;

            BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(containerName);

            if (await container.ExistsAsync())
            {
                return container;
            }

            return null;
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
            int saltLength = 32;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[saltLength];
            rng.GetBytes(saltBytes);
            return saltBytes;
        }

        private async Task<bool> ValidRegisterData()
        {
            try
            {
                dpLoader.Visibility = Visibility.Visible;
                if (string.IsNullOrEmpty(txbUsername.Text))
                {
                    MessageBox.Show("El campo nombre de usuario no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbUsername.Focus();
                    dpLoader.Visibility = Visibility.Collapsed;
                    return false;
                }
                if (!await Herramientas.ComprobarUsuario(txbUsername.Text))
                {
                    MessageBox.Show("Ese nombre de usuario ya está en uso.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbUsername.Focus();
                    dpLoader.Visibility = Visibility.Collapsed;
                    return false;
                }
                if (string.IsNullOrEmpty(txbEmail.Text))
                {
                    MessageBox.Show("El campo email no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbEmail.Focus();
                    dpLoader.Visibility = Visibility.Collapsed;
                    return false;
                }
                if (!await Herramientas.ComprobarUsuario(txbEmail.Text))
                {
                    MessageBox.Show("Ese email ya está en uso.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txbUsername.Focus();
                    dpLoader.Visibility = Visibility.Collapsed;
                    return false;
                }
                if (string.IsNullOrEmpty(pwbRegisterPass.Password))
                {
                    MessageBox.Show("El campo contraseña no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _ = _showRegisterPassword == true ? txbRegisterPass.Focus() : pwbRegisterPass.Focus();
                    dpLoader.Visibility = Visibility.Collapsed;
                    return false;
                }
                dpLoader.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo establecer una conexión con el servidor. Por favor, inténtelo de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                dpLoader.Visibility = Visibility.Collapsed;
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
