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

                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    Border border = new Border
                    {
                        CornerRadius = new CornerRadius(5),
                        BorderThickness = new Thickness(0),
                        Height = 100,
                        Width = 100
                    };

                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = new BitmapImage(new Uri(u.FotoPerfil));

                    border.Background = imageBrush;
                    Grid.SetColumn(border, 0);
                    grid.Children.Add(border);

                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(5)
                    };

                    Grid.SetColumn(stackPanel, 1);
                    grid.Children.Add(stackPanel);

                    TextBlock usernameTextBlock = new TextBlock
                    {
                        Text = u.NombreUsuario,
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        TextTrimming = TextTrimming.WordEllipsis,
                        TextWrapping = TextWrapping.Wrap
                    };

                    stackPanel.Children.Add(usernameTextBlock);

                    TextBlock fullNameTextBlock = new TextBlock
                    {
                        Text = $"{u.Nombre} {u.Apellido1} {u.Apellido2}",
                        TextWrapping = TextWrapping.Wrap
                    };

                    stackPanel.Children.Add(fullNameTextBlock);


                    ComboBox comboBox = new ComboBox
                    {
                        Width = 120,
                        Padding = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    ComboBoxItem adminItem = new ComboBoxItem
                    {
                        Content = "Administrador",
                        Tag = "ADMIN",
                        IsSelected = u.Perfil.Equals("ADMIN")
                    };

                    ComboBoxItem clienteItem = new ComboBoxItem
                    {
                        Content = "Cliente",
                        Tag = "CLIENTE",
                        IsSelected = u.Perfil.Equals("CLIENTE")
                    };

                    comboBox.Items.Add(adminItem);
                    comboBox.Items.Add(clienteItem);
                    comboBox.Visibility = _usuario.Perfil.Equals("ADMIN") ? Visibility.Visible : Visibility.Hidden;

                    comboBox.SelectionChanged += (sender, ev) => CambiarRol(u, sender);

                    Grid.SetColumn(comboBox, 2);
                    grid.Children.Add(comboBox);

                    if (u.Id != _usuario.Id)
                    {
                        bool follow = await Herramientas.ComprobarSeguimiento(_usuario.Id, u.Id);

                        if (follow == false)
                        {
                            Border followBorder = new Border
                            {
                                Margin = new Thickness(10),
                                CornerRadius = new CornerRadius(5),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Background = new SolidColorBrush(Color.FromRgb(52, 168, 83)),
                                BorderThickness = new Thickness(0),
                                Cursor = Cursors.Hand,
                            };

                            TextBlock followTextBlock = new TextBlock
                            {
                                Text = "Seguir",
                                Padding = new Thickness(10),
                                FontWeight = FontWeights.Bold,
                                FontFamily = new FontFamily("Segoe UI"),
                                Foreground = new SolidColorBrush(Colors.White)
                            };

                            followBorder.MouseDown += (sender, e) => FollowUser(u);

                            followBorder.Child = followTextBlock;
                            Grid.SetColumn(followBorder, 3);
                            grid.Children.Add(followBorder);
                        }
                        else
                        {
                            Border unfollowBorder = new Border
                            {
                                Margin = new Thickness(10),
                                CornerRadius = new CornerRadius(5),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Background = new SolidColorBrush(Color.FromRgb(255, 105, 97)),
                                BorderThickness = new Thickness(0),
                                Cursor = Cursors.Hand,
                            };

                            TextBlock unfollowTextBlock = new TextBlock
                            {
                                Text = "No seguir",
                                Padding = new Thickness(10),
                                FontWeight = FontWeights.Bold,
                                FontFamily = new FontFamily("Segoe UI"),
                                Foreground = new SolidColorBrush(Colors.White)
                            };


                            unfollowBorder.MouseDown += (sender, e) => UnfollowUser(u);

                            unfollowBorder.Child = unfollowTextBlock;
                            Grid.SetColumn(unfollowBorder, 3);
                            grid.Children.Add(unfollowBorder);
                        }
                    }

                    listViewItem.Cursor = Cursors.Arrow;
                    listViewItem.Content = grid;
                    lvUsuarios.Items.Add(listViewItem);
                }
            }
        }

        private async void CambiarRol(Usuario userSwappingRole, object sender)
        {
            ComboBox combobox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)combobox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedRole = selectedItem.Tag.ToString();
                userSwappingRole.Perfil = selectedRole;

                await Herramientas.UpdateUsuario(userSwappingRole.Id, userSwappingRole);
                CargarUsuarios();
            }
        }

        private async void UnfollowUser(Usuario unfollowedUser)
        {
            await Herramientas.UnfollowUser(_usuario.Id, unfollowedUser.Id);
            CargarUsuarios();
        }
    }
}
