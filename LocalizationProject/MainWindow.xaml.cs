using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LocalizationProject
{
    public partial class MainWindow : Window
    {
        private readonly List<(string fileName, string KeyPrefix, string Key, string Text)> list = [];

        private readonly List<(string name, Type type)> headerColumns = [];

        private readonly ObservableCollection<object> unknownClasses = [];

        public string ResourceDictionaryPatternOpeningTeg { get; set; } = "<ResourceDictionary xmlns=\"https://github.com/avaloniaui\"\n" +
                                                                          "\t\txmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n" +
                                                                          "\t\txmlns:system=\"clr-namespace:System;assembly=System.Runtime\">\n";

        public string ResourceDictionaryPatternClosingTeg { get; set; } = "\r\n</ResourceDictionary>";

        public MainWindow()
        {
            headerColumns.Add(("Key", typeof(string)));

            InitializeComponent();
        }

        /// <summary>
        /// Формирование таблицы
        /// </summary>
        private void BuildingTable()
        {
            object unknownClass1 = UnknownClass.BuildingClass($"MainClass", headerColumns);

            unknownClasses.Add(unknownClass1);
        }

        /// <summary>
        /// Создание наименования колонок
        /// </summary>
        /// <param name="bindingName">Строка подключения/Наименование колонки</param>
        /// <returns>Колонка</returns>
        private DataGridTextColumn BuildingGridViewColumn(string bindingName) =>
            new()
            {
                Header = bindingName,
                Binding = new Binding(bindingName)
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            };

        #region Обработчики

        /// <summary>
        /// Сформированные таблицы
        /// </summary>
        private void AssembleTable(object sender, RoutedEventArgs e)
        {
            BuildingTable();

            DataGridTable.Columns.Clear();

            DataGridTable.ItemsSource = unknownClasses;

            FormButton.IsEnabled = false;
            Language.IsEnabled = false;
            AddLanguageBtn.IsEnabled = false;

            CreationFileBtn.IsEnabled = true;

        }

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

        private void CreationFileWithLocalization(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidationStringField() == false)
                    return;


                for (int i = 1; i < DataGridTable.Columns.Count; i++)
                    for (int j = 0; j < unknownClasses.Count; j++)
                        list.Add(
                                    (
                                        DataGridTable.Columns[i].Header.ToString()!, // наименование языка
                                        KeyPrefix.Text.Trim(),
                                        UnknownClass.GetProperty(unknownClasses[j], DataGridTable.Columns[0].Header.ToString()!).ToString()!, // ключ (для обращения к локализации в проекте)
                                        UnknownClass.GetProperty(unknownClasses[j], DataGridTable.Columns[i].Header.ToString()!).ToString()! // сам текст
                                    )
                                );

                CreationFile();

                MessageBox.Show($"Файл(ы) локализации созданы по пути: {FilePath.Text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создание необходимой строки
        /// </summary>
        /*<system:String x:Key="{Key-Prefix}.{Key}">{Text}</system:String>*/
        private string BuildingStrings(string _keyPrefix, string _key, string _text) =>
            $"<system:String x:key=\"{_keyPrefix}.{_key}\">{_text}</system:String>";

        private string? TextConstruction(string _fileName)
        {
            if (ValidationStringField() == false)
                return null;

            string text = ResourceDictionaryPatternOpeningTeg;

            foreach (var item in list)
                if (item.fileName == _fileName)
                    text += $"\t{BuildingStrings(item.KeyPrefix, item.Key, item.Text)}\n";

            text += ResourceDictionaryPatternClosingTeg;

            return text;
        }

        private void CreationFile()
        {
            List<string> fileNames = [];

            foreach (var item in list)
                if (fileNames.Contains(item.fileName) == false)
                    fileNames.Add(item.fileName);

            foreach (var item in fileNames)
                File.WriteAllText($"{FilePath.Text.Trim()}\\{item}.axaml", TextConstruction(item));
        }

        private bool ValidationStringField()
        {
            if (KeyPrefix.Text == null || KeyPrefix.Text == "" ||
                FilePath.Text == null || FilePath.Text == "")
            {
                MessageBox.Show("Что то пошло не так, прошу проверить все ли поля заполнены корректно");
                return false;
            }
            return true;
        }

        private void ChoosingPathBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                FilePath.Text = dialog.FolderName;
            }
        }
    }
}
