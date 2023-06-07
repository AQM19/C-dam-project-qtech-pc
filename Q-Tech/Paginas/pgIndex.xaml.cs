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
using System.Threading;

namespace Q_Tech.Modales
{
    public partial class PgIndex : Page
    {
        private readonly Usuario _user;
        private Terrario _selectedTerra;
        private List<Terrario> _terrarios;
        private Timer _timer;

        public PgIndex()
        {
            InitializeComponent();
        }

        public PgIndex(Usuario user) : this()
        {
            _user = user;
            ObtenerTerrarios();
        }

        private async void ObtenerTerrarios()
        {
            spListTerra.Children.Clear();
            _terrarios = await Herramientas.GetTerrariosUsuario(_user.Id);

            if (_terrarios.Count > 0)
            {
                brMessage.Visibility = Visibility.Collapsed;
                dpMainPanel.Visibility = Visibility.Visible;
            }

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

            _timer = new Timer(async (_) => await ObtenerValoresTerrario(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private async Task ObtenerValoresTerrario()
        {
            if (_selectedTerra != null)
            {
                Lectura lectura = await Herramientas.GetLecturaActual(_selectedTerra.Id);

                if (lectura != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        chartTemperature.Value = float.Parse(Math.Round(lectura.Temperatura, 1).ToString("F1"));
                        chartHumid.Value = float.Parse(Math.Round(lectura.Humedad, 1).ToString("F1"));
                        chartLight.Value = lectura.Luz * 10;
                    });
                }
            };
        }



        private async void SeleccionarTerrario(Terrario terra)
        {
            if (_terrarios.Count > 0)
            {
                _selectedTerra = terra;
                SelectedTerra.Source = new BitmapImage(new Uri(_selectedTerra.Foto ?? "/Recursos/Iconos/MainIcon.png", UriKind.RelativeOrAbsolute));
                await ObtenerValoresTerrario();
            }
        }

        private async void bdrMainTerra_MouseDown(object sender, MouseButtonEventArgs e)
        {

            FrmTerraMaker terraMaker = new FrmTerraMaker(_user, _selectedTerra);
            if (terraMaker.ShowDialog() == true)
            {
                await Herramientas.UpdateTerrario(_selectedTerra.Id, _selectedTerra);
                await Herramientas.UpdateEspeciesOfTerrario(_selectedTerra.Id, terraMaker.EspeciesTerrario);

                ObtenerTerrarios();
            }
        }

        private async void btnAddTerra_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Terrario terra = new Terrario();
            FrmTerraMaker terraMaker = new FrmTerraMaker(_user, terra);

            if (terraMaker.ShowDialog() == true)
            {
                await Herramientas.CreateTerrario(terra);
                if (terra.Id > 0)
                {
                    await Herramientas.UpdateEspeciesOfTerrario(_selectedTerra.Id, terraMaker.EspeciesTerrario);
                }
                ObtenerTerrarios();
            }
        }

        private void PgIndex_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer?.Dispose();
        }
    }
}
