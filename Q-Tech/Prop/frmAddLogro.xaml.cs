using Azure.Storage.Blobs;
using Azure.Storage;
using Entities;
using Q_Tech.Paginas;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Xml.Linq;
using Microsoft.Win32;
using Path = System.IO.Path;
using System.Web.UI.WebControls;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmAddLogro.xaml
    /// </summary>
    public partial class FrmAddLogro : Window
    {
        private readonly Logro _logro;
        private string _filename;
        public FrmAddLogro()
        {
            InitializeComponent();
        }
        public FrmAddLogro(Logro logro) : this()
        {
            _logro = logro;

            DesplegarInformacion();
        }

        public Logro Logro { get { return _logro; } }

        private void DesplegarInformacion()
        {
            imgLogro.Source = new BitmapImage(new Uri(_logro.Icono ?? "\\Recursos\\Iconos\\MainIcon.png", UriKind.RelativeOrAbsolute));
            txbLogroName.Text = !string.IsNullOrEmpty(_logro.Titulo) ? _logro.Titulo : "Nombre del logro";
            dtpFechadesde.SelectedDate = _logro.Fechadesde;
            dtpFechahasta.SelectedDate = _logro.Fechahasta;
            txbLogroDescripcion.Text = _logro.Descripcion;
        }
        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private async void btnAddLogro_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidData())
            {
                _logro.Titulo = txbLogroName.Text;
                _logro.Descripcion = txbLogroDescripcion.Text;
                _logro.Fechadesde = dtpFechadesde.SelectedDate;
                _logro.Fechahasta = dtpFechahasta.SelectedDate;
                if (_logro.Icono == null)
                {
                    _logro.Icono = await CargarImagenAzure();
                }

                this.DialogResult = true;
                this.Close();
            }
        }

        private async Task<string> CargarImagenAzure()
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                Uri blobUri = new Uri($"https://qtechstorage.blob.core.windows.net/logros/{_logro.Titulo}{Path.GetExtension(_filename)}");
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

        private bool ValidData()
        {
            if (imgLogro.Source == null)
            {
                MessageBox.Show("Debe haber un icono establecido.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                imgLogro.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbLogroName.Text) || txbLogroName.Text == "Nombre del logro")
            {
                MessageBox.Show("El logro debe tener un nombre.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbLogroName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txbLogroDescripcion.Text))
            {
                MessageBox.Show("El logro debe tener una descripción.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbLogroDescripcion.Focus();
                return false;
            }
            if (dtpFechadesde.SelectedDate.HasValue && dtpFechahasta.SelectedDate.HasValue && dtpFechadesde.SelectedDate > dtpFechahasta.SelectedDate)
            {
                MessageBox.Show("No se puede poner la fecha de obtención mayor a la fecha de caducidad del logro.");
                dtpFechadesde.Focus();
                return false;
            }
            return true;
        }

        private void imgLogro_MouseDown(object sender, MouseButtonEventArgs e)
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

                imgLogro.Source = bitmap;
            }
        }
    }
}
