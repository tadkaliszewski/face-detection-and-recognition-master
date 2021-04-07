using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
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
        private CascadeClassifier haarCascade;
        private List<FaceData> knownFacesList = new List<FaceData>();
        private VectorOfMat imageList = new VectorOfMat();
        private List<string> nameList = new List<string>();
        private VectorOfInt labelList = new VectorOfInt();
        private EigenFaceRecognizer recognizer;
        private Image<Gray, Byte> detectedFace = null;

        public WFFaceRecognitionFromImage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrepareKnownFacesList();

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

                detectedFace = oryginalImage.Copy(rectangle).Convert<Gray, byte>();

                FaceRecognition();


                /* break;
                
                try
                {
                    face = oryginalImage.GetSubRect(rectangle.Scale(50, 0, -10));
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                */
            }

            /*
            imgCamera.Source = BitmapToImageSource(
                face.AsBitmap()
                //oryginalImage.AsBitmap()
                );
            */
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

        private void FaceRecognition()
        {
            if (imageList.Size != 0)
            {
                //Eigen Face Algorithm
                FaceRecognizer.PredictionResult result = recognizer.
                    Predict(detectedFace.Resize(100, 100, Inter.Cubic));

                var faceName = nameList[result.Label];

                var cameraCaptureFace = detectedFace.ToBitmap();
            }
            else
            {
                /* FaceName = "Please Add Face"; */
            }
        }

        /// <summary>
        /// Wczytuje listę znanych twarzy i przygotowuje
        /// obiekt rozpoznawania
        /// </summary>
        public void PrepareKnownFacesList()
        {

            if (!File.Exists(Config.HaarCascadePath))
            {
                string text = "Cannot find Haar cascade data file:\n\n";
                text += Config.HaarCascadePath;
                MessageBoxResult result = MessageBox.Show(text, "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }

            haarCascade = new CascadeClassifier(Config.HaarCascadePath);

            knownFacesList.Clear();

            string line;

            if (!Directory.Exists(Config.FacePhotosPath))
            {
                Directory.CreateDirectory(Config.FacePhotosPath);
            }

            if (!File.Exists(Config.FaceListTextFile))
            {
                string text = "Cannot find face data file:\n\n";
                text += Config.FaceListTextFile + "\n\n";
                text += "If this is your first time running the app, an empty file will be created for you.";
                MessageBoxResult result = MessageBox.Show(text, "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        String dirName = Path.GetDirectoryName(Config.FaceListTextFile);
                        Directory.CreateDirectory(dirName);
                        File.Create(Config.FaceListTextFile).Close();
                        break;
                }
            }

            StreamReader reader = new StreamReader(Config.FaceListTextFile);
            int i = 0;

            while ((line = reader.ReadLine()) != null)
            {
                string[] lineParts = line.Split(':');
                var faceInstance = new FaceData();
                faceInstance.FaceImage = new Image<Gray, byte>
                    (Config.FacePhotosPath + lineParts[0] + Config.ImageFileExtension);
                faceInstance.PersonName = lineParts[1];
                knownFacesList.Add(faceInstance);
            }

            foreach (var face in knownFacesList)
            {
                imageList.Push(face.FaceImage.Mat);
                nameList.Add(face.PersonName);
                labelList.Push(new[] { i++ });
            }

            reader.Close();

            if (imageList.Size > 0)
            {
                recognizer = new EigenFaceRecognizer(imageList.Size);
                recognizer.Train(imageList, labelList);
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
