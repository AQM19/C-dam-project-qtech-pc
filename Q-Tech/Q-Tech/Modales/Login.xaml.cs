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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Q_Tech
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _showPassword;

        public MainWindow()
        {
            InitializeComponent();

            _showPassword = false;
        }

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("El campo usuario no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUser.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(PasswordBind.Password))
            {
                MessageBox.Show("El campo contraseña no puede estar vacío.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                _ = (PasswordBind.Visibility == Visibility.Visible) ? PasswordBind.Focus() : PasswordUnbind.Focus();
                return false;
            }
            return true;
        }

        private void PasswordViewer_Click(object sender, RoutedEventArgs e)
        {
            _showPassword = !_showPassword;
            PasswordUnbind.Visibility = _showPassword ? Visibility.Visible : Visibility.Hidden;
            PasswordBind.Visibility = _showPassword ? Visibility.Hidden : Visibility.Visible;
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                MessageBox.Show("¡Login correcto!.");
            }
        }

        private void brdUser_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUser.Focus();
        }

        private void PasswordBind_KeyUp(object sender, KeyEventArgs e)
        {
            PasswordUnbind.Text = PasswordBind.Password;
        }

        private void PasswordUnbind_KeyUp(object sender, KeyEventArgs e)
        {
            PasswordBind.Password = PasswordUnbind.Text;
        }
    }
}
