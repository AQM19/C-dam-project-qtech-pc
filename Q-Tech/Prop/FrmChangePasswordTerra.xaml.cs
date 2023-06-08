using Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para FrmChangePasswordTerra.xaml
    /// </summary>
    public partial class FrmChangePasswordTerra : Window
    {
        private Terrario _terra;

        public FrmChangePasswordTerra()
        {
            InitializeComponent();
        }

        public FrmChangePasswordTerra(Terrario terrario) : this()
        {
            _terra = terrario;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidData())
            {
                _terra.Contrasena = SamePassword(PswNewPassword.Password);
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
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            string hashString = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                hashString = BitConverter.ToString(hash).Replace("-", "");
            }

            return hashString;
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
            if (SamePassword(PswCurrentPassword.Password) != _terra.Contrasena)
            {
                MessageBox.Show("La contraseña es incorrecta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PswCurrentPassword.BorderBrush = Brushes.Red;
                PswCurrentPassword.Focus();
                return false;
            }
            if (SamePassword(PswNewPassword.Password) == _terra.Contrasena)
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
