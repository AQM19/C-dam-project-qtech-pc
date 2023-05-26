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
using System.Windows.Shapes;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmCodeConfirmation.xaml
    /// </summary>
    public partial class FrmCodeConfirmation : Window
    {
        private readonly string _codeConfirmation;
        private int _try = 3;
        public FrmCodeConfirmation()
        {
            InitializeComponent();
        }
        public FrmCodeConfirmation(string codeConfirmation) : this()
        {
            _codeConfirmation = codeConfirmation;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (TxtConfirmCode.Text.Equals(_codeConfirmation))
            {
                this.DialogResult = true;
                this.Close();
            }

            --_try;

            if(_try == 0)
            {
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
