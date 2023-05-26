using BusinessLogic;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para frmListaTareas.xaml
    /// </summary>
    public partial class FrmListaTareas : Window
    {
        private readonly long _id;
        private readonly List<Tarea> _tareasCambiadas;

        public List<Tarea> TareasCambiadas => _tareasCambiadas;

        public FrmListaTareas()
        {
            InitializeComponent();
        }
        public FrmListaTareas(long id) : this()
        {
            _id = id;
            _tareasCambiadas = new List<Tarea>();
            CargarTareas();
        }

        private async void CargarTareas()
        {
            lvListaTareas.Items.Clear();

            List<Tarea> tareas = await Herramientas.GetTareasByTerra(_id);

            foreach (Tarea t in tareas)
            {
                ListViewItem listViewItem = new ListViewItem
                {
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                Grid grid = new Grid();

                ColumnDefinition column1 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                ColumnDefinition column2 = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                ColumnDefinition column3 = new ColumnDefinition
                {
                    Width = new GridLength(0.6, GridUnitType.Star)
                };

                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                grid.ColumnDefinitions.Add(column3);

                RowDefinition row1 = new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                RowDefinition row2 = new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };

                grid.RowDefinitions.Add(row1);
                grid.RowDefinitions.Add(row2);

                TextBlock titleTextBlock = new TextBlock
                {
                    Text = t.Titulo,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(5),
                    FontWeight = FontWeights.Bold,
                };
                titleTextBlock.SetValue(Grid.ColumnProperty, 0);
                titleTextBlock.SetValue(Grid.ColumnSpanProperty, 2);
                titleTextBlock.SetValue(Grid.RowProperty, 0);

                TextBlock dateTextBlock = new TextBlock
                {
                    Text = t.FechaCreacion.ToShortDateString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Padding = new Thickness(5)
                };
                dateTextBlock.SetValue(Grid.ColumnProperty, 2);
                dateTextBlock.SetValue(Grid.RowProperty, 0);

                TextBlock descriptionTextBlock = new TextBlock
                {
                    Text = t.Descripcion,
                    TextWrapping = TextWrapping.Wrap,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Padding = new Thickness(5)
                };
                descriptionTextBlock.SetValue(Grid.ColumnProperty, 0);
                descriptionTextBlock.SetValue(Grid.ColumnSpanProperty, 2);
                descriptionTextBlock.SetValue(Grid.RowProperty, 1);

                ComboBox statusComboBox = new ComboBox
                {
                    Margin = new Thickness(5)
                };
                statusComboBox.SetValue(Grid.ColumnProperty, 2);
                statusComboBox.SetValue(Grid.RowProperty, 1);

                ComboBoxItem startedItem = new ComboBoxItem
                {
                    IsSelected = t.Estado == "Iniciada",
                    Content = "Iniciada"
                };
                statusComboBox.Items.Add(startedItem);

                ComboBoxItem inProgressItem = new ComboBoxItem
                {
                    IsSelected = t.Estado == "En progreso",
                    Content = "En progreso"
                };
                statusComboBox.Items.Add(inProgressItem);

                ComboBoxItem completedItem = new ComboBoxItem
                {
                    IsSelected = t.Estado == "Realizada",
                    Content = "Realizada"
                };
                statusComboBox.Items.Add(completedItem);

                ComboBoxItem cancelledItem = new ComboBoxItem
                {
                    IsSelected = t.Estado == "Cancelada",
                    Content = "Cancelada"
                };
                statusComboBox.Items.Add(cancelledItem);

                statusComboBox.SelectionChanged += (sender, e) =>
                {
                    string selectedText = ((ComboBoxItem)statusComboBox.SelectedItem).Content.ToString();

                    if (selectedText == "Realizada")
                    {
                        t.FechaResolucion = DateTime.Now;
                    }

                    t.Estado = selectedText;
                    TareasCambiadas.Add(t);
                };

                grid.Children.Add(titleTextBlock);
                grid.Children.Add(dateTextBlock);
                grid.Children.Add(descriptionTextBlock);
                grid.Children.Add(statusComboBox);

                listViewItem.Content = grid;

                listViewItem.MouseDoubleClick += (sender, e) => UpdateTarea(t);

                lvListaTareas.Items.Add(listViewItem);
            }
        }

        private async void UpdateTarea(Tarea tarea)
        {
            FrmAddTarea frmAddTarea = new FrmAddTarea(_id, tarea);

            if (frmAddTarea.ShowDialog() == true)
            {
                await Herramientas.UpdateTarea(tarea.Id, tarea);
                CargarTareas();
            }
        }

        private async void BtnSaveTareas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TareasCambiadas.Count > 0)
            {
                foreach (Tarea t in TareasCambiadas)
                {
                    await Herramientas.UpdateTarea(t.Id, t);
                }

                TareasCambiadas.Clear();
                CargarTareas();
            }
        }

        private async void BtnNewTarea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Tarea tarea = new Tarea();
            FrmAddTarea frmAddTarea = new FrmAddTarea(_id, tarea);

            if (frmAddTarea.ShowDialog() == true)
            {
                await Herramientas.CreateTarea(tarea);
                CargarTareas();
            }
        }
    }
}
