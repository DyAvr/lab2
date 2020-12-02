using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
using ClosedXML.Excel;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace lab2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static BindingList<Threat> threatsData = new BindingList<Threat>();
        private static Dictionary<Threat, Threat> updatedData = new Dictionary<Threat, Threat>();
        private static BindingList<Threat> addedData = new BindingList<Threat>();
        private static DateTime updateTime = new DateTime();
        public MainWindow()
        {
            InitializeComponent();
            using (var c = new WebClient())
            {
                c.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", @"..\..\thrlist.xlsx");
            }

            if (!File.Exists("..\\..\\threatsData.txt"))
            {
                string message = "Файла с локальной базой не существует! Хотите провести первичную загрузку данных?";
                string caption = "Ошибка";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result;
                result = MessageBox.Show(message, caption, buttons,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    ParseDB();
                    updateTime = DateTime.Now;
                    SerializeDB();
                }
                
            }
            else
            {
                DeserializeDB();
            }
            dgLocal.ItemsSource = threatsData;
            if ((DateTime.Now - updateTime).TotalDays >= 30)
            {
                string message = "Файла с локальной базой устарел! Хотите провести обновление данных?";
                string caption = "Устаревшая версия локальной базы";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result;
                result = MessageBox.Show(message, caption, buttons,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateDB();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgLocal.ItemsSource = threatsData;
        }

        private void dgLocal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int selectedColumn = dgLocal.CurrentCell.Column.DisplayIndex;
            var selectedCell = dgLocal.SelectedCells[selectedColumn];
            var cellContent = selectedCell.Column.GetCellContent(selectedCell.Item);
            if (cellContent is TextBlock)
            {
                MessageBox.Show((cellContent as TextBlock).Text);
            }
        }

        private void DeserializeDB()
        {
            try
            {
                using (var reader = File.OpenText("..\\..\\threatsData.txt"))
                {
                    threatsData = JsonConvert.DeserializeObject<BindingList<Threat>>(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка", e.Message);
            }

            try
            {
                using (var reader = File.OpenText("..\\..\\updateTime.txt"))
                {
                    updateTime = JsonConvert.DeserializeObject<DateTime>(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка", e.Message);
            }
            
        }

        private void SerializeDB()
        {
            using (StreamWriter writer = File.CreateText("..\\..\\threatsData.txt"))
            {
                var output = JsonConvert.SerializeObject(threatsData);
                writer.Write(output);
            }
            using (StreamWriter writer = File.CreateText("..\\..\\updateTime.txt"))
            {
                var output = JsonConvert.SerializeObject(updateTime);
                writer.Write(output);
            }
        }

        private void ParseDB()
        {
            XLWorkbook workbook;
            //Открываем файл. Только XLSX, старые xls вроде не читает.
            using (workbook = new XLWorkbook(@"..\..\thrlist.xlsx"))
            {
                //workbook.Worksheets - коллекция листов, можно применять функции как для списков. 
                //В данном случае First()
                var ws = workbook.Worksheets.First();
                var rows = ws.RangeUsed().RowsUsed();
                //Общее количество строк, занятых данными
                int rowsCount = rows.Count();

                
                //i - строка, с которой начинаем считывать данные. Выше, например, шапка таблицы
                for (int i = 3; i <= rowsCount; ++i)
                {
                    var Data1 = ws.Cell(i, 1);
                    var Data2 = ws.Cell(i, 2);
                    var Data3 = ws.Cell(i, 3);
                    var Data4 = ws.Cell(i, 4);
                    var Data5 = ws.Cell(i, 5);
                    var Data6 = ws.Cell(i, 6);
                    var Data7 = ws.Cell(i, 7);
                    var Data8 = ws.Cell(i, 8);

                    threatsData.Add(new Threat()
                    {
                        Id = Data1.GetValue<int>(),
                        Name = Data2.GetValue<string>(),
                        Description = Data3.GetValue<string>(),
                        Source = Data4.GetValue<string>(),
                        Target = Data5.GetValue<string>(),
                        IsNotConfidential = (Data6.GetValue<int>() == 1),
                        IsNotComplete = (Data7.GetValue<int>() == 1),
                        IsUnavailable = (Data8.GetValue<int>() == 1),
                    });
                }
            }
        }

        // для каждой изменённой записи выводится 2 строки:
        // 1) старая запись
        // 2) новая запись
        // для каждой новой строки выводится её запись
        private void UpdateDB()
        {
            try
            {
                using (var c = new WebClient())
                {
                    c.DownloadFile(@"https://bdu.fstec.ru/files/documents/thrlist.xlsx", @"..\..\thrlist.xlsx");
                }

                var old = threatsData;
                threatsData = new BindingList<Threat>();
                ParseDB();
                for (int i = 0; i < threatsData.Count; i++)
                {
                    if (i < old.Count)
                    {
                        if (!threatsData[i].isTheSame(old[i]))
                        {
                            updatedData.Add(old[i],threatsData[i]);
                        }
                    }
                    else
                    {
                        addedData.Add(threatsData[i]);
                    }
                }
                var updatedList = new BindingList<Threat>();
                foreach (var item in updatedData)
                {
                    updatedList.Add(item.Key);
                    updatedList.Add(item.Value);
                }

                foreach (var item in addedData)
                {
                    updatedList.Add(item);
                }
                //dgLocal.ItemsSource = threatsData;
                dgLocal.ItemsSource = updatedList;
                this.dgLocal.UpdateLayout();
                updateTime = DateTime.Now;
                SerializeDB();
                MessageBox.Show($"Общее количество обновлённых записей {updatedData.Count}\n" +
                                $"Общее количество новых записей {addedData.Count}", "Успешно");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка");
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            dgLocal.ItemsSource = threatsData;
            this.dgLocal.UpdateLayout();
        }

        private void ShowListBox()
        {
            var w = new Window1(1000,1);
            w.Show();
            this.Close();
        }

        private void Window1_Button_Click(object sender, RoutedEventArgs e)
        {
            ShowListBox();
        }
    }
}
