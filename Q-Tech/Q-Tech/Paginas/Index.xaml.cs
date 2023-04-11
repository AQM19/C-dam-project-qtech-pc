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
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Q_Tech.Modales
{
    public partial class Index : Page
    {
        private long _id;
        private List<Terrario> _terrarios;

        public Index()
        {
            InitializeComponent();
        }

        public Index(long id) : this()
        {
            _id = id;
            ObtenerTerrarios();
        }

        private async void ObtenerTerrarios()
        {
            _terrarios = await Herramientas.GetTerrarios(_id);

            for (int i = 0; i < _terrarios.Count; i++)
            {
                ImageBrush myImageBrush = new ImageBrush();
                myImageBrush.ImageSource = new BitmapImage(new Uri(_terrarios[i].Foto, UriKind.Absolute));
                myImageBrush.Stretch = Stretch.Fill;

                Border myBorder = new Border
                {
                    CornerRadius = new CornerRadius(15),
                    Height = 100,
                    Width = 100,
                    Margin = new Thickness(15),
                    Background = myImageBrush,
                    Cursor = Cursors.Hand,
                    Tag = i                  
                };

                myBorder.MouseDown += (sender, e) => SeleccionarTerrario((int)((Border)sender).Tag);


                spListTerra.Children.Add(myBorder);
            }

            SeleccionarTerrario(0);
        }

        private void SeleccionarTerrario(int id)
        {
            if (_terrarios.Count > 0)
            {
                SelectedTerra.ImageSource = new BitmapImage(new Uri(_terrarios[id].Foto, UriKind.Absolute));
            }
        }


        private void bdrMainTerra_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
