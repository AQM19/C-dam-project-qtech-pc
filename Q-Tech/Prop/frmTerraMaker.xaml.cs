using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmTerraMaker.xaml
    /// </summary>
    public partial class frmTerraMaker : Window
    {
        private Usuario _usuario;
        private List<Especie> _especies;
        private List<EspecieTerrario> _especiesTerrario;
        private Terrario _terrario;

        public Terrario Terrario { get => _terrario; set => _terrario = value; }

        public frmTerraMaker()
        {
            InitializeComponent();
            _especies = new List<Especie>();
            _especiesTerrario = new List<EspecieTerrario>();
        }

        public frmTerraMaker(Usuario usuario, Terrario terrario) : this()
        {
            _usuario = usuario;
            _terrario = terrario;
            DesplegarInformacion();

            this.Title = $"Terrario de {_usuario.NombreUsuario}";
        }

        private async void DesplegarInformacion()
        {
            if (_terrario != null)
            {
                tbName.Text = _terrario.Nombre;
                chkPrivate.IsChecked = (_terrario.Privado == 0) ? false : true;
                cmbSustrato.SelectedItem = _terrario.Sustrato;
                cmbEcosistema.SelectedItem = _terrario.Ecosistema;
                numberPicker.Text = _terrario.Tamano.ToString();
                txtImageSource.Text = _terrario.Foto;
                tbDescription.Text = _terrario.Descripcion;

                _especiesTerrario = await Herramientas.GetEspeciesTerrario(_terrario.Id);

                for (int i = 0; i < _especiesTerrario.Count; i++)
                {
                    Especie especie = await Herramientas.GetEspecie(_especiesTerrario[i].Idespecie);
                    _especies.Add(especie);
                }

                MostrarEspecies();
            }
        }

        private void spSize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            numberPicker.Focus();
        }

        private void numberPicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsNumberKey(e.Key) && e.Key != Key.Tab)
            {
                e.Handled = true;
            }
        }

        private bool IsNumberKey(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }

        private void tbDescription_KeyUp(object sender, KeyEventArgs e)
        {
            int length = tbDescription.Text.Length;
            string text = tbDescription.Text;
            Brush foreground = tbDescription.Foreground;

            text = $"{length} caracteres";

            if (length > 0 && length < 200)
                foreground = Brushes.LimeGreen;
            if (length > 200 && length < 250)
            {
                foreground = Brushes.Orange;
                tbDescription.Visibility = Visibility.Hidden;
            }

            if (length > 250)
            {
                foreground = Brushes.Red;
                tbDescription.Visibility = Visibility.Visible;
            }

        }

        private void bdrUpload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Subir imagen";
            openFileDialog.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
            }
        }

        private void chkPrivate_Click(object sender, RoutedEventArgs e)
        {
            spPass.Visibility = (chkPrivate.IsChecked == true) ? Visibility.Visible : Visibility.Hidden;
        }

        private async void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                _terrario.Idusuario = _usuario.Id;
                _terrario.Nombre = tbName.Text;

                if (chkPrivate.IsChecked == true)
                {
                    _terrario.Privado = 1;
                    _terrario.Contrasena = EncriptacionSHA256(pwbPassword.Password);
                }

                _terrario.Sustrato = cmbSustrato.SelectedItem.ToString();
                _terrario.Ecosistema = cmbEcosistema.SelectedItem.ToString();
                _terrario.Tamano = Int32.Parse(numberPicker.Text);
                _terrario.Foto = CargarImagenAzure();
                _terrario.Descripcion = tbDescription.Text;
                _terrario.FechaCreacion = DateTime.Today;
                _terrario.FechaUltimaModificacion = DateTime.Today;

                if (_especies.Count > 0)
                {
                    _terrario.TemperaturaMinima = _especies.Max(x => x.TemperaturaMinima);
                    _terrario.TemperaturaMaxima = _especies.Min(x => x.TemperaturaMaxima);
                    _terrario.TemperaturaMedia = (_terrario.TemperaturaMaxima + _terrario.TemperaturaMinima) / 2;

                    _terrario.HumedadMinima = _especies.Max(x => x.HumedadMinima);
                    _terrario.HumedadMaxima = _especies.Min(x => x.HumedadMaxima);
                    _terrario.HumedadMedia = (_terrario.HumedadMaxima + _terrario.HumedadMinima) / 2;

                    _terrario.HorasLuz = _especies.Min(x => x.HorasLuz);

                    _terrario.Hibernacion = (sbyte)(_especies.All(x => x.Hiberna == 1) ? 1 : 0);

                    if (_terrario.Hibernacion == 1)
                    {
                        _terrario.TemperaturaMinimaHiber = _especies.Max(x => x.TemperaturaHibMinima);
                        _terrario.TemperaturaMaximaHiber = _especies.Min(x => x.TemperaturaHibMaxima);
                        _terrario.TemperaturaMediaHiber = (_terrario.TemperaturaMaximaHiber + _terrario.TemperaturaMinimaHiber) / 2;

                        _terrario.HorasLuzHiber = _especies.Min(x => x.HorasLuzHib);
                    }
                }

                if (_terrario.Id >= 0)
                {
                    List<Visita> visitas = await Herramientas.GetVisitas(_terrario.Id);
                    _terrario.PuntuacionMedia = visitas.Average(x => x.Puntuacion);
                }

                List<long> idEspecies = _especies.Select(x => x.Id).ToList();
                List<long> idEspeciesTerrario = _especiesTerrario.Select(x => x.Idespecie).ToList();
                List<long> idFaltantes = idEspecies.Except(idEspeciesTerrario).ToList();

                EspecieTerrario especieTerrario;

                for (int i = 0; i < idFaltantes.Count; i++)
                {
                    especieTerrario = new EspecieTerrario
                    {
                        Idespecie = idFaltantes[i],
                        Idterrario = _terrario.Id,
                        FechaInsercion = DateTime.Today
                    };

                    // Crearlo con Herramientas
                }

                this.Close();
            }
        }

        private string CargarImagenAzure()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=aquintanalm01@educantabria.es;AccountKey=6e499D18eD86@1;EndpointSuffix=core.windows.net";
            string containerName = _usuario.NombreUsuario;
            string userName = _usuario.NombreUsuario;
            string imageName = _terrario.Nombre;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            string prefix = userName + "/";

            CloudBlockBlob blob = container.GetBlockBlobReference(prefix + imageName);

            using (FileStream fileStream = File.OpenRead(txtImageSource.Text))
            {
                blob.UploadFromStream(fileStream);
            }

            return $"https://qtechstorage.blob.core.windows.net/{prefix}{imageName}";
        }

        private string EncriptacionSHA256(string psw)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(psw);
            string hashString = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                hashString = BitConverter.ToString(hash).Replace("-", "");
            }

            return hashString;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("El campo nombre no puede estar vacío", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbName.Focus();
                return false;
            }
            if (chkPrivate.IsChecked == true && string.IsNullOrEmpty(pwbPassword.Password))
            {
                MessageBox.Show("Si el terrario es privado debería tener una contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                pwbPassword.Focus();
                return false;
            }
            if (int.Parse(numberPicker.Text) <= 0)
            {
                MessageBox.Show("El terrario no puede tener un tamaño negativo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                numberPicker.Focus();
                return false;
            }

            return true;
        }

        private void bdrAddSp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            frmEspecies frmEspecies = new frmEspecies();
            frmEspecies.ShowDialog();

            Especie especie = frmEspecies.Especie;

            if (_especies.FirstOrDefault(x => x.Id == especie.Id) == null)
            {
                _especies.Add(especie);
                MostrarEspecies();
            }
        }

        private void bdrDelSp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvEspecies.SelectedItems.Count > 0)
            {
                ListViewItem item = (ListViewItem)lvEspecies.SelectedItem;
                long id = (long)item.Tag;
                Especie especie = _especies.Find(especies => especies.Id == id);
                _especies.Remove(especie);
                MostrarEspecies();
            }
        }

        private void MostrarEspecies()
        {
            lvEspecies.Items.Clear();

            for (int i = 0; i < _especies.Count; i++)
            {
                ListViewItem item = new ListViewItem
                {
                    Content = $"{_especies[i].Genero} {_especies[i].Sp}",
                    Tag = _especies[i].Id
                };

                lvEspecies.Items.Add(item);
            }
        }
    }
}
