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
    /// Lógica de interacción para frmAddTarea.xaml
    /// </summary>
    public partial class FrmAddTarea : Window
    {
        private readonly long _id;
        private readonly Tarea _tarea;
        public FrmAddTarea()
        {
            InitializeComponent();
        }
        public FrmAddTarea(long id, Tarea tarea) : this()
        {
            _id = id;
            _tarea = tarea;

            if (_tarea.Id > 0)
            {
                _tarea.FechaCreacion = DateTime.Now;
            }
            DesplegarInformacion();
        }

        private void DesplegarInformacion()
        {
            if (_tarea.Id > 0)
            {
                txtTitulo.Text = _tarea.Titulo;
                cboStatus.Text = _tarea.Estado;
                txtTarea.Text = _tarea.Descripcion;
            }
            _tarea.Idterrario = _id;
        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _tarea.Titulo = txtTitulo.Text;
            _tarea.Descripcion = txtTarea.Text;
            _tarea.Estado = cboStatus.Text;

            if (cboStatus.Text.Equals("Realizada"))
            {
                _tarea.FechaResolucion = DateTime.Now;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
