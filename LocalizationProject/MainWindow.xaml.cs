using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace LocalizationProject
{
    public partial class MainWindow : Window
    {
        private readonly (string KeyPrefix, string Key, string Text) list = new();

        private readonly List<(string name, Type type)> headerColumns = new();

        public MainWindow()
        {
            InitializeComponent();

            DataGridTable.Columns.Add(BuildingGridViewColumn("Key"));

            headerColumns.Add(("Key", typeof(string)));
        }

        /// <summary>
        /// Формирование таблицы
        /// </summary>
        private void BuildingTable()
        {
            object newClass = UnknownClass.BuildingClass("MainClass", headerColumns);

            DataGridTable.Items.Add(newClass);
        }

        /// <summary>
        /// Создание наименования колонок
        /// </summary>
        /// <param name="bindingName">Строка подключения/Наименование колонки</param>
        /// <returns>Колонка</returns>
        private DataGridTextColumn BuildingGridViewColumn(string bindingName) =>
            new() { Header = bindingName, Binding = new Binding(bindingName) };

        /// <summary>
        /// Создание необходимой строки
        /// </summary>
        /*<system:String x:Key="{Key-Prefix}.{Key}">{Text}</system:String>*/
        private string BuildingStrings() =>
            $"<system:String x:Key=\"{list.KeyPrefix}.{list.Key}\">{list.Text}</system:String>";

        #region Обработчики

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

            DataGridTable.Columns.Add(BuildingGridViewColumn(Language.Text.Trim()));

            headerColumns.Add((Language.Text.Trim(), typeof(string)));

            Language.Clear();
        }
        #endregion

        private void DataGridTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}