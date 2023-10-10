// See https://aka.ms/new-console-template for more information
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Drawing;
using Tesseract;
using Emgu.CV.Face;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Reflection;

Console.WriteLine("Hello, World!");


string imagePath = "C:\\Users\\chris\\Desktop\\Proyect\\image\\example6.jpeg";
string tessDataPath = @"./tessdata"; // Change to your Tesseract data path

using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.Color))
using (Mat gray = new Mat())
using (Mat imagenResize = new Mat())
using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
{
    CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
    CvInvoke.Resize(image, imagenResize, new Size(800, 600), 0, 0, Inter.Linear);
    CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 0);
    CvInvoke.Canny(gray, gray, 150, 200);
    CvInvoke.Dilate(gray, gray, null, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(1));

    CvInvoke.FindContours(gray, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);


    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
    path = Path.Combine(path, "tessdata");
    path = path.Replace("file:\\", "");
    using (var engine = new TesseractEngine(path, "eng", EngineMode.Default))
    {
        for (int i = 0; i < contours.Size; i++)
        {
            VectorOfPoint contour = contours[i];
            double area = CvInvoke.ContourArea(contour);
            Rectangle rect = CvInvoke.BoundingRectangle(contour);
            double epsilon = 0.03 * CvInvoke.ArcLength(contour, true);
            VectorOfPoint approx = new VectorOfPoint();
            CvInvoke.ApproxPolyDP(contour, approx, epsilon, true);

            if (approx.Size >= 4 && area > 2100)
            {
                double aspectRatio = (double)rect.Width / rect.Height;
                if (aspectRatio > 2.4)
                {
                    Mat placa = new Mat(gray, rect);

                    CvInvoke.Imshow("Image", placa);
                    CvInvoke.WaitKey(0);
                    CvInvoke.DestroyAllWindows();

                    string tempImagePath = "temp_image.png";
                    CvInvoke.Imwrite(tempImagePath, placa);
                    // Cargar el archivo de imagen temporal en un objeto Pix

                    string dataProof = "C:\\Users\\chris\\source\\repos\\PuebaVA\\PuebaVA\\bin\\Debug\\net7.0\\temp_image.png";

                    using (Tesseract.Pix pix = Tesseract.Pix.LoadFromFile(dataProof))
                    {
                        using (Tesseract.Page page = engine.Process(pix, PageSegMode.SingleWord))
                        {
                            string text = page.GetText();
                            string newPlate = text.Substring(0, text.Length - 2);

                            Console.WriteLine("PLACA: " + newPlate);

                            CvInvoke.Rectangle(image, rect, new MCvScalar(0, 255, 0), 3);
                            CvInvoke.PutText(image, newPlate, new Point(rect.X - 20, rect.Y - 10), FontFace.HersheyDuplex, 2.2, new MCvScalar(0, 255, 0), 3);
                            CvInvoke.Resize(image, imagenResize, new Size(800, 600), 0, 0, Inter.Linear);
                        }
                    }
                }
            }
        }
    }

    CvInvoke.Imshow("Image", image);
    CvInvoke.WaitKey(0);
    CvInvoke.DestroyAllWindows();
}