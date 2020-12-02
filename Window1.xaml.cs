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

namespace lab2
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        List<string> items = new List<string>();
        List<List<string>> pagination = new List<List<string>>();
        private int n;
        private int list;
        public Window1(int n, int list)
        {
            this.n = n;
            this.list = list;
            InitializeComponent();
            foreach (var item in MainWindow.threatsData)
            {
                items.Add($"УБИ.{item.Id} {item.Name}");
            }
            int i1 = 0;
            int j1 = 0;
            foreach (var item in items)
            {
                if (j1 == 0)
                {
                    pagination.Add(new List<string>());
                }
                if (j1 < n - 1)
                {
                    pagination[i1].Add(item);
                    j1++;
                }
                else
                {
                    pagination[i1].Add(item);
                    j1 = 0;
                    i1++;
                }
            }
            ListBox.Items.Clear();
            foreach (var item in pagination[list - 1])
            {
                
                ListBox.Items.Add(item);
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            var i = int.Parse(selectedItem.Content.ToString());
            if (n != i && (list - 1) * i < items.Count)
            {
                Window1 w1 = new Window1(i, list);
                w1.Show();
                this.Close();
            }
            else
            {
                Window1 w1 = new Window1(1000, 1);
                w1.Show();
                this.Close();
            }
        }

        private void TextBlock1_TextChanged(object sender, TextChangedEventArgs e)
        {
            int x;
            TextBox textBox = (TextBox)sender;
            if (int.TryParse(textBox.Text, out x))
            {
                if (list != x && (x-1)*n<items.Count)
                {
                    Window1 w1 = new Window1(n, x);
                    w1.Show();
                    this.Close();
                }
                else
                {
                    TextBlock1.Text = $"{list}";
                }

            }
            else TextBlock1.Text = $"{list}";
        }
    }
}
