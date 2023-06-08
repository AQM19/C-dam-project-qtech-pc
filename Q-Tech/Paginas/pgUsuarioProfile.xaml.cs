using Azure.Storage.Blobs;
using Azure.Storage;
using BusinessLogic;
using Entities;
using Microsoft.Win32;
using Q_Tech.Prop;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.PeerToPeer;
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
using Path = System.IO.Path;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgUsuario.xaml
    /// </summary>
    public partial class PgUsuarioProfile : Page
    {
        private readonly Usuario _usuario;
        private string _filename;
        private bool _imageChanged = false;
        public PgUsuarioProfile()
        {
            InitializeComponent();
        }
        public PgUsuarioProfile(Usuario usuario) : this()
        {
            this._usuario = usuario;
            CargarUsuario();
        }

        private void CargarUsuario()
        {

            if (!string.IsNullOrEmpty(_usuario.FotoPerfil))
            {
                imgProfilePic.Source = new BitmapImage(new Uri(_usuario.FotoPerfil));
                imgProfilePic.Stretch = Stretch.UniformToFill;
                _filename = _usuario.FotoPerfil;
            }

            txtUsername.Text = _usuario.NombreUsuario;
            txbEmail.Text = _usuario.Email;
            txbNombre.Text = _usuario.Nombre;
            txbApellidos.Text = $"{_usuario.Apellido1} {_usuario.Apellido2}";
            txbTelephone.Text = _usuario.Telefono;
            dtpBirth.Text = _usuario.FechaNacimiento.ToString();

            CargarTerrarios();
        }

        private async void CargarTerrarios()
        {
            lvListaTerrarios.Items.Clear();

            List<Terrario> terrarios = await Herramientas.GetTerrariosUsuario(_usuario.Id);

            if (terrarios.Count > 0)
            {
                foreach (Terrario t in terrarios)
                {
                    ListViewItem listViewItem = new ListViewItem
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    Grid grid = new Grid();

                    ColumnDefinition columnDefinition1 = new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Auto)
                    };
                    ColumnDefinition columnDefinition2 = new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    };

                    grid.ColumnDefinitions.Add(columnDefinition1);
                    grid.ColumnDefinitions.Add(columnDefinition2);

                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri(t.Foto ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute)),
                        Width = 75,
                        Height = 75,
                        Stretch = Stretch.Uniform
                    };
                    Grid.SetColumn(image, 0);

                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical
                    };
                    Grid.SetColumn(stackPanel, 1);

                    TextBlock textBlock1 = new TextBlock
                    {
                        Text = t.Nombre,
                        Padding = new Thickness(5),
                        FontSize = 16,
                        FontWeight = FontWeights.DemiBold
                    };

                    TextBlock textBlock2 = new TextBlock
                    {
                        Text = t.Descripcion,
                        TextWrapping = TextWrapping.Wrap,
                        Padding = new Thickness(5)
                    };

                    TextBlock textBlock3 = new TextBlock
                    {
                        Text = t.FechaCreacion.ToShortDateString(),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        FontWeight = FontWeights.Light,
                        Padding = new Thickness(3)
                    };

                    stackPanel.Children.Add(textBlock1);
                    stackPanel.Children.Add(textBlock2);
                    stackPanel.Children.Add(textBlock3);

                    grid.Children.Add(image);
                    grid.Children.Add(stackPanel);

                    listViewItem.Content = grid;
                    listViewItem.Tag = t.Id;

                    listViewItem.MouseDoubleClick += async (sender, e) =>
                    {
                        FrmTerraMaker terraMaker = new FrmTerraMaker(_usuario, t);

                        if (terraMaker.ShowDialog() == true)
                        {
                            await Herramientas.UpdateTerrario(t.Id, t);
                            await Herramientas.UpdateEspeciesOfTerrario(t.Id, terraMaker.EspeciesTerrario);

                            CargarTerrarios();
                        }
                    };

                    lvListaTerrarios.Items.Add(listViewItem);
                }
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            FrmChangePassword frmChangePassword = new FrmChangePassword(_usuario);
            if (frmChangePassword.ShowDialog() == true)
            {
                Herramientas.UpdateUsuario(_usuario.Id, _usuario);
            };
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                _usuario.NombreUsuario = txtUsername.Text;
                _usuario.Email = txbEmail.Text;
                _usuario.Nombre = txbNombre.Text;
                string[] apellidos = txbApellidos.Text.Split(' ');
                _usuario.Apellido1 = apellidos[0];
                _usuario.Apellido2 = apellidos[1];
                _usuario.Telefono = txbTelephone.Text;
                if (DateTime.TryParse(dtpBirth.Text, out DateTime fecha))
                {
                    _usuario.FechaNacimiento = fecha;
                }
                if (_imageChanged == true)
                {
                    _usuario.FotoPerfil = CambiarImagenUsuario();
                }

                Herramientas.UpdateUsuario(_usuario.Id, _usuario);
            }
        }

        private string CambiarImagenUsuario()
        {
            if (!string.IsNullOrEmpty(_filename))
            {
                Uri blobUri = new Uri($"https://qtechstorage.blob.core.windows.net/{_usuario.NombreUsuario.ToLower()}/profile_pic{Path.GetExtension(_filename)}");
                StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential("qtechstorage", ConfigurationManager.AppSettings["qtechstorage"].ToString());
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

                FileStream fileStream = File.OpenRead(Path.GetFullPath(_filename));
                blobClient.Upload(fileStream, true);
                fileStream.Close();
                _imageChanged = true;

                return blobUri.ToString();
            }

            return string.Empty;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => CargarUsuario();

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(txbEmail.Text))
            {
                MessageBox.Show("El campo correo no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txbEmail.Focus();
                return false;
            }
            return true;
        }

        private void txbTelephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0]) && e.Text[0] != ' ' && e.Text[0] != '\b' && e.Text[0] != '\t' || (txbTelephone.Text + e.Text).Length > 12)
            {
                e.Handled = true;
            }
        }

        private void imgProfilePic_MouseDown(object sender, MouseButtonEventArgs e)
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

                imgProfilePic.Source = bitmap;
            }
        }

        private async void btnAddTerra_Click(object sender, RoutedEventArgs e)
        {
            Terrario terra = new Terrario();

            FrmTerraMaker terraMaker = new FrmTerraMaker(_usuario, terra);

            if (terraMaker.ShowDialog() == true)
            {
                await Herramientas.CreateTerrario(terra);
                await Herramientas.UpdateEspeciesOfTerrario(terra.Id, terraMaker.EspeciesTerrario);

                CargarTerrarios();
            }
        }

        private async void btnDelTerra_Click(object sender, RoutedEventArgs e)
        {
            if (lvListaTerrarios.SelectedItem != null)
            {
                long id = (long)((ListViewItem)lvListaTerrarios.SelectedItem).Tag;

                MessageBoxResult result = MessageBox.Show("¿Estás seguro de querer borrar este terrario?", "Aviso", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await Herramientas.DeleteTerrario(id);
                    CargarTerrarios();
                }
            }
        }
    }
}
