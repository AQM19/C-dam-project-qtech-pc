using BusinessLogic;
using Entities;
using Q_Tech.Prop;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgTerrarios.xaml
    /// </summary>
    public partial class pgListaTerrarios : Page
    {

        private long _id;
        public pgListaTerrarios()
        {
            InitializeComponent();
        }

        public pgListaTerrarios(long id) : this()
        {
            _id = id;
            CargarTerrarios();
        }

        private async void CargarTerrarios()
        {
            List<Terrario> terrarios = await Herramientas.GetTerrariosSocial(_id);

            for (int i = 0; i < terrarios.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                ImageBrush imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri($"{terrarios[i].Foto}"))
                };

                Border imageBorder = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(0),
                    Width = 50,
                    Height = 50,
                    Background = imageBrush
                };

                stackPanel.Children.Add(imageBorder);

                StackPanel infoStackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(20, 0, 20, 0),
                    Width = 250
                };

                TextBlock titleTextBlock = new TextBlock
                {
                    Text = $"{terrarios[i].Nombre}"
                };


                infoStackPanel.Children.Add(titleTextBlock);

                TextBlock descriptionTextBlock = new TextBlock
                {
                    Text = $"{terrarios[i].Descripcion}",
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    MaxWidth = 250,

                };

                infoStackPanel.Children.Add(descriptionTextBlock);

                DockPanel dockPanel = new DockPanel
                {
                    LastChildFill = false
                };

                TextBlock ratingTextBlock = new TextBlock
                {
                    Text = $"{await Herramientas.GetPuntuacionTerrario(terrarios[i].Id)}"
                };

                DockPanel.SetDock(ratingTextBlock, Dock.Left);
                dockPanel.Children.Add(ratingTextBlock);

                Image ratingImage = new Image
                {
                    Source = new BitmapImage(new Uri("/Recursos/Iconos/star.png", UriKind.Relative)),
                    Width = 16,
                    Margin = new Thickness(5, 0, 15, 0)
                };

                DockPanel.SetDock(ratingImage, Dock.Left);
                dockPanel.Children.Add(ratingImage);

                TextBlock dateTextBlock = new TextBlock
                {
                    Text = $"{terrarios[i].FechaCreacion.ToShortDateString()}"
                };

                DockPanel.SetDock(dateTextBlock, Dock.Right);
                dockPanel.Children.Add(dateTextBlock);

                infoStackPanel.Children.Add(dockPanel);
                stackPanel.Children.Add(infoStackPanel);

                Border exportBorder = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(0),
                    Background = Brushes.GhostWhite,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Padding = new Thickness(5),
                    Cursor = Cursors.Hand
                };

                Image exportImage = new Image
                {
                    Source = new BitmapImage(new Uri("/Recursos/Iconos/exportar.png", UriKind.Relative)),
                    Width = 20
                };

                exportBorder.AddHandler(ContentElement.MouseDownEvent, new MouseButtonEventHandler(Border_MouseDown));

                exportBorder.Child = exportImage;
                stackPanel.Children.Add(exportBorder);
                item.Content = stackPanel;
                item.Tag = terrarios[i].Id;

                lvTerrarios.Items.Add(item);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border exportBorder = (Border)sender;
            StackPanel panel = (StackPanel)exportBorder.Parent;
            ListViewItem item = (ListViewItem)panel.Parent;

            long terrarioId = (long)item.Tag;

            MessageBox.Show(terrarioId.ToString());
        }
    }
}
