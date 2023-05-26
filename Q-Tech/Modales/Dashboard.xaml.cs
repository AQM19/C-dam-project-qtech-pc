using BusinessLogic;
using Entities;
using Q_Tech.Paginas;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Q_Tech.Modales
{
    public partial class FrmDashboard : Window
    {

        private readonly Usuario _usuario;
        private string _textValue;
        private readonly DispatcherTimer debounceTimer = new DispatcherTimer();

        public FrmDashboard()
        {
            InitializeComponent();

            debounceTimer.Interval = TimeSpan.FromMilliseconds(500);
            debounceTimer.Tick += DebounceTimer_Tick;
        }

        public FrmDashboard(Usuario usuario) : this()
        {
            _usuario = usuario;

            if(_usuario.Perfil == "CLIENTE")
            {
                btnLogros.Visibility = Visibility.Collapsed;
                btnEspecies.Visibility = Visibility.Collapsed;
            }


            //StartNotificationPolling();
            //NotificationPollingComponent.PropertyChanged += (sender, e) =>
            //{
            //    if (e.PropertyName == nameof(NotificationPollingComponent.PendingNotifications))
            //    {
            //        imgNotificationBell.Source = new BitmapImage(new Uri(NotificationPollingComponent.PendingNotifications
            //            ? "/Recursos/Iconos/notification_yes.png"
            //            : "/Recursos/Iconos/notification_no.png",
            //            UriKind.RelativeOrAbsolute));
            //    }
            //};

            CargarIndex();
        }

        private async void StartNotificationPolling()
        {
            await NotificationPollingComponent.StartPeriodicQuery(_usuario.Id);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            movingImage.Visibility = Visibility.Visible;

            Border clickedButton = (Border)sender;
            Point targetPoint = clickedButton.TranslatePoint(new Point(0, 0), dkpVerticalMenu);

            DoubleAnimation animationY = new DoubleAnimation
            {
                To = targetPoint.Y - 65,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            TranslateTransform transform = (TranslateTransform)movingImage.RenderTransform;

            transform.BeginAnimation(TranslateTransform.YProperty, animationY);

            if (clickedButton.Name.Equals("btnInicio"))
                CargarIndex();
            if (clickedButton.Name.Equals("btnUsuarios"))
                CargarUsuarios();
            if (clickedButton.Name.Equals("btnTerrarios"))
                CargarTerrarios();
            if (clickedButton.Name.Equals("btnLogros"))
                CargarLogros();
            if (clickedButton.Name.Equals("btnEspecies"))
                CargarEspecies();

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtBuscador.Focus();
        }

        private void CargarIndex()
        {
            PgIndex index = new PgIndex(_usuario);
            frmMainFrame.Content = index;
        }

        private void CargarUsuarios()
        {
            PgListaUsuarios pgUsuarios = new PgListaUsuarios(_usuario, frmMainFrame);
            frmMainFrame.Content = pgUsuarios;
        }

        private void CargarTerrarios()
        {
            PgListaTerrarios pgTerrarios = new PgListaTerrarios(_usuario.Id, frmMainFrame);
            frmMainFrame.Content = pgTerrarios;
        }

        private void CargarLogros()
        {
            PgListaLogros pgLogros = new PgListaLogros();
            frmMainFrame.Content = pgLogros;
        }

        private void CargarEspecies()
        {
            PgListaEspecies pgEspecies = new PgListaEspecies();
            frmMainFrame.Content = pgEspecies;
        }

        private void txtBuscador_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textValue = ((TextBox)sender).Text;
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();

            if (string.IsNullOrEmpty(_textValue))
            {
                return;
            }

            List<Usuario> list = await Herramientas.Search(_textValue);

            if (list.Count > 0)
            {
                searchList.Items.Clear();
                searchScroll.Visibility = Visibility.Visible;

                for (int i = 0; i < list.Count; i++)
                {
                    StackPanel stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };

                    Image image1 = new Image
                    {
                        Height = 30,
                        Width = 30,
                        Source = new BitmapImage(new Uri(list[i].FotoPerfil ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute)),
                    };

                    stackPanel.Children.Add(image1);

                    TextBlock textBlock = new TextBlock
                    {
                        Text = list[i].NombreUsuario,
                        Margin = new Thickness(15, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    stackPanel.Children.Add(textBlock);

                    Image image2 = new Image
                    {
                        Height = 28,
                        Width = 28,
                        Source = new BitmapImage(new Uri("/Recursos/Iconos/lupa.png", UriKind.RelativeOrAbsolute)),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(190, 0, 0, 0),

                    };

                    stackPanel.Children.Add(image2);

                    ListViewItem listViewItem = new ListViewItem
                    {
                        Tag = list[i].Id,
                        Content = stackPanel,
                        Cursor = Cursors.Hand
                    };

                    int index = i;
                    listViewItem.MouseDoubleClick += (o, s) =>
                    {
                        PgUsuario pgUsuario = new PgUsuario(_usuario.Id, list[index], frmMainFrame);
                        frmMainFrame.Content = pgUsuario;
                    };

                    searchList.Items.Add(listViewItem);
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            searchScroll.Visibility = Visibility.Collapsed;
        }

        private void imgProfile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            movingImage.Visibility = Visibility.Hidden;
            PgUsuarioProfile pgUsuario = new PgUsuarioProfile(_usuario);
            frmMainFrame.Content = pgUsuario;
        }

        private void imgNotifications_MouseDown(object sender, MouseButtonEventArgs e)
        {
            movingImage.Visibility = Visibility.Hidden;
            PgListaNotificaciones pgListaNotificaciones = new PgListaNotificaciones(_usuario.Id);
            frmMainFrame.Content = pgListaNotificaciones;
        }

        private void txtBuscador_LostFocus(object sender, RoutedEventArgs e)
        {
            txtBuscador.Clear();
        }

        private void BtnLogout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
