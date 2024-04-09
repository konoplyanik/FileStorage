using System.Windows;
using System.Windows.Media.Imaging;

namespace FileStorageClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _imagePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Открыть диалог выбора файла
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png, *.jpg, *.jpeg, *.gif)|*.png;*.jpg;*.jpeg;*.gif";
            if (dialog.ShowDialog() == true)
            {
                // Сохранить путь к выбранному файлу
                _imagePath = dialog.FileName;
                // Отобразить выбранную картинку
                ImagePreview.Source = new BitmapImage(new System.Uri(_imagePath));
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Получить текст из поля ввода
            string title = TitleTextBox.Text;

            // Здесь вы можете добавить код для отправки данных (title и _imagePath) на сервер
            MessageBox.Show($"Title: {title}, Image path: {_imagePath}");
        }
    }
}