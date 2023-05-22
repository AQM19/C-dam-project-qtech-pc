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
    /// Lógica de interacción para pgTerrario.xaml
    /// </summary>
    public partial class pgTerrario : Page
    {
        private Terrario _terrario;
        public pgTerrario()
        {
            InitializeComponent();
        }
        public pgTerrario(Terrario terrario) : this()
        {
            _terrario = terrario;
            DesplegarInfo();
        }

        private async void DesplegarInfo()
        {
            //imgTerraPic.Source = new BitmapImage(new Uri(_terrario.Foto != null ? _terrario.Foto : "C:\\Users\\aaron\\OneDrive\\Escritorio\\PROJECTS\\QTECH_PC\\Q-Tech\\Recursos\\Iconos\\MainIcon.png"));
            txbTerraName.Text = _terrario.Nombre;
            txbTerraDescription.Text = _terrario.Descripcion;
            txbFecha.Text = _terrario.FechaCreacion.ToShortDateString();
            txbTerraSubstrate.Text = _terrario.Sustrato;
            txbTerraEcosystem.Text = _terrario.Ecosistema;
            txbTerraSize.Text = _terrario.Tamano.ToString();
            float puntuacion = await Herramientas.GetPuntuacionTerrario(_terrario.Id);
            txbTerraPunctuation.Text = puntuacion.ToString();


            List<Especie> list = await Herramientas.GetEspeciesTerrario(_terrario.Id);

            foreach(Especie e in list)
            {
                ListViewItem item = new ListViewItem{
                    Content = $"{e.Genero} {e.Sp}"
                };

                lvSpList.Items.Add(item);
            }
        }

    }
}
