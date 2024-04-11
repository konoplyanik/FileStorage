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

namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string _imgPath;
    private List<ImageTextData> _uploadedImages = new List<ImageTextData>();

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
            ImagesPanel.Children.Clear();

            // Создать панель для отображения текста и изображений
            var mainPanel = new Grid
            {
                Margin = new Thickness(10), // Добавить отступы для всей панели
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            // Создать заголовки "Images" и "Text"
            var imageHeader = new TextBlock { Text = "Images", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 10) };
            var textHeader = new TextBlock { Text = "Text", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 0, 0, 10) };
            Grid.SetColumn(imageHeader, 0);
            Grid.SetColumn(textHeader, 1);
            mainPanel.Children.Add(imageHeader);
            mainPanel.Children.Add(textHeader);

            // Создать вертикальный StackPanel для отображения текста и изображений
            var textImagePanel = new StackPanel { Orientation = Orientation.Vertical };

            // Отобразить ранее загруженные изображения
            foreach (var imageData in _uploadedImages)
            {
                // Создать новый элемент со стеком текста и изображения
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 10) };

                // Добавить текст
                var textPanel = new StackPanel { Orientation = Orientation.Vertical };
                var textBlock = new TextBlock { Text = imageData.Text, TextWrapping = TextWrapping.Wrap, MaxWidth = 300 };
                textPanel.Children.Add(textBlock);
                stackPanel.Children.Add(textPanel);

                // Добавить изображение
                var image = new Image
                {
                    Source = await ConvertByteArrayToBitmapImage(imageData.Image),
                    Height = 100,
                    Width = 100,
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    Margin = new Thickness(20, 0, 0, 0) // Обновленные отступы для изображения
                };
                stackPanel.Children.Add(image);

                // Добавить элемент в вертикальный StackPanel
                textImagePanel.Children.Add(stackPanel);
            }


            // Добавить вертикальный StackPanel в Grid
            Grid.SetColumn(textImagePanel, 0);
            Grid.SetColumn(textHeader, 0);
            Grid.SetColumn(imageHeader, 1);
            mainPanel.Children.Add(textImagePanel);

            // Добавить mainPanel в ImagesPanel
            ImagesPanel.Children.Add(mainPanel);
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