using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmEspecies.xaml
    /// </summary>
    public partial class FrmEspecies : Window
    {
        private Especie _especie;
        private List<Especie> _especies;
        private string _textValue;
        private readonly DispatcherTimer debounceTimer = new DispatcherTimer();
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public Especie Especie { get => _especie; }

        public FrmEspecies()
        {
            InitializeComponent();
            debounceTimer.Interval = TimeSpan.FromMilliseconds(500);
            debounceTimer.Tick += DebounceTimer_Tick;
        }
        public FrmEspecies(List<Especie> especies) : this()
        {
            _especies = new List<Especie>();
            ObtenerEspecies(especies);
        }

        private async void ObtenerEspecies(List<Especie> especies)
        {
            _especies = await Herramientas.GetEspeciesPosibles(especies);

            lvEspecies.Items.Clear();

            for (int i = 0; i < _especies.Count; i++)
            {
                ListViewItem listViewItem = new ListViewItem
                {
                    Content = _especies[i],
                    Tag = _especies[i].Id
                };

                listViewItem.MouseDoubleClick += ListViewItem_MouseDoubleClick;

                lvEspecies.Items.Add(listViewItem);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _especie = (Especie)((ListViewItem)sender).Content;
            this.DialogResult = true;
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textValue = ((TextBox)sender).Text;
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();

            // Obtener el texto del TextBox
            string searchText = _textValue.ToLower();

            // Filtrar las especies que coincidan con el texto ingresado en el TextBox
            List<Especie> especiesFiltradas = _especies.FindAll(especie =>
                especie.Genero.ToLower().Contains(searchText) ||
                especie.Sp.ToLower().Contains(searchText)
            );

            lvEspecies.Items.Clear();

            foreach (Especie especie in especiesFiltradas)
            {
                ListViewItem listViewItem = new ListViewItem
                {
                    Content = especie,
                    Tag = especie.Id
                };

                listViewItem.MouseDoubleClick += ListViewItem_MouseDoubleClick;

                lvEspecies.Items.Add(listViewItem);
            }
        }
    }
}
