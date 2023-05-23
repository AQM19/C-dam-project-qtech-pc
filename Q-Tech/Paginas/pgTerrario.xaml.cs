using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para pgTerrario.xaml
    /// </summary>
    public partial class pgTerrario : Page
    {
        private Terrario _terrario;
        private Timer _timer;
        private Visita _visita;
        private long _idUsuario;
        public pgTerrario()
        {
            InitializeComponent();
        }
        public pgTerrario(Terrario terrario, long idUsuario) : this()
        {
            _terrario = terrario;
            _idUsuario = idUsuario;
            DesplegarInfo();
        }

        private async Task ObtenerValoresTerrario()
        {

            Lectura lectura = await Herramientas.GetLecturaActual(_terrario.Id);

            if (lectura != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    charTemp.Value = lectura.Temperatura;
                    chartHumid.Value = lectura.Humedad;
                });
            }
        }

        private void DesplegarInfo()
        {
            imgTerraPic.Source = new BitmapImage(new Uri(_terrario.Foto != null ? _terrario.Foto : "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute));
            txbTerraName.Text = _terrario.Nombre;
            txbTerraDescription.Text = _terrario.Descripcion;

            _timer = new Timer(async (_) => await ObtenerValoresTerrario(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

            CargarComentarios();
            CargarEspecies();
        }

        private async void CargarComentarios()
        {
            lvComentsList.Items.Clear();

            List<Visita> visitas = await Herramientas.GetVisitas(_terrario.Id);

            foreach (Visita v in visitas)
            {
                if (!string.IsNullOrEmpty(v.Comentario))
                {
                    Usuario user = await Herramientas.GetUsuario(v.Idusuario);

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
                        Source = new BitmapImage(new Uri(user.FotoPerfil, UriKind.RelativeOrAbsolute)),
                        Width = 75
                    };
                    Grid.SetColumn(image, 0);

                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical
                    };
                    Grid.SetColumn(stackPanel, 1);

                    var textBlock1 = new TextBlock
                    {
                        Text = user.NombreUsuario,
                        Padding = new Thickness(5),
                        FontSize = 16,
                        FontWeight = FontWeights.DemiBold
                    };

                    var textBlock2 = new TextBlock
                    {
                        Text = v.Comentario,
                        TextWrapping = TextWrapping.Wrap,
                        Padding = new Thickness(5)
                    };

                    var textBlock3 = new TextBlock
                    {
                        Text = v.Fecha.ToShortDateString(),
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
                }
            }
        }

        private async void CargarEspecies()
        {
            lvSpList.Items.Clear();
            List<Especie> list = await Herramientas.GetEspeciesTerrario(_terrario.Id);

            foreach (Especie e in list)
            {
                ListViewItem listViewItem = new ListViewItem
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Cursor = Cursors.Hand
                };

                Grid grid = new Grid();

                ColumnDefinition columnDefinition1 = new ColumnDefinition
                {
                    Width = new GridLength(0.3, GridUnitType.Star)
                };
                ColumnDefinition columnDefinition2 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(e.Imagen, UriKind.RelativeOrAbsolute)),
                };
                Grid.SetColumn(image, 0);

                TextBlock textBlock = new TextBlock
                {
                    Text = $"{e.Genero} {e.Sp}",
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(textBlock, 1);

                grid.Children.Add(image);
                grid.Children.Add(textBlock);

                listViewItem.Content = grid;

                lvSpList.Items.Add(listViewItem);
            }
        }

        private void AddComment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lvComentsList.Visibility = Visibility.Collapsed;
            AddComment.Visibility = Visibility.Collapsed;
            TxbComment.Visibility = Visibility.Visible;
            SaveComment.Visibility = Visibility.Visible;
        }

        private void SaveComment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lvComentsList.Visibility = Visibility.Visible;
            AddComment.Visibility = Visibility.Visible;
            TxbComment.Visibility = Visibility.Collapsed;
            SaveComment.Visibility = Visibility.Collapsed;

            _visita.Comentario = TxbComment.Text;

            // crear visita completa

            CargarComentarios();
        }
    }
}
