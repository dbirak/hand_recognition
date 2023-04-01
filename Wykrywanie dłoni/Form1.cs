using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoStab;
using System.Diagnostics;
using System.IO;
using Emgu.CV.Util;
using Emgu.CV.Dnn;

namespace Wykrywanie_dłoni
{
    public partial class Form1 : Form
    {
        int picture1Width = 0;
        int picture1Height = 0;
        int picture2Width = 0;
        int picture2Height = 0;

        string text = "";

        int[,] coordinatesPoints1 = new int[21,2];
        int[,] coordinatesPoints2 = new int[21,2];

        Boolean obraz1 = false;
        Boolean obraz2 = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void wybierz_zdjecie_1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pliki .jpg |*.jpg|Pliki .png |*.png|Pliki .jepg |*.jepg|Pliki .bmp |*.bmp";

            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);

                    picture1Width = pictureBox1.Image.Width;
                    picture1Height = pictureBox1.Image.Height;

                    Bitmap masterImage = (Bitmap)pictureBox1.Image;

                    text = "Wymiary obrazu:\nW: " + picture1Width + " H: " + picture1Height + "\n";

                    wykryj_dlon(1, masterImage);
                    
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void wybierz_zdjecie_2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pliki .jpg |*.jpg|Pliki .png |*.png|Pliki .jepg |*.jepg|Pliki .bmp |*.bmp";

            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = new Bitmap(ofd.FileName);

                    picture2Width = pictureBox2.Image.Width;
                    picture2Height = pictureBox2.Image.Height;

                    Bitmap masterImage = (Bitmap)pictureBox2.Image;

                    text = "Wymiary obrazu:\nW: " + picture2Width + " H: " + picture2Height + "\n";

                    wykryj_dlon(2, masterImage);
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void wykryj_dlon(int number, Bitmap masterImage)
        {
            try
            {
                if (number == 1) obraz1 = false;
                if (number == 2) obraz2 = false;

                var img = masterImage.ToImage<Bgr, byte>();
                var blob = DnnInvoke.BlobFromImage(img, 1.0 / 255.0, new Size(368, 368), new MCvScalar(0, 0, 0));

                int[,] POSE_PAIRS = new int[,] { { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 4 }, { 0, 5 }, { 5, 6 }, { 6, 7 },
                    { 7, 8 }, { 0, 9 }, { 9, 10 }, { 10, 11 }, { 11, 12 }, { 0, 13 }, { 13, 14 }, { 14, 15 }, { 15, 16 },
                    { 0, 17 }, { 17, 18 }, { 18, 19 }, { 19, 20 } };

                string prototxt = @"..\..\pose_deploy.prototxt";
                string modelpath = @"..\..\pose_iter_102000.caffemodel";

                var net = DnnInvoke.ReadNetFromCaffe(prototxt, modelpath);

                net.SetInput(blob);
                net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);

                var output = net.Forward();

                var H = output.SizeOfDimension[2];
                var W = output.SizeOfDimension[3];

                var probMap = output.GetData();

                int nPoints = 21;

                var points = new List<Point>();

                for (int i = 0; i < nPoints; i++)
                {
                    Matrix<float> matrix = new Matrix<float>(H, W);
                    for (int row = 0; row < H; row++)
                    {
                        for (int col = 0; col < W; col++)
                        {
                            matrix[row, col] = (float)probMap.GetValue(0, i, row, col);
                        }
                    }


                    double minVal = 0, maxVal = 0;
                    Point minLoc = default, maxLoc = default;
                    CvInvoke.MinMaxLoc(matrix, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                    var x = (img.Width * maxLoc.X) / W;
                    var y = (img.Height * maxLoc.Y) / H;

                    var p = new Point(x, y);
                    points.Add(p);
                    CvInvoke.Circle(img, p, 5, new MCvScalar(0, 255, 0), -1);
                    CvInvoke.PutText(img, i.ToString(), p, FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);

                    text += "\n" + i + ": " + p.X + ", " + p.Y;

                    if (number == 1)
                    {
                        coordinatesPoints1[i, 0] = p.X;
                        coordinatesPoints1[i, 1] = p.Y;

                        obraz1 = true;
                    }
                    if (number == 2)
                    {
                        coordinatesPoints2[i, 0] = p.X;
                        coordinatesPoints2[i, 1] = p.Y;

                        obraz2 = true;
                    }

                }

                if(number == 1) richTextBox1.Text = text;
                if(number == 2) richTextBox3.Text = text;

                string text2 = "Wektory punktów:\n[A, B] -> [B.x-A.x, B.y-A.y]\n";
                
                for (int i = 0; i < POSE_PAIRS.GetLongLength(0); i++)
                {
                    var startIndex = POSE_PAIRS[i, 0];
                    var endIndex = POSE_PAIRS[i, 1];

                    if (points.Contains(points[startIndex]) && points.Contains(points[endIndex]))
                    {
                        CvInvoke.Line(img, points[startIndex], points[endIndex], new MCvScalar(255, 0, 0), 2);

                        if(number == 1) text2 += "\n[" + POSE_PAIRS[i, 0] + ", " + POSE_PAIRS[i, 1] + "] -> [" + (coordinatesPoints1[POSE_PAIRS[i, 1], 0] - coordinatesPoints1[POSE_PAIRS[i, 0], 0]) + ", " + (coordinatesPoints1[POSE_PAIRS[i, 1], 1] - coordinatesPoints1[POSE_PAIRS[i, 0], 1]) + "]";
                        if(number == 2) text2 += "\n[" + POSE_PAIRS[i, 0] + ", " + POSE_PAIRS[i, 1] + "] -> [" + (coordinatesPoints2[POSE_PAIRS[i, 1], 0] - coordinatesPoints2[POSE_PAIRS[i, 0], 0]) + ", " + (coordinatesPoints2[POSE_PAIRS[i, 1], 1] - coordinatesPoints2[POSE_PAIRS[i, 0], 1]) + "]";
                    }
                }

                if (number == 1)
                {
                    pictureBox1.Image = img.ToBitmap();
                    richTextBox4.Text = text2;
                }
                if (number == 2)
                {
                    pictureBox2.Image = img.ToBitmap();
                    richTextBox5.Text = text2;
                }

                if (obraz1 == true && obraz2 == true) porownaj(number);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void porownaj(int number)
        {
            text = "Wymiary obrazu:\nW: " + picture1Width + " H: " + picture1Height + "\n";

            for(int i=0; i<21; i++)
            {
                text += "\n" + i + ": " + Math.Round((Convert.ToDouble(picture1Width) * Convert.ToDouble(coordinatesPoints2[i,0])) / Convert.ToDouble(picture2Width)) + ", " + Math.Round((Convert.ToDouble(picture1Height) * Convert.ToDouble(coordinatesPoints2[i, 1])) / Convert.ToDouble(picture2Height));
            }

            richTextBox2.Text = text;

        }
    }
}
