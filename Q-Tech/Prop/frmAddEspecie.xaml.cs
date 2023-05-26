using Azure.Storage.Blobs;
using Azure.Storage;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Path = System.IO.Path;
using Microsoft.Win32;
using System.Configuration;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmAddEspecie.xaml
    /// </summary>
    public partial class FrmAddEspecie : Window
    {
        private readonly Especie _especie;
        private string _filename;

        public FrmAddEspecie()
        {
            InitializeComponent();
        }
        public FrmAddEspecie(Especie especie) : this()
        {
            _especie = especie;
            CargarEspecie();
        }

        public Especie Especie { get; }

        private void CargarEspecie()
        {
            if (string.IsNullOrEmpty(_especie.Genero))
            {
                txbGenSp.Text = "Genero Especie";
            }
            else
            {
                txbGenSp.Text = $"{_especie.Genero} {_especie.Sp}";
            }
            txbNombreComun.Text = _especie.NombreComun;
            txbDescripcion.Text = _especie.Descripcion;
            imgTerraPic.Source = new BitmapImage(new Uri(_especie.Imagen ?? "/Recursos/Iconos/MainIcon.png", UriKind.Relative));
            txbEcosistema.Text = _especie.Ecosistema;
            txbAniosVida.Text = _especie.DuracionVida.ToString();
            txbMinTemp.Text = _especie.TemperaturaMinima.ToString();
            txbMaxTemp.Text = _especie.TemperaturaMaxima.ToString();
            txbMinHum.Text = _especie.HumedadMinima.ToString();
            txbMaxHum.Text = _especie.HumedadMaxima.ToString();
            txbMaxLuz.Text = _especie.HorasLuz.ToString();
            cboClase.Text = _especie.Tipo;
            if (_especie.Hiberna == 1)
            {
                chkHibernacion.IsChecked = true;
                txbMaxTempHib.Text = _especie.TemperaturaHibMaxima.ToString();
                txbMinTempHib.Text = _especie.TemperaturaHibMinima.ToString();
                txbMaxLuzHib.Text = _especie.HorasLuzHib.ToString();
            }
        }
        private async void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidData())
            {
                string[] gensp = txbGenSp.Text.Split(' ');

                _especie.Genero = gensp[0];
                _especie.Sp = gensp[1];
                _especie.NombreComun = txbNombreComun.Text;
                _especie.Descripcion = txbDescripcion.Text;
                _especie.Imagen = await ObtenerImagen();
                _especie.Ecosistema = txbEcosistema.Text;
                _especie.DuracionVida = Int32.Parse(txbAniosVida.Text);
                _especie.Hiberna = 0;
                _especie.TemperaturaMinima = Int32.Parse(txbMinTemp.Text);
                _especie.TemperaturaMaxima = Int32.Parse(txbMaxTemp.Text);
                _especie.HumedadMinima = Int32.Parse(txbMinHum.Text);
                _especie.HumedadMaxima = Int32.Parse(txbMaxHum.Text);
                _especie.HorasLuz = Int32.Parse(txbMaxLuz.Text);
                _especie.Tipo = cboClase.Text;
                if (chkHibernacion.IsChecked == true)
                {
                    _especie.Hiberna = 1;
                    _especie.TemperaturaHibMaxima = Int32.Parse(txbMaxTempHib.Text);
                    _especie.TemperaturaHibMinima = Int32.Parse(txbMinTempHib.Text);
                    _especie.HorasLuzHib = Int32.Parse(txbMaxLuzHib.Text);
                }

                this.DialogResult = true;
                this.Close();
            }
        }
        private async Task<string> ObtenerImagen()
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                Uri blobUri = new Uri($"https://qtechstorage.blob.core.windows.net/especies/{_especie.Genero}{_especie.Sp}{Path.GetExtension(_filename)}");
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
            return null;
        }
        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void txbMaxTemp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != "\t" && e.Text != "\b")
            {
                e.Handled = true;
            }
        }
        private void chkHibernacion_Click(object sender, RoutedEventArgs e)
        {
            if (chkHibernacion.IsChecked == false)
            {
                spTempHibMAX.Visibility = Visibility.Collapsed;
                spTempHibmin.Visibility = Visibility.Collapsed;
                spLuzHibMAX.Visibility = Visibility.Collapsed;

                Grid.SetColumnSpan(spTempMAX, 2);
                Grid.SetColumnSpan(spTempmin, 2);
                Grid.SetColumnSpan(spLuzMAX, 2);
            }
            else
            {
                spTempHibMAX.Visibility = Visibility.Visible;
                spTempHibmin.Visibility = Visibility.Visible;
                spLuzHibMAX.Visibility = Visibility.Visible;

                Grid.SetColumnSpan(spTempMAX, 1);
                Grid.SetColumnSpan(spTempmin, 1);
                Grid.SetColumnSpan(spLuzMAX, 1);
            }
        }
        private bool ValidData()
        {
            if (string.IsNullOrEmpty(txbGenSp.Text) || txbGenSp.Text.Equals("Genero Especie"))
            {
                MessageBox.Show("La especie debe tener su nombre científico asignado.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbGenSp.Focus();
                return false;
            }
            if (!string.IsNullOrEmpty(txbAniosVida.Text) && !Int32.TryParse(txbAniosVida.Text, out _))
            {
                MessageBox.Show("Los años de vida de una especie han de ser numéricos.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbAniosVida.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cboClase.Text))
            {
                MessageBox.Show("Se ha de elegir una clase.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                cboClase.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txbMaxTemp.Text) && !Int32.TryParse(txbMaxTemp.Text, out _))
            {
                MessageBox.Show("Se ha de establecer el valor máximo de temperatura.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxTemp.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txbMinTemp.Text) && !Int32.TryParse(txbMinTemp.Text, out _))
            {
                MessageBox.Show("Se ha de establecer el valor mínimo de temperatura.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMinTemp.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbMaxHum.Text) && !Int32.TryParse(txbMaxHum.Text, out _))
            {
                MessageBox.Show("Se ha de establecer el valor máximo de humedad.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxHum.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbMinHum.Text) && !Int32.TryParse(txbMinHum.Text, out _))
            {
                MessageBox.Show("Se ha de establecer el valor máximo de humedad.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMinHum.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbMaxLuz.Text) && !Int32.TryParse(txbMaxLuz.Text, out _))
            {
                MessageBox.Show("Se ha de establecer el valor máximo de horas de luz.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxLuz.Focus();
                return false;
            }

            if (Int32.Parse(txbAniosVida.Text) < 0)
            {
                MessageBox.Show("No se puede poner un valor negativo a los años de vida.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxLuzHib.Focus();
                return false;
            }
            if (Int32.Parse(txbMinTemp.Text) > Int32.Parse(txbMaxTemp.Text))
            {
                MessageBox.Show("La temperatura mínima no puede ser superior a la máxima.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMinTemp.Focus();
                return false;
            }
            if (Int32.Parse(txbMinHum.Text) > Int32.Parse(txbMaxHum.Text))
            {
                MessageBox.Show("La humedad mínima no puede ser superior a la máxima.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMinHum.Focus();
                return false;
            }
            if (Int32.Parse(txbMaxLuz.Text) < 0)
            {
                MessageBox.Show("No se puede necesitar tener horas de luz diarias en negativo.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxLuzHib.Focus();
                return false;
            }
            if (Int32.Parse(txbMaxLuz.Text) > 24)
            {
                MessageBox.Show("No puede necesitar más horas de luz diarias de las que hay en un día.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txbMaxLuzHib.Focus();
                return false;
            }

            if (chkHibernacion.IsChecked == true)
            {
                // Controles
                if (string.IsNullOrEmpty(txbMaxTempHib.Text) && !Int32.TryParse(txbMaxTempHib.Text, out _))
                {
                    MessageBox.Show("Se ha de establecer el valor máximo de temperatura en hibernación.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMaxTempHib.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txbMinTempHib.Text) && !Int32.TryParse(txbMinTempHib.Text, out _))
                {
                    MessageBox.Show("Se ha de establecer el valor mínimo de temperatura en hibernación.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMinTempHib.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txbMaxLuzHib.Text) && !Int32.TryParse(txbMaxLuzHib.Text, out _))
                {
                    MessageBox.Show("Se ha de establecer el valor máximo de horas de luz en hibernación.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMaxLuzHib.Focus();
                    return false;
                }

                // Coherencia
                if (Int32.Parse(txbMaxTempHib.Text) > Int32.Parse(txbMinTemp.Text))
                {
                    MessageBox.Show("El máximo de temperatura en hibernación permitido no puede ser al mínimo normal permitido.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMaxTempHib.Focus();
                    return false;
                }
                if (Int32.Parse(txbMinTempHib.Text) > Int32.Parse(txbMaxTempHib.Text))
                {
                    MessageBox.Show("No se puede establecer como mínima temperatura permitida un valor más alto que el máximo permitido.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMinTempHib.Focus();
                    return false;
                }
                if (Int32.Parse(txbMaxLuzHib.Text) > Int32.Parse(txbMaxLuz.Text))
                {
                    MessageBox.Show("No se puede poner un valor más alto de horas de luz necesarias en hibernación que de normal.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txbMaxLuzHib.Focus();
                    return false;
                }
            }

            return true;
        }
        private void imgTerraPic_MouseDown(object sender, MouseButtonEventArgs e)
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

                imgTerraPic.Source = bitmap;
            }
        }
    }
}
