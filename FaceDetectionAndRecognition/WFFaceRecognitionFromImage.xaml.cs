using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FaceDetectionAndRecognition
{
    /// <summary>
    /// Interaction logic for WFFaceRecognitionFromImage.xaml
    /// </summary>
    public partial class WFFaceRecognitionFromImage : Window
    {
        public WFFaceRecognitionFromImage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                DetectMultiScale(filename);
            }
        }

        /// <summary>
        /// Najlepsze parametry
        ///   var rectangles = haarCascade.DetectMultiScale(gray,
        ///       1.1, 5, new System.Drawing.Size(400, 400));
        /// </summary>
        /// <param name="fileName"></param>
        private void DetectMultiScale(string fileName)
        {
            var oryginalImage = new Image<Bgr, Byte>(fileName);

            Image<Gray, byte> gray = new Image<Gray, byte>(fileName);

            var haarCascade = new CascadeClassifier(Config.HaarCascadePath); ;

            var rectangles = haarCascade.DetectMultiScale(gray,
                1.1, 5, new System.Drawing.Size(400, 400));

            if (rectangles.Count() == 0)
            {
                MessageBox.Show("Nic!");
            }

            Image<Bgr, byte> face = null;

            foreach (var rectangle in rectangles)
            {
                /*oryginalImage.Draw(rectangle.Scale(50, 0, -10), 
                    new Bgr(System.Drawing.Color.White), 3);*/

                try
                {
                    face = oryginalImage.GetSubRect(rectangle.Scale(50, 0, -10));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            imgCamera.Source = BitmapToImageSource(
                face.AsBitmap()
                //oryginalImage.AsBitmap()
                );
        }

        /// <summary>
        /// Convert bitmap to bitmap image for image control
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <returns>Image Source</returns>
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

    }

    public static class Extensions
    {
        public static System.Drawing.Rectangle Scale
            (this System.Drawing.Rectangle value, decimal scale, 
                decimal shiftX = 0, decimal shiftY = 0)
        {
            var newWidth = (int)(value.Width + (value.Width * scale)/100);

            var newHeight = (int)(value.Height + (value.Height * scale) / 100);

            var newX = value.X - ((newWidth - value.Width) / 2);

            if (shiftX != 0)
            { 
                newX = (int)(newX + (newX * shiftX) / 100);
            }

            var newY = value.Y - ((newHeight - value.Height) / 2);

            if (shiftY != 0)
            {
                newY = (int)(newY + (newY * shiftY) / 100);
            }

            return new System.Drawing.Rectangle(newX, newY, newWidth, newHeight);
        }
    }

}
