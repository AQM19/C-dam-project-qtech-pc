using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Q_Tech.Paginas
{
    public partial class pgUsuarios : Page
    {
        public pgUsuarios()
        {
            InitializeComponent();
            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            List<Usuario> usuarios = await Herramientas.GetUsuarios();

            for (int i = 0; i < usuarios.Count; i++) // Se usa el bucle for por eficiencia
            {
                
                ListViewItem listViewItem = new ListViewItem();
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 50, Margin = new Thickness(10) };
                ImageBrush imageBrush = new ImageBrush();
                if (!string.IsNullOrEmpty(usuarios[i].FotoPerfil)) imageBrush.ImageSource = new BitmapImage(new Uri(usuarios[i].FotoPerfil));
                Border border = new Border { CornerRadius = new CornerRadius(5), BorderThickness = new Thickness(0), Height = 50, Width = 50, Margin = new Thickness(0, 0, 10, 0), Background = imageBrush};
                StackPanel textStackPanel = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0), Width = 150 };
                TextBlock nameTextBlock = new TextBlock { Text = usuarios[i].NombreUsuario };
                TextBlock fullNameTextBlock = new TextBlock { Text = $"{usuarios[i].Nombre} {usuarios[i].Apellido1} {usuarios[i].Apellido2}" };
                Border followBorder = new Border{ Margin = new Thickness(10), CornerRadius = new CornerRadius(5), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, Background = new SolidColorBrush(Color.FromRgb(52, 168, 83)), BorderThickness = new Thickness(0), Cursor = Cursors.Hand };
                TextBlock followTextBlock = new TextBlock { Text = "Seguir", Padding = new Thickness(10, 5, 10, 5), FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Segoe UI"), Foreground = new SolidColorBrush(Colors.White) };

                textStackPanel.Children.Add(nameTextBlock);
                textStackPanel.Children.Add(fullNameTextBlock);

                followBorder.Child = followTextBlock;

                stackPanel.Children.Add(border);
                stackPanel.Children.Add(textStackPanel);
                stackPanel.Children.Add(followBorder);

                listViewItem.Content = stackPanel;
                lvUsuarios.Items.Add(listViewItem);
                
            }
        }
    }
}
