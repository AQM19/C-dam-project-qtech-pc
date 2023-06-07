﻿using Azure.Storage.Blobs;
using Azure.Storage;
using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmTerraMaker.xaml
    /// </summary>
    public partial class FrmTerraMaker : Window
    {
        private readonly Usuario _usuario;
        private List<Especie> _especies;
        private readonly List<EspecieTerrario> _especiesTerrario;
        private readonly Terrario _terrario;
        private string _filename;

        public Terrario Terrario { get => _terrario; }
        public List<EspecieTerrario> EspeciesTerrario { get => _especiesTerrario; }

        public FrmTerraMaker()
        {
            InitializeComponent();
            _especies = new List<Especie>();
            _especiesTerrario = new List<EspecieTerrario>();
        }

        public FrmTerraMaker(Usuario usuario, Terrario terrario) : this()
        {
            _terrario = terrario;
            _usuario = usuario;

            if (_terrario.Id == 0)
            {
                btnObservaciones.IsEnabled = false;
                btnObservaciones.Background = Brushes.LightGray;
                btnTareas.IsEnabled = false;
                btnTareas.Background = Brushes.LightGray;
                btnDel.IsEnabled = false;
                btnDel.Background = Brushes.LightGray;
                btnAdd.IsEnabled = false;
                btnAdd.Background = Brushes.LightGray;
            }

            DesplegarInformacion();

            this.Title = $"Terrario de {_usuario.NombreUsuario}";
        }

        private async void DesplegarInformacion()
        {
            if (_terrario != null)
            {
                tbName.Text = !string.IsNullOrEmpty(_terrario.Nombre) ? _terrario.Nombre : "Nombre del terrario";
                chkPrivate.IsChecked = _terrario.Privado != 0;
                txbSustrato.Text = _terrario.Sustrato;
                txbEcosistema.Text = _terrario.Ecosistema;
                numberPicker.Text = _terrario.Tamano.ToString();
                imgTerraPic.Source = new BitmapImage(new Uri(_terrario.Foto ?? "\\Recursos\\Iconos\\MainIcon.png", UriKind.RelativeOrAbsolute));
                _filename = imgTerraPic.Source.ToString();
                txbDescripcion.Text = _terrario.Descripcion;

                if (_terrario.Id != 0)
                {
                    _especies = await Herramientas.GetEspeciesTerrario(_terrario.Id);
                }

                if (_terrario.Privado == 0)
                {
                    spPassOne.Visibility = Visibility.Hidden;
                    spPassTwo.Visibility = Visibility.Hidden;
                }

                MostrarEspecies();
            }
        }

        private void tbDescription_KeyUp(object sender, KeyEventArgs e)
        {
            int length = txbDescripcion.Text.Length;

            tbDescription.Text = $"{length} caracteres";

            if (length > 0 && length < 200)
                tbDescription.Foreground = Brushes.LimeGreen;

            if (length > 200 && length < 250)
            {
                tbDescription.Foreground = Brushes.Orange;
                tbDescription.Visibility = Visibility.Hidden;
            }

            if (length > 250)
            {
                tbDescription.Foreground = Brushes.Red;
                tbDescription.Visibility = Visibility.Visible;
            }
        }

        private void chkPrivate_Click(object sender, RoutedEventArgs e)
        {
            spPassOne.Visibility = (chkPrivate.IsChecked == true) ? Visibility.Visible : Visibility.Hidden;
            spPassTwo.Visibility = (chkPrivate.IsChecked == true) ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidData())
            {
                _terrario.Idusuario = _usuario.Id;
                _terrario.Nombre = tbName.Text;

                if (chkPrivate.IsChecked == true)
                {
                    _terrario.Privado = 1;
                    _terrario.Contrasena = EncriptacionSHA256(pswPassOne.Password);
                }

                _terrario.Sustrato = txbSustrato.Text;
                _terrario.Ecosistema = txbEcosistema.Text;

                if (!string.IsNullOrEmpty(numberPicker.Text))
                {
                    _terrario.Tamano = Int32.Parse(numberPicker.Text);
                }

                if (_filename != null)
                {
                    _terrario.Foto = CargarImagenAzure();
                }

                _terrario.Descripcion = txbDescripcion.Text;
                _terrario.FechaCreacion = DateTime.Today;
                _terrario.FechaUltimaModificacion = DateTime.Today;

                for (int i = 0; i < _especies.Count; i++)
                {
                    _especiesTerrario.Add(new EspecieTerrario
                    {
                        Idespecie = _especies[i].Id,
                        Idterrario = _terrario.Id,
                        FechaInsercion = DateTime.Today
                    });
                }


                this.DialogResult = true;
                this.Close();
            }
        }

        private string CargarImagenAzure()
        {
            if (!string.IsNullOrEmpty(_filename) && !_filename.Equals(imgTerraPic.Source.ToString()))
            {
                string containerName = _usuario.NombreUsuario.ToLower();
                string imageName = _terrario.Nombre;
                long miliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                string uri = $"https://qtechstorage.blob.core.windows.net/{containerName}/{imageName}{miliseconds}{Path.GetExtension(_filename)}".Replace(" ", "").Trim();

                Uri blobUri = new Uri(uri);
                StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential("qtechstorage", ConfigurationManager.AppSettings["qtechstorage"].ToString());
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

                FileStream fileStream = File.OpenRead(Path.GetFullPath(_filename));
                blobClient.Upload(fileStream, true);
                fileStream.Close();

                return blobUri.ToString();
            }

            return imgTerraPic.Source.ToString();
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

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
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
            if (chkPrivate.IsChecked == true && string.IsNullOrEmpty(pswPassOne.Password))
            {
                MessageBox.Show("Si el terrario es privado debería tener una contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                pswPassOne.Focus();
                return false;
            }
            if (!string.IsNullOrEmpty(numberPicker.Text) && int.Parse(numberPicker.Text) <= 0)
            {
                MessageBox.Show("El terrario no puede tener un tamaño negativo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                numberPicker.Focus();
                return false;
            }

            return true;
        }

        private void bdrAddSp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrmEspecies frmEspecies = new FrmEspecies(_especies);
            if (frmEspecies.ShowDialog() == true)
            {
                Especie especie = frmEspecies.Especie;

                if (_especies.FirstOrDefault(x => x.Id == especie.Id) == null)
                {
                    _especies.Add(especie);
                    MostrarEspecies();
                }
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
                imgTerraPic.Source = new BitmapImage(new Uri(_filename));
            }
        }

        private void numberPicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != "\t" && e.Text != "\b")
            {
                e.Handled = true;
            }
        }

        private void btnObservaciones_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrmListaObservaciones lista = new FrmListaObservaciones(_terrario.Id);
            lista.ShowDialog();
        }

        private void btnTareas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrmListaTareas lista = new FrmListaTareas(_terrario.Id);
            lista.ShowDialog();
        }
    }
}
