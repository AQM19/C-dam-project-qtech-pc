using Entities;
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
    /// Lógica de interacción para frmAddObservacion.xaml
    /// </summary>
    public partial class FrmAddObservacion : Window
    {
        private readonly long _id;
        private readonly Observacion _observacion;
        public FrmAddObservacion()
        {
            InitializeComponent();
        }
        public FrmAddObservacion(long id, Observacion observacion) : this()
        {
            _id = id;
            _observacion = observacion;
            DesplegarInformacion();
        }

        private void DesplegarInformacion()
        {
            txbTexto.Text = _observacion.Texto;
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _observacion.Texto = txbTexto.Text;
            _observacion.Fecha = DateTime.Now;
            _observacion.Idterrario = _id;

            this.DialogResult = true;
            this.Close();
        }
    }
}
