using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para FrmChangePassword.xaml
    /// </summary>
    public partial class FrmChangePassword : Window
    {
        private Usuario _user;

        public FrmChangePassword()
        {
            InitializeComponent();
        }

        public FrmChangePassword(Usuario user) : this()
        {
            _user = user;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                _user.Contrasena = SamePassword(PswNewPassword.Password);
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string SamePassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] inputBytes = passwordBytes.Concat(_user.Salt).ToArray();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passHash = sha256.ComputeHash(inputBytes);
                string hash = Convert.ToBase64String(passHash);
                return hash;
            }
        }

        private bool ValidData()
        {
            if (string.IsNullOrEmpty(PswCurrentPassword.Password) || string.IsNullOrWhiteSpace(PswCurrentPassword.Password))
            {
                MessageBox.Show("El campo de la contraseña no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswCurrentPassword.BorderBrush = Brushes.Red;
                PswCurrentPassword.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(PswNewPassword.Password) || string.IsNullOrWhiteSpace(PswNewPassword.Password))
            {
                MessageBox.Show("El campo de la contraseña no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswNewPassword.BorderBrush = Brushes.Red;
                PswNewPassword.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(PswNewRepeatedPAssword.Password) || string.IsNullOrWhiteSpace(PswNewRepeatedPAssword.Password))
            {
                MessageBox.Show("El campo de la contraseña no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswNewRepeatedPAssword.BorderBrush = Brushes.Red;
                PswNewRepeatedPAssword.Focus();
                return false;
            }
            if(SamePassword(PswCurrentPassword.Password) != _user.Contrasena)
            {
                MessageBox.Show("La contraseña es incorrecta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswCurrentPassword.BorderBrush = Brushes.Red;
                PswCurrentPassword.Focus();
                return false;
            }
            if (SamePassword(PswNewPassword.Password) == _user.Contrasena)
            {
                MessageBox.Show("La nueva contraseña no puede coincidir con la actual.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswNewPassword.BorderBrush = Brushes.Red;
                PswNewPassword.Focus();
                return false;
            }
            if (PswNewPassword.Password != PswNewRepeatedPAssword.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswNewPassword.BorderBrush = Brushes.Red;
                PswNewRepeatedPAssword.BorderBrush = Brushes.Red;
                PswNewPassword.Focus();
                return false;
            }
            return true;
        }
    }
}
