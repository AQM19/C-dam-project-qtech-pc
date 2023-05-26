using BusinessLogic;
using Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgListaNotificaciones.xaml
    /// </summary>
    public partial class PgListaNotificaciones : Page
    {
        private readonly long _id;
        private readonly List<Notificacion> _notificaciones;

        public List<Notificacion> Notificaciones { get; }

        public PgListaNotificaciones()
        {
            InitializeComponent();
        }
        public PgListaNotificaciones(long id) : this()
        {
            this._id = id;
            this._notificaciones = new List<Notificacion>();
            CargarNotificaciones();
        }

        private async void CargarNotificaciones()
        {
            List<Notificacion> notificaciones = await Herramientas.GetNotificacionesByUserId(_id);

            if(notificaciones.Count== 0) brMessage.Visibility = Visibility.Visible;

            for (int i = 0; i < notificaciones.Count; i++)
            {
                Grid grid = new Grid();

                ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) };
                ColumnDefinition col2 = new ColumnDefinition { Width = new GridLength(200, GridUnitType.Pixel) };

                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);

                TextBlock textBlock1 = new TextBlock
                {
                    Text = notificaciones[i].Texto,
                    Padding = new Thickness(5)
                };

                TextBlock textBlock2 = new TextBlock
                {
                    Text = notificaciones[i].Fecha.ToShortDateString(),
                    Padding = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Grid.SetColumn(textBlock1, 0);
                Grid.SetColumn(textBlock2, 1);

                grid.Children.Add(textBlock1);
                grid.Children.Add(textBlock2);

                ListViewItem item = new ListViewItem
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Content = grid,
                    Background = notificaciones[i].Vista == 1 ? new SolidColorBrush(Color.FromRgb(0x59, 0xD9, 0x6B)) : new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36))
                };

                int index = i;

                item.MouseEnter += async (sender, e) =>
                {
                    item.Background = new SolidColorBrush(Color.FromRgb(0x59, 0xD9, 0x6B));
                    notificaciones[index].Vista = 1;
                    await Herramientas.UpdateNotificacion(notificaciones[index], notificaciones[index].Id);
                };

                lvNotificaciones.Items.Add(item);
            }
        }
    }
}
