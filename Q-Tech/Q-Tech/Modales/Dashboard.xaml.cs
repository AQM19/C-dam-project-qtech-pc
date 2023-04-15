using BusinessLogic;
using Entities;
using Q_Tech.Paginas;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Q_Tech.Modales
{
    public partial class FrmDashboard : Window
    {

        private Usuario _usuario;
        public FrmDashboard()
        {
            InitializeComponent();
            ObtenerUsuario();
        }

        private async void ObtenerUsuario() // cachear el usuario
        {
            _usuario = await Herramientas.GetUsuario(1);
            CargarIndex();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Border clickedButton = (Border)sender;
            Point targetPoint = clickedButton.TranslatePoint(new Point(0, 0), dkpVerticalMenu);

            DoubleAnimation animationY = new DoubleAnimation();
            animationY.To = targetPoint.Y - 65;
            animationY.Duration = TimeSpan.FromSeconds(0.3);

            TranslateTransform transform = (TranslateTransform)movingImage.RenderTransform;

            transform.BeginAnimation(TranslateTransform.YProperty, animationY);

            if (clickedButton.Name.Equals("btnInicio"))
                CargarIndex();
            if (clickedButton.Name.Equals("btnUsuarios"))
                CargarUsuarios();
            if (clickedButton.Name.Equals("btnTerrarios"))
                CargarTerrarios();

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtBuscador.Focus();
        }

        private void CargarIndex()
        {
            Index index = new Index(_usuario.Id);
            frmMainFrame.Content = index;
        }

        private void CargarUsuarios()
        {
            pgUsuarios pgUsuarios = new pgUsuarios();
            frmMainFrame.Content = pgUsuarios;
        }

        private void CargarTerrarios()
        {
            pgTerrarios pgTerrarios = new pgTerrarios(_usuario);
            frmMainFrame.Content = pgTerrarios;
        }
    }
}
