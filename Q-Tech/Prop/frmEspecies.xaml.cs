using BusinessLogic;
using Entities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Q_Tech.Prop
{
    /// <summary>
    /// Lógica de interacción para frmEspecies.xaml
    /// </summary>
    public partial class FrmEspecies : Window
    {
        private Especie _especie;
        private List<Especie> _especies;

        public Especie Especie { get => _especie; }

        public FrmEspecies()
        {
            InitializeComponent();
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
            //string[] item = new string[8];

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
    }
}
