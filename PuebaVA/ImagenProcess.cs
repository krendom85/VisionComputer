using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuebaVA
{
    internal class ImagenProcess
    {
        public string ImagePath { get; set; }

        public Mat ResizeImage(Mat image, int width, int heigth)
        {
            Mat newImage = new Mat();
            CvInvoke.Resize(image, newImage, new Size(width, heigth), 0, 0, Inter.Linear);
            return newImage;
        }

        public VectorOfVectorOfPoint GetContorns()
        {
            Mat image = CvInvoke.Imread(this.ImagePath, ImreadModes.Color);
            Mat gray = new Mat();

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);
            CvInvoke.GaussianBlur(gray, gray, new Size(3, 3), 0);
            CvInvoke.Canny(gray, gray, 150, 200);
            CvInvoke.Dilate(gray, gray, null, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(1));

            CvInvoke.FindContours(gray, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            
            return contours;
        }

        public void GetText()
        {

        }
    }
}
