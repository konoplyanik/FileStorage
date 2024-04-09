using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp.Models;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _imgPath;
        private List<ImageTextData> _uploadedImages = new List<ImageTextData>();
        private StackPanel _imagesPanel;

        public MainWindow()
        {
            InitializeComponent();
            _imagesPanel = new StackPanel(); // Инициализируем ImagesPanel
            _imagesPanel.Orientation = Orientation.Horizontal; // Устанавливаем ориентацию панели
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Открыть диалог выбора файла
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png, *.jpg, *.jpeg, *.gif)|*.png;*.jpg;*.jpeg;*.gif";
            if (dialog.ShowDialog() == true)
            {
                // Сохранить путь к выбранному файлу
                _imgPath = dialog.FileName;
                // Отобразить выбранную картинку
                ImagePreview.Source = new BitmapImage(new Uri(_imgPath));
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполнение полей
            if (string.IsNullOrEmpty(TitleTextBox.Text) || string.IsNullOrEmpty(_imgPath))
            {
                MessageBox.Show("Please fill in all the fields.");
                return;
            }

            try
            {
                // Отправить данные на веб-API
                await UploadToWebApiAsync(TitleTextBox.Text, _imgPath);
                MessageBox.Show("Data uploaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading data: {ex.Message}");
            }
        }

        private async Task UploadToWebApiAsync(string text, string imgPath)
        {
            var apiUrl = "https://localhost:7048/ImageText/save-image";

            using (var httpClient = new HttpClient())
            {
                var imageTextData = new ImageTextData
                {
                    Text = text,
                    Image = await File.ReadAllBytesAsync(imgPath)
                };

                var content = new StringContent(JsonSerializer.Serialize(imageTextData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                if (response.StatusCode.ToString() != "OK")
                {
                    MessageBox.Show($"ERROR: {response.StatusCode}");
                }
                else
                {
                    MessageBox.Show($"SUCCESS: {await response.Content.ReadAsStringAsync()}");
                }
            }
        }

        private async void GetImagesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получить список ранее загруженных изображений с сервера
                _uploadedImages = await GetImagesFromWebApiAsync();

                // Очистить панель для отображения изображений
                _imagesPanel.Children.Clear();

                // Отобразить ранее загруженные изображения
                foreach (var imageData in _uploadedImages)
                {
                    // Создать новое изображение и добавить его в панель
                    var image = new Image
                    {
                        Source = await ConvertByteArrayToBitmapImage(imageData.Image),
                        Height = 100,
                        Margin = new Thickness(10)
                    };
                    image.Stretch = System.Windows.Media.Stretch.Uniform;
                    _imagesPanel.Children.Add(image);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting images: {ex.Message}");
            }
        }

        private async Task<List<ImageTextData>> GetImagesFromWebApiAsync()
        {
            var apiUrl = "https://localhost:7048/ImageText/get-images";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                };

                var imageTextDataList = JsonSerializer.Deserialize<List<ImageTextData>>(jsonString, options);
                return imageTextDataList;
            }
        }

        private async Task<BitmapImage> ConvertByteArrayToBitmapImage(byte[] imageBytes)
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}