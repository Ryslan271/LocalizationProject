using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LocalizationProject
{
    public partial class MainWindow : Window
    {
        private readonly (string KeyPrefix, string Key, string Text) list = new();

        private readonly List<(string name, Type type)> headerColumns = new();

        private List<object> unknownClasses = new();

        private object? unknownClass;

        public MainWindow() =>
            InitializeComponent();

        /// <summary>
        /// Формирование таблицы
        /// </summary>
        private void BuildingTable()
        {
            if (unknownClasses.Count() != 0)
                return;

            unknownClass ??= UnknownClass.BuildingClass($"MainClass", headerColumns);

            unknownClasses.Add(unknownClass);

            DataGridTable.Columns.Clear();
            DataGridTable.ItemsSource = unknownClasses;
        }

        /// <summary>
        /// Создание наименования колонок
        /// </summary>
        /// <param name="bindingName">Строка подключения/Наименование колонки</param>
        /// <returns>Колонка</returns>
        private DataGridTextColumn BuildingGridViewColumn(string bindingName) =>
            new() { Header = bindingName, Binding = new Binding(bindingName) { Mode = BindingMode.TwoWay } };

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

        private void GetPTable(object sender, RoutedEventArgs e)
        {
            List<string> mess = new();

            for (int i = 0; i < DataGridTable.Columns.Count(); i++)
            {
                mess.Add(UnknownClass.GetProperty(unknownClass!, DataGridTable.Columns[i].Header.ToString()!).ToString());
            }
            MessageBox.Show(string.Join("\n", mess));
        }
    }
}