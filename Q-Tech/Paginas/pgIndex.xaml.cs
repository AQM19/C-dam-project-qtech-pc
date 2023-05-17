﻿using BusinessLogic;
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
using Q_Tech.Prop;

namespace Q_Tech.Modales
{
    public partial class pgIndex : Page
    {
        private readonly Usuario _user;
        private Terrario _selectedTerra;
        private List<Terrario> _terrarios;

        public pgIndex()
        {
            InitializeComponent();
        }

        public pgIndex(Usuario user) : this()
        {
            _user = user;
            ObtenerTerrarios();
        }

        private async void ObtenerTerrarios()
        {
            spListTerra.Children.Clear();
            _terrarios = await Herramientas.GetTerrariosUsuario(_user.Id);

            if(_terrarios.Count == 0)
                brMessage.Visibility = Visibility.Visible;

            for (int i = 0; i < _terrarios.Count; i++)
            {
                int index = i;
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(_terrarios[i].Foto ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill
                };

                Border myBorder = new Border
                {
                    CornerRadius = new CornerRadius(15),
                    Height = 100,
                    Width = 100,
                    Margin = new Thickness(15),
                    Cursor = Cursors.Hand,
                    Child = image,
                    Tag = index
                };

                myBorder.MouseDown += (sender, e) => SeleccionarTerrario(_terrarios[index]);

                spListTerra.Children.Add(myBorder);
            }

            if (_terrarios.Count > 0) SeleccionarTerrario(_terrarios[0]);
        }

        private void SeleccionarTerrario(Terrario terra)
        {
            if (_terrarios.Count > 0)
            {
                _selectedTerra = terra;

                SelectedTerra.Source = new BitmapImage(new Uri(_selectedTerra.Foto ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute));
            }
        }

        private async void bdrMainTerra_MouseDown(object sender, MouseButtonEventArgs e)
        {
            frmTerraMaker terraMaker = new frmTerraMaker(_user, _selectedTerra);
            if(terraMaker.ShowDialog() == true)
            {
                await Herramientas.UpdateTerrario(_selectedTerra.Id, _selectedTerra);
                await Herramientas.UpdateEspeciesOfTerrario(_selectedTerra.Id, terraMaker.EspeciesTerrario);
                ObtenerTerrarios();
            }
        }

        private async void btnAddTerra_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Terrario terra = new Terrario();
            frmTerraMaker terraMaker = new frmTerraMaker(_user, terra);
            if (terraMaker.ShowDialog() == true)
            {
                await Herramientas.CreateTerrario(terra);
                ObtenerTerrarios();
            }

        }
    }
}
