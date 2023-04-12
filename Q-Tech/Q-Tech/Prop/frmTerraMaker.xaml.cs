using Entities;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmTerraMaker.xaml
    /// </summary>
    public partial class frmTerraMaker : Window
    {
        private Usuario _usuario;
        public frmTerraMaker()
        {
            InitializeComponent();
        }

        public frmTerraMaker(Usuario usuario) : base()
        {
            _usuario = usuario;
            this.Title = $"Terrario de {_usuario.NombreUsuario}";
        }

        private void spSize_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            numberPicker.Focus();
        }

        private void numberPicker_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!IsNumberKey(e.Key) && e.Key != Key.Tab)
            {
                e.Handled = true;
            }
        }

        private bool IsNumberKey(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }

        private void tbDescription_KeyUp(object sender, KeyEventArgs e)
        {
            int length = tbDescription.Text.Length;
            string text = tbDescription.Text;
            Brush foreground = tbDescription.Foreground;

            text = $"{length} caracteres";

            if (length > 0 && length < 200)
                foreground = Brushes.LimeGreen;
            if (length > 200 && length < 250)
            {
                foreground = Brushes.Orange;
                tbDescription.Visibility = Visibility.Hidden;
            }

            if (length > 250)
            {
                foreground = Brushes.Red;
                tbDescription.Visibility = Visibility.Visible;
            }

        }

        private void bdrUpload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Subir imagen";
            openFileDialog.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
            }
        }

        private void chkPrivate_Click(object sender, RoutedEventArgs e)
        {
            spPass.Visibility = (chkPrivate.IsChecked == true) ? Visibility.Visible : Visibility.Hidden;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("El campo nombre no puede estar vacío", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbName.Focus();
                return false;
            }
            if (chkPrivate.IsChecked == true && string.IsNullOrEmpty(pwbPassword.Password))
            {
                MessageBox.Show("Si el terrario es privado debería tener una contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                pwbPassword.Focus();
                return false;
            }
            return true;
        }
    }
}
