using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace LocalizationProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private (string KeyPrefix, string Key, string Text) list = new();
        private GridView gridView = new();


        public MainWindow()
        {
            InitializeComponent();

            gridView.Columns.Add(BuildingGridViewColumn("Key"));
            ListTable.View = gridView;
        }

        /// <summary>
        /// Формирование таблицы
        /// </summary>
        private void BuildingTable()
        {

        }

        /// <summary>
        /// Создание наименования колонок
        /// </summary>
        /// <param name="bindingName">Строка подключения/Наименование колонки</param>
        /// <returns>Колонка</returns>
        private GridViewColumn BuildingGridViewColumn(string bindingName) =>
            new(){ Header = bindingName, DisplayMemberBinding = new Binding(bindingName) };

        /*
         * <ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:system="clr-namespace:System;assembly=System.Runtime">

                <system:String x:Key="{Key-Prefix}.{Key}">{Text}</system:String>

            </ResourceDictionary>
        */

        /// <summary>
        /// Создание необходим  ой строки
        /// </summary>
        /*<system:String x:Key="{Key-Prefix}.{Key}">{Text}</system:String>*/
        private string BuildingStrings() =>
            $"<system:String x:Key=\"{list.KeyPrefix}.{list.Key}\">{list.Text}</system:String>";

        /// <summary>
        /// Сформированные таблицы
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e) =>
            BuildingTable();

        /// <summary>
        /// Добавление новой колонки
        /// </summary>
        private void AddLanguage(object sender, RoutedEventArgs e)
        {
            if (Language.Text == "" || Language.Text == null)
                return;

            gridView.Columns.Add(BuildingGridViewColumn(Language.Text.Trim()));
        }
    }
}