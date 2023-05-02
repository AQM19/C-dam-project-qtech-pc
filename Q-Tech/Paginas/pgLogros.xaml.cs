using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgLogros.xaml
    /// </summary>
    public partial class pgLogros : Page
    {
        private List<Logro> cambiosPendientes = new List<Logro>();

        public pgLogros()
        {
            InitializeComponent();
            CargarLogros();
        }

        private async void CargarLogros()
        {
            List<Logro> logros = await Herramientas.GetLogros();

            for (int i = 0; i < logros.Count; i++)
            {
                ListViewItem newItem = new ListViewItem();
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 50, Margin = new Thickness(10) };
                ImageBrush imageBrush = new ImageBrush { ImageSource = new BitmapImage(new Uri($"{logros[i].Icono}", UriKind.Absolute)), };
                Border border = new Border { CornerRadius = new CornerRadius(5), BorderThickness = new Thickness(0), Height = 50, Width = 50, Margin = new Thickness(0, 0, 10, 0), Background = imageBrush };
                StackPanel secondStackPanel = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0), Width = 250 };
                TextBlock logroTextBlock = new TextBlock { Text = $"{logros[i].Titulo}" };
                TextBlock descripcionTextBlock = new TextBlock { Text = $"{logros[i].Descripcion}" };
                ComboBox comboBox = new ComboBox { VerticalContentAlignment = VerticalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0), Tag = logros[i] };
                ComboBoxItem deshabilitadoItem = new ComboBoxItem { Content = "Deshabilitado", IsSelected = logros[i].Disponible == 1 ? false : true };
                ComboBoxItem habilitadoItem = new ComboBoxItem { Content = "Habilitado", IsSelected = logros[i].Disponible == 0 ? false : true };

                stackPanel.Children.Add(border);
                secondStackPanel.Children.Add(logroTextBlock);
                secondStackPanel.Children.Add(descripcionTextBlock);
                stackPanel.Children.Add(secondStackPanel);
                comboBox.Items.Add(deshabilitadoItem);
                comboBox.Items.Add(habilitadoItem);
                stackPanel.Children.Add(comboBox);
                newItem.Content = stackPanel;
                lvLogros.Items.Add(newItem);

                comboBox.SelectionChanged += ComboBox_SelectionChanged;
            }
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem deshabilitadoItem = (ComboBoxItem)comboBox.Items[0];
            ComboBoxItem habilitadoItem = (ComboBoxItem)comboBox.Items[1];

            Logro logro = (Logro)comboBox.Tag;

            logro.Disponible = deshabilitadoItem.IsSelected ? (sbyte)0 : (sbyte)1;

            if (!cambiosPendientes.Contains(logro))
            {
                cambiosPendientes.Add(logro);
            }
        }

        private async void SaveLogros_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (Logro logro in cambiosPendientes)
            {
                Herramientas.UpdateLogro(logro);
            }

            cambiosPendientes.Clear();
        }
    }
}
