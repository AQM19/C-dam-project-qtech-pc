using BusinessLogic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Q_Tech.Paginas
{
    /// <summary>
    /// Lógica de interacción para pgLogros.xaml
    /// </summary>
    public partial class pgLogros : Page
    {
        private long _id;
        public pgLogros()
        {
            InitializeComponent();
        }
        public pgLogros(long id) : this()
        {
            this._id = id;
            CargarLogros();
        }

        private async void CargarLogros()
        {
            List<Logro> logros = await Herramientas.GetLogros(_id);
        }
    }
}
