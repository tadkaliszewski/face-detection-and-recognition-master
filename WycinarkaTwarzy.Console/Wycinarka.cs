using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.IO;
using System.Linq;

namespace WycinarkaTwarzy.Console
{
    public class Wycinarka
    {


        /// <summary>
        /// Najlepsze parametry
        ///   var rectangles = haarCascade.DetectMultiScale(gray,
        ///     1.1, 5, new System.Drawing.Size(400, 400));
        /// </summary>
        /// <param name="fileName"></param>
        public void DetectMultiScale(string fileName)
        {
            var oryginalImage = new Image<Bgr, Byte>(fileName);

            Image<Gray, byte> gray = new Image<Gray, byte>(fileName);

            var haarCascade = new CascadeClassifier(Config.HaarCascadePath); ;

            var rectangles = haarCascade.DetectMultiScale(gray,
                1.1, 5, new System.Drawing.Size(400, 400));

            if (rectangles.Count() == 0)
            {
                /* MessageBox.Show("Nic!"); */
                System.Console.WriteLine(" - nie znaleziono twarzy.");
            }
            else
            {
                System.Console.WriteLine($" - znaleziono {rectangles.Count()} twarz(y).");
            }

            Image<Bgr, byte> face = null;

            var n = 0;

            foreach (var rectangle in rectangles)
            {
                /*oryginalImage.Draw(rectangle.Scale(50, 0, -10), 
                    new Bgr(System.Drawing.Color.White), 3);*/

                try
                {
                    face = oryginalImage.GetSubRect(rectangle.Scale(50, 0, -10));

                    var fileInfo = new FileInfo(fileName);

                    var newfileName = $"C:\\Tmp\\Whatsup Wycinarka Out\\{fileInfo.Name}-{n++}.jpg";

                    System.Console.WriteLine($" - zapisuję: {newfileName}");

                    face.AsBitmap().Save(newfileName, 
                        System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(" - błąd wyciągania twarzy.");
                    /* MessageBox.Show(ex.ToString()); */
                }
            }

            /*
            imgCamera.Source = BitmapToImageSource(
                face.AsBitmap()
                //oryginalImage.AsBitmap()
                );*/
        }





    }
}
