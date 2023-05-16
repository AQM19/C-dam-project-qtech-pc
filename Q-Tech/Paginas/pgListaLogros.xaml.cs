using BusinessLogic;
using Entities;
using Q_Tech.Prop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgLogros.xaml
    /// </summary>
    public partial class pgListaLogros : Page
    {
        private readonly List<Logro> cambiosPendientes = new List<Logro>();

        public pgListaLogros()
        {
            InitializeComponent();
            CargarLogros();
        }

        private async void CargarLogros()
        {
            lvLogros.Items.Clear();
            List<Logro> logros = await Herramientas.GetLogros();

            foreach (Logro logro in logros)
            {
                ListViewItem listViewItem = new ListViewItem();

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(350, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                Image image = new Image
                {
                    Width = 50,
                    Margin = new Thickness(5),
                    Source = new BitmapImage(new Uri(logro.Icono ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute))
                };

                Grid.SetColumn(image, 0);
                Grid.SetRow(image, 0);
                Grid.SetRowSpan(image, 2);

                TextBlock logroTextBlock = new TextBlock
                {
                    Text = logro.Titulo,
                    Padding = new Thickness(5)

                };
                Grid.SetColumn(logroTextBlock, 1);
                Grid.SetRow(logroTextBlock, 0);

                TextBlock descripcionTextBlock = new TextBlock
                {
                    Text = logro.Descripcion,
                    Padding = new Thickness(5),
                    TextWrapping = TextWrapping.Wrap,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                Grid.SetColumn(descripcionTextBlock, 1);
                Grid.SetRow(descripcionTextBlock, 1);

                DatePicker dtpFechaDesde = new DatePicker
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5),
                    SelectedDate = logro.Fechadesde
                };
                dtpFechaDesde.SelectedDateChanged += (sender, e) => ActualizarFechaLogro(sender, logro, "Fechadesde");
                Grid.SetColumn(dtpFechaDesde, 2);
                Grid.SetRow(dtpFechaDesde, 0);
                Grid.SetRowSpan(dtpFechaDesde, 2);

                DatePicker dtpFechaHasta = new DatePicker
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5),
                    SelectedDate = logro.Fechahasta

                };
                dtpFechaHasta.SelectedDateChanged += (sender, e) => ActualizarFechaLogro(sender, logro, "Fechahasta");
                Grid.SetColumn(dtpFechaHasta, 3);
                Grid.SetRow(dtpFechaHasta, 0);
                Grid.SetRowSpan(dtpFechaHasta, 2);

                grid.Children.Add(image);
                grid.Children.Add(logroTextBlock);
                grid.Children.Add(descripcionTextBlock);
                grid.Children.Add(dtpFechaDesde);
                grid.Children.Add(dtpFechaHasta);

                listViewItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                listViewItem.Content = grid;
                listViewItem.MouseDoubleClick += (sender, e) => UpdateLogro(logro);

                lvLogros.Items.Add(listViewItem);
            }
        }

        private void ActualizarFechaLogro(object sender, Logro logro, string param)
        {
            DatePicker dtp = (DatePicker)sender;

            if (dtp.SelectedDate.HasValue)
            {
                typeof(Logro).GetProperty(param)?.SetValue(logro, dtp.SelectedDate.Value);
            }
            else
            {
                typeof(Logro).GetProperty(param)?.SetValue(logro, null);
            }

            cambiosPendientes.Add(logro);
        }

        private async void SaveLogros_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (cambiosPendientes.Count > 0)
            {
                foreach (Logro logro in cambiosPendientes)
                {
                    if (logro.Fechadesde.HasValue && logro.Fechahasta.HasValue && logro.Fechadesde > logro.Fechahasta)
                    {
                        MessageBox.Show("No se puede poner la fecha de obtención mayor a la fecha de caducidad del logro.");
                        cambiosPendientes.Remove(logro);
                        return;
                    }

                    await Herramientas.UpdateLogro(logro);
                }

                cambiosPendientes.Clear();
            }
        }

        private async void AddLogro_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Logro logro = new Logro();
            frmAddLogro addLogro = new frmAddLogro(logro);

            if (addLogro.ShowDialog() == true)
            {
                await Herramientas.CreateLogro(logro);
                CargarLogros();
            }
        }

        private async void UpdateLogro(Logro logro)
        {
            frmAddLogro addLogro = new frmAddLogro(logro);

            if (addLogro.ShowDialog() == true)
            {
                await Herramientas.UpdateLogro(logro);
                CargarLogros();
            }
        }
    }
}
