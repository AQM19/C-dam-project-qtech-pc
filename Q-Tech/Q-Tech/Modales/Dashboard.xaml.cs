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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Q_Tech.Modales
{
    /// <summary>
    /// Lógica de interacción para FrmDashboard.xaml
    /// </summary>
    public partial class FrmDashboard : Window
    {
        public FrmDashboard()
        {
            InitializeComponent();
        }        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            Point targetPoint = clickedButton.TranslatePoint(new Point(0, 0), dkpVerticalMenu);

            DoubleAnimation animationY = new DoubleAnimation();
            animationY.To = targetPoint.Y - 65;
            animationY.Duration = TimeSpan.FromSeconds(0.3);

            TranslateTransform transform = (TranslateTransform)movingImage.RenderTransform;

            transform.BeginAnimation(TranslateTransform.YProperty, animationY);
            
        }
        
    }
}
