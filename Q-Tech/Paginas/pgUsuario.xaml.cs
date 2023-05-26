using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgUsuario.xaml
    /// </summary>
    public partial class PgUsuario : Page
    {
        private readonly long _id;
        private bool _follow;
        private readonly Usuario _usuario;
        private readonly Frame _mainFrame;

        public PgUsuario()
        {
            InitializeComponent();
        }
        public PgUsuario(long id, Usuario usuario, Frame mainFrame) : this()
        {
            _usuario = usuario;
            _mainFrame = mainFrame;
            _id = id;
            DesplegarInformacion();
        }

        private async void DesplegarInformacion()
        {
            imgUserPic.Source = new BitmapImage(new Uri(_usuario.FotoPerfil ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute));
            txbUserName.Text = _usuario.NombreUsuario;
            txbUserTimeline.Text = $"{_usuario.Nombre} {_usuario.Apellido1} {_usuario.Apellido2}";

            CargarFollow();
            CargarTerrarios();
        }

        private async void CargarTerrarios()
        {
            lvListaTerrarios.Items.Clear();

            List<Terrario> terrarios = await Herramientas.GetTerrariosUsuario(_usuario.Id);

            if (terrarios.Count > 0)
            {
                foreach (Terrario t in terrarios)
                {
                    ListViewItem listViewItem = new ListViewItem
                    {
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    Grid grid = new Grid();

                    ColumnDefinition columnDefinition1 = new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Auto)
                    };
                    ColumnDefinition columnDefinition2 = new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    };

                    grid.ColumnDefinitions.Add(columnDefinition1);
                    grid.ColumnDefinitions.Add(columnDefinition2);

                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri(t.Foto ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute)),
                        Width = 75
                    };
                    Grid.SetColumn(image, 0);

                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical
                    };
                    Grid.SetColumn(stackPanel, 1);

                    TextBlock textBlock1 = new TextBlock
                    {
                        Text = t.Nombre,
                        Padding = new Thickness(5),
                        FontSize = 16,
                        FontWeight = FontWeights.DemiBold
                    };

                    TextBlock textBlock2 = new TextBlock
                    {
                        Text = t.Descripcion,
                        TextWrapping = TextWrapping.Wrap,
                        Padding = new Thickness(5)
                    };

                    TextBlock textBlock3 = new TextBlock
                    {
                        Text = t.FechaCreacion.ToShortDateString(),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        FontWeight = FontWeights.Light,
                        Padding = new Thickness(3)
                    };

                    stackPanel.Children.Add(textBlock1);
                    stackPanel.Children.Add(textBlock2);
                    stackPanel.Children.Add(textBlock3);

                    grid.Children.Add(image);
                    grid.Children.Add(stackPanel);

                    listViewItem.Content = grid;

                    listViewItem.MouseDoubleClick += (sender, e) =>
                    {
                        PgTerrario pgTerrario = new PgTerrario(t, _usuario.Id);
                        _mainFrame.Content = pgTerrario;
                    };

                    lvListaTerrarios.Items.Add(listViewItem);
                }
            }
        }

        private async void CargarFollow()
        {
            _follow = await Herramientas.ComprobarSeguimiento(_id, _usuario.Id);

            btnFollow.Visibility = _follow == true ? Visibility.Collapsed : Visibility.Visible;
            BtnUnfollow.Visibility = _follow == false ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void btnFollow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UsuarioUsuario follow = new UsuarioUsuario
            {
                Idusuario = _id,
                Idcontacto = _usuario.Id,
                FechaContacto = DateTime.Now
            };

            await Herramientas.FollowUser(follow);
            Thread.Sleep(1000);
            CargarFollow();
        }

        private async void BtnUnfollow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await Herramientas.UnfollowUser(_id, _usuario.Id);
            Thread.Sleep(1000);
            CargarFollow();
        }
    }
}
