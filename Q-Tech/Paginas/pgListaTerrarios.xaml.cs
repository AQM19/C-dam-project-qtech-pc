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
    public partial class PgListaTerrarios : Page
    {

        private readonly long _id;
        private readonly Frame _mainFrame;
        public PgListaTerrarios()
        {
            InitializeComponent();
        }

        public PgListaTerrarios(long id, Frame mainFrame) : this()
        {
            _id = id;
            _mainFrame = mainFrame;
            CargarTerrarios();
        }

        private async void CargarTerrarios()
        {
            List<Terrario> terrarios = await Herramientas.GetTerrariosSocial(_id);

            foreach (Terrario t in terrarios)
            {
                ListViewItem item = new ListViewItem();
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(string.IsNullOrEmpty(t.Foto) ? "/Recursos/Iconos/MainIcon.png" : t.Foto, UriKind.RelativeOrAbsolute))
                };

                Border imageBorder = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    BorderThickness = new Thickness(0),
                    Width = 50,
                    Height = 50,
                    Child = image
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
                    Text = t.Nombre
                };


                infoStackPanel.Children.Add(titleTextBlock);

                TextBlock descriptionTextBlock = new TextBlock
                {
                    Text = t.Descripcion,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    MaxWidth = 250,

                };

                infoStackPanel.Children.Add(descriptionTextBlock);

                DockPanel dockPanel = new DockPanel
                {
                    LastChildFill = false
                };

                TextBlock dateTextBlock = new TextBlock
                {
                    Text = t.FechaCreacion.ToShortDateString()
                };

                DockPanel.SetDock(dateTextBlock, Dock.Right);
                dockPanel.Children.Add(dateTextBlock);

                infoStackPanel.Children.Add(dockPanel);
                stackPanel.Children.Add(infoStackPanel);

                item.Content = stackPanel;
                item.Cursor = Cursors.Hand;
                item.Tag = t.Id;

                item.MouseDoubleClick += (sender, e) =>
                {
                    PgTerrario pgTerrario = new PgTerrario(t, _id);
                    _mainFrame.Content = pgTerrario;
                };

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
