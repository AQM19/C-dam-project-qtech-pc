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
    public partial class PgListaUsuarios : Page
    {
        private readonly Usuario _usuario;
        private readonly Frame _mainFrame;

        public PgListaUsuarios()
        {
            InitializeComponent();

        }
        public PgListaUsuarios(Usuario usuario, Frame mainFrame) : this()
        {
            _usuario = usuario;
            _mainFrame = mainFrame;
            CargarUsuarios();
        }

        private async void FollowUser(Usuario u)
        {
            UsuarioUsuario follow = new UsuarioUsuario
            {
                Idusuario = _usuario.Id,
                Idcontacto = u.Id,
                FechaContacto = DateTime.Now
            };

            await Herramientas.FollowUser(follow);
            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            lvUsuarios.Items.Clear();

            List<Usuario> usuarios = _usuario.Perfil == "ADMIN" ? await Herramientas.GetUsuarios() : await Herramientas.GetSocial(_usuario.Id);

            if (usuarios.Count > 0)
            {
                foreach (Usuario u in usuarios)
                {
                    ListViewItem listViewItem = new ListViewItem();

                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Height = 50,
                        Margin = new Thickness(10)
                    };

                    ImageBrush imageBrush = new ImageBrush();

                    if (!string.IsNullOrEmpty(u.FotoPerfil))
                        imageBrush.ImageSource = new BitmapImage(new Uri(u.FotoPerfil));

                    Border border = new Border
                    {
                        CornerRadius = new CornerRadius(5),
                        BorderThickness = new Thickness(0),
                        Height = 50,
                        Width = 50,
                        Margin = new Thickness(0, 0, 10, 0),
                        Background = imageBrush
                    };

                    StackPanel textStackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0),
                        Width = 150
                    };

                    TextBlock nameTextBlock = new TextBlock
                    {
                        Text = u.NombreUsuario
                    };

                    TextBlock fullNameTextBlock = new TextBlock
                    {
                        Text = $"{u.Nombre} {u.Apellido1} {u.Apellido2}"
                    };

                    textStackPanel.Children.Add(nameTextBlock);
                    textStackPanel.Children.Add(fullNameTextBlock);


                    stackPanel.Children.Add(border);
                    stackPanel.Children.Add(textStackPanel);

                    if (_usuario.Perfil != "ADMIN")
                    {
                        Border followBorder = new Border
                        {
                            Margin = new Thickness(10),
                            CornerRadius = new CornerRadius(5),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Background = new SolidColorBrush(Color.FromRgb(52, 168, 83)),
                            BorderThickness = new Thickness(0),
                            Cursor = Cursors.Hand
                        };

                        followBorder.MouseDown += (sender, e) => FollowUser(u);

                        TextBlock followTextBlock = new TextBlock
                        {
                            Text = "Seguir",
                            Padding = new Thickness(10, 5, 10, 5),
                            FontWeight = FontWeights.Bold,
                            FontFamily = new FontFamily("Segoe UI"),
                            Foreground = new SolidColorBrush(Colors.White)
                        };

                        followBorder.Child = followTextBlock;
                        stackPanel.Children.Add(followBorder);
                    }

                    listViewItem.MouseDoubleClick += (sender, e) =>
                    {
                        PgUsuario pgUsuario = new PgUsuario(_usuario.Id, u, _mainFrame);
                        _mainFrame.Content = pgUsuario;
                    };

                    listViewItem.Content = stackPanel;
                    lvUsuarios.Items.Add(listViewItem);
                }
            }
        }
    }
}
