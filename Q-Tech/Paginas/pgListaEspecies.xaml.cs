using BusinessLogic;
using Entities;
using Q_Tech.Prop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Orientation = System.Windows.Controls.Orientation;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgListaEspecies.xaml
    /// </summary>
    public partial class PgListaEspecies : Page
    {
        public PgListaEspecies()
        {
            InitializeComponent();
            CargarEspecies();
        }

        private async void brdAniadirEspecie_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Especie sp = new Especie();
            FrmAddEspecie addEspecie = new FrmAddEspecie(sp);
            if(addEspecie.ShowDialog() == true)
            {
                await Herramientas.AddEspecie(sp);
                CargarEspecies();
            }
        }

        private async void CargarEspecies()
        {
            lvEspecies.Items.Clear();
            List<Especie> especies = await Herramientas.GetEspecies();

            foreach (Especie e in especies)
            {
                long index = e.Id;
                ListViewItem listViewItem = new ListViewItem
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                // Crear Grid
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.35, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(300, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                // Crear Border
                Border border = new Border
                {
                    Width = 50,
                    Height = 50
                };
                ImageBrush imageBrush = new ImageBrush();
                if (e.Imagen != null) imageBrush.ImageSource = new BitmapImage(new Uri(e.Imagen, UriKind.Absolute));
                border.Background = imageBrush;
                Grid.SetColumn(border, 0);
                grid.Children.Add(border);

                // Crear StackPanel
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5)
                };
                Grid.SetColumn(stackPanel, 1);
                grid.Children.Add(stackPanel);

                // Crear TextBlocks
                TextBlock textBlock1 = new TextBlock
                {
                    Text = $"{e.Genero} {e.Sp}",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.WordEllipsis,
                    TextWrapping = TextWrapping.Wrap
                };
                stackPanel.Children.Add(textBlock1);

                TextBlock textBlock2 = new TextBlock
                {
                    Text = e.NombreComun
                };
                stackPanel.Children.Add(textBlock2);

                // Crear TextBlock
                TextBlock textBlock3 = new TextBlock
                {
                    Text = e.Descripcion,
                    Padding = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap,
                    TextTrimming = TextTrimming.WordEllipsis
                };
                Grid.SetColumn(textBlock3, 2);
                grid.Children.Add(textBlock3);

                // Crear StackPanel
                StackPanel stackPanel2 = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(stackPanel2, 3);
                grid.Children.Add(stackPanel2);

                // Crear Borders
                Border border2 = new Border
                {
                    Width = 32,
                    Height = 32,
                    Background = new SolidColorBrush(Color.FromRgb(247, 189, 86)),
                    Padding = new Thickness(3),
                    Margin = new Thickness(5, 0, 5, 0),
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Cursor = Cursors.Hand
                };
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri("/Recursos/Iconos/editar.png", UriKind.Relative))
                };
                border2.Child = image;
                border2.MouseDown += async (sender, ev) =>
                {
                    FrmAddEspecie frmAdd = new FrmAddEspecie(e);
                    if(frmAdd.ShowDialog() == true)
                    {
                        await Herramientas.UpdateEspecie(e.Id, e);
                        CargarEspecies();
                    }
                };
                stackPanel2.Children.Add(border2);

                Border border3 = new Border
                {
                    Width = 32,
                    Height = 32,
                    Background = new SolidColorBrush(Color.FromRgb(255, 105, 97)),
                    Padding = new Thickness(3),
                    Margin = new Thickness(5, 0, 5, 0),
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Cursor = Cursors.Hand
                };
                Image image2 = new Image
                {
                    Source = new BitmapImage(new Uri("/Recursos/Iconos/cerrar.png", UriKind.Relative))
                };
                border3.Child = image2;
                border3.MouseDown += async (sender, ev) =>
                {
                    await Herramientas.DeleteEspecie(index);
                    CargarEspecies();
                };
                stackPanel2.Children.Add(border3);

                // Agregar Grid al ListViewItem
                listViewItem.Content = grid;

                // Agregar ListViewItem al ListView
                lvEspecies.Items.Add(listViewItem);
            }
        }
    }
}
