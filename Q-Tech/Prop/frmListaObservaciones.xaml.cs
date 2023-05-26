using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
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

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmListaObservaciones.xaml
    /// </summary>
    public partial class FrmListaObservaciones : Window
    {
        private readonly long _id;
        public FrmListaObservaciones()
        {
            InitializeComponent();
        }
        public FrmListaObservaciones(long id) : this()
        {
            _id = id;
            DesplegarInformacion();
        }

        private async void DesplegarInformacion()
        {
            lvObservaciones.Items.Clear();

            List<Observacion> observaciones = await Herramientas.GetObservacionesByTerra(_id);

            foreach (Observacion o in observaciones)
            {
                ListViewItem listViewItem = new ListViewItem
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Gray
                };

                Grid grid = new Grid();

                ColumnDefinition columnDefinition1 = new ColumnDefinition
                {
                    Width = new GridLength(100, GridUnitType.Pixel)
                };
                ColumnDefinition columnDefinition2 = new ColumnDefinition
                {
                    Width = new GridLength(0.025, GridUnitType.Star)
                };
                ColumnDefinition columnDefinition3 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(columnDefinition1);
                grid.ColumnDefinitions.Add(columnDefinition2);
                grid.ColumnDefinitions.Add(columnDefinition3);

                TextBlock textBlock1 = new TextBlock
                {
                    Text = o.Fecha.ToShortDateString(),
                    Padding = new Thickness(5),
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(textBlock1, 0);

                Separator separator = new Separator();
                Grid.SetColumn(separator, 1);

                TextBlock textBlock2 = new TextBlock
                {
                    Text = o.Texto,
                    Padding = new Thickness(5),
                    Margin = new Thickness(5)
                };
                Grid.SetColumn(textBlock2, 2);

                grid.Children.Add(textBlock1);
                grid.Children.Add(separator);
                grid.Children.Add(textBlock2);

                listViewItem.Content = grid;

                listViewItem.MouseDoubleClick += (sender, e) => ActualizarObservacion(o);

                lvObservaciones.Items.Add(listViewItem);
            }
        }

        private async void AddObservacion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Observacion observacion = new Observacion();

            FrmAddObservacion frmAddObservacion = new FrmAddObservacion(_id, observacion);
            if(frmAddObservacion.ShowDialog() == true)
            {
                await Herramientas.CreateObservacion(observacion);
                DesplegarInformacion();
            }
        }

        private async void ActualizarObservacion(Observacion obs)
        {
            FrmAddObservacion frmAddObservacion = new FrmAddObservacion(_id, obs);
            if (frmAddObservacion.ShowDialog() == true)
            {
                await Herramientas.UpdateObservacion(obs.Id, obs);
                DesplegarInformacion();
            }
        }
    }
}
