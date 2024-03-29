﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Calibracion_de_camara
{
    public partial class Form1 : Form
    {

        VectorOfPointF corners1, corners2;
        VideoCapture capture;
        Image<Bgr, byte> imgTry;
        Image imgdot;
        Mat H;

        Image<Bgr, Byte> imgOriginal;

        public Form1()
        {
            InitializeComponent();
            Run();
        }

        private void Run()
        {
            try
            {
                //imgdot = Image.FromFile("C:\\Users\\William\\Desktop\\Eric PTV Ediciones\\PTV semana 24-11 1366 x 768 22-01\\PPTV_MainModule\\bin\\Debug\\Imagenes\\dot10x10.png");
                capture = new VideoCapture("http://" + "root" + ":" + "PPTV" + "@" + "192.168.2.2" + "/axis-cgi/mjpg/video.cgi?resolution=640x360&req_fps=30&.mjpg");


                

                /*int CuadLarg = 9;
                int CuadAlt = 7;
                Size patternSize = new Size(CuadLarg - 1, CuadAlt - 1);
                int CuadTot = (CuadLarg - 1) * (CuadAlt - 1);

                corners2 = new VectorOfPointF();
                Image<Bgr, byte> imgtest1 = capture.QuerySmallFrame().ToImage<Bgr, Byte>();

                Boolean found = CvInvoke.FindChessboardCorners(imgtest1, patternSize, corners2);
                Console.WriteLine(corners2.Size);
                PointF[] corners = corners2.ToArray();
                PointF[] srcs = new PointF[4];  // esquinas de la cuadricula proyectada bajo el punto de vista de la camara (Rsolucion camara 640 x 360 )
                srcs[0] = corners[0];
                srcs[1] = corners[7];
                srcs[2] = corners[40];
                srcs[3] = corners[47];

                PointF[] dsts = new PointF[4];   // esquinas de la cuadricula proyectada bajo el punto de vista real (Escenario) (Rsolucion proyector 1280 x 768 )

                int w = imageBox4.Size.Width;
                int h = imageBox4.Size.Height;
                dsts[0] = new PointF(100, 100);
                dsts[1] = new PointF(w-100, 100);
                dsts[2] = new PointF(100, h-100);
                dsts[3] = new PointF(w-100, h-100);
                if (found)
                {
                    CvInvoke.DrawChessboardCorners(imgtest1, patternSize, corners2, found);
                    imageBox3.Image = imgtest1;
                    Mat img1_warp = new Mat();
                    Mat H = CvInvoke.GetPerspectiveTransform(srcs, dsts);

                    var img1 = imgtest1;

                    CvInvoke.WarpPerspective(img1, img1_warp, H, img1.Size);
                    imageBox4.Image = img1_warp;
                }*/
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return;
            }
            Application.Idle += ProcessFrame;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            imageBox1.Image = capture.QuerySmallFrame();
            if (capture != null)
            {
                Image<Gray, Byte> imgProcessed;
                imgOriginal = capture.QuerySmallFrame().ToImage<Bgr, Byte>();
                imgProcessed = imgOriginal.Convert<Gray, byte>();
                imgProcessed = imgProcessed.InRange(new Gray(250), new Gray(256));   // 239 - 255
                imgProcessed = imgProcessed.SmoothGaussian(9);
                CircleF[] circles = imgProcessed.HoughCircles(new Gray(1), new Gray(1), 1, imgProcessed.Height / 4, 1, 4)[0];
                foreach (var CircleF in circles)
                {
                    var center = new Point((int)CircleF.Center.X, (int)CircleF.Center.Y);
                    CvInvoke.Circle(imgOriginal, center, 4, new MCvScalar(0, 0, 255),                                 // draw pure red
                                                    -1,
                                                    LineType.Filled,                                    // use AA to smooth the pixels
                                                    0);
                }

                img2.Image = imgOriginal;
                if (H != null) {
                    Mat img1_warp = new Mat();
                    CvInvoke.WarpPerspective(imgOriginal, img1_warp, H, imgOriginal.Size);
                    //corners = CvInvoke.PerspectiveTransform(corners, H);
                    Image<Bgr, byte> img1_w_1 = img1_warp.ToImage<Bgr, byte>();

                    imgProcessed = img1_w_1.Convert<Gray, byte>();
                    imgProcessed = imgProcessed.InRange(new Gray(239), new Gray(255));   // 239 - 255
                    imgProcessed = imgProcessed.SmoothGaussian(3);
                    circles = imgProcessed.HoughCircles(new Gray(1), // canny threshold
                                                                            new Gray(1), // acumulator threshold
                                                                            1,  // size of the image / this param. = "acumulator resolution"
                                                                            imgProcessed.Height / 4, // imgProcessed.Height / 4 min distance in pixels between the center of the detected circles
                                                                            1,   // min radius detected circles
                                                                            4)[0]; // max radius detected circle. get circles from the first channel

                    foreach (var CircleF in circles)
                    {
                        var center = new Point((int)CircleF.Center.X, (int)CircleF.Center.Y);
                        CvInvoke.Circle(img1_w_1, center, 4, new MCvScalar(0, 0, 255),                                 // draw pure red
                                                        -1,
                                                        LineType.Filled,                                    // use AA to smooth the pixels
                                                        0);
                    }
                    imageBox4.Image = img1_w_1;
                }
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                Image<Gray, Byte> imgProcessed;
                imgOriginal = capture.QuerySmallFrame().ToImage<Bgr, Byte>();
                imgProcessed = imgOriginal.Convert<Gray, byte>();
                imgProcessed = imgProcessed.InRange(new Gray(254), new Gray(256));   // 239 - 255
                imgProcessed = imgProcessed.SmoothGaussian(9);
                CircleF[] circles = imgProcessed.HoughCircles(new Gray(0), new Gray(1), 1, imgProcessed.Height, 1,4)[0];
                imageBox4.Image = imgProcessed;

                foreach (var CircleF in circles)
                {
                    var center = new Point((int)CircleF.Center.X, (int)CircleF.Center.Y);
                    CvInvoke.Circle(imgOriginal, center, 4, new MCvScalar(0, 0, 255),                                 // draw pure red
                                                    -1,           
                                                    LineType.Filled,                                    // use AA to smooth the pixels
                                                    0);
                }
                img2.Image = imgOriginal;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int CuadLarg = 9;
            int CuadAlt = 7;
            Size patternSize = new Size(CuadLarg - 1, CuadAlt - 1);
            int CuadTot = (CuadLarg - 1) * (CuadAlt - 1);

            corners2 = new VectorOfPointF();
            Image<Bgr, byte> imgtest1 = capture.QuerySmallFrame().ToImage<Bgr, Byte>();

            Boolean found = CvInvoke.FindChessboardCorners(imgtest1, patternSize, corners2);
            Console.WriteLine(corners2.Size);
            PointF[] corners = corners2.ToArray();
            PointF[] srcs = new PointF[4];  // esquinas de la cuadricula proyectada bajo el punto de vista de la camara (Rsolucion camara 640 x 360 )
            srcs[0] = corners[0];
            srcs[1] = corners[7];
            srcs[2] = corners[40];
            srcs[3] = corners[47];
            
            PointF[] dsts = new PointF[4];   // esquinas de la cuadricula proyectada bajo el punto de vista real (Escenario) (Rsolucion proyector 1280 x 768 )

            int w =imageBox4.Size.Width;
            int h = imageBox4.Size.Height;
            dsts[0] = new PointF(214/6, 160/6);
            dsts[1] = new PointF(1680/6, 160/6);
            dsts[2] = new PointF(214/6, 902/6);
            dsts[3] = new PointF(1680/6, 902/6);

            int x = (int)Math.Ceiling(corners[0].X - (corners[1].X - corners[0].X)); //Puntos iniciales para rectángulo de calibración.
            int y = (int)Math.Ceiling(corners[0].Y - (corners[CuadLarg].Y - corners[0].Y));
            int wid = (int)Math.Abs((int)Math.Ceiling(corners[CuadTot - 1].X) - x + (corners[CuadTot - 1].X - corners[CuadTot - 2].X)); //Ancho para el réctangulo de calibración.
            int hei = (int)Math.Abs((int)Math.Ceiling(corners[CuadTot - 1].Y) - y + (corners[CuadTot - 1].Y - corners[CuadTot - CuadLarg].Y)); //Alto para el recángulo de calibración.

            if (found)
            {
                //CvInvoke.DrawChessboardCorners(imgtest1, patternSize, corners2, found);
                Rectangle rectangle = new Rectangle(x,y,wid,hei);
                


                //drawChessboard(corners);
                imgtest1.Draw(rectangle, new Bgr(Color.Red), 2);
                imageBox3.Image = imgtest1;
                Mat img1_warp = new Mat();
                H = CvInvoke.GetPerspectiveTransform(srcs, dsts);

                var img1 = imgtest1;

                CvInvoke.WarpPerspective(img1, img1_warp, H, img1.Size);
                corners = CvInvoke.PerspectiveTransform(corners, H);
                Image<Bgr, byte> img1_w_1 = img1_warp.ToImage<Bgr, byte>();
                foreach (var punt in corners)
                {
                    CircleF circle = new CircleF(punt, 0);

                    img1_w_1.Draw(circle, new Bgr(Color.Red), 2);
                }

                imageBox4.Image = img1_w_1;
            }
        }


        public void drawChessboard(PointF[] corners)
        {
            int index;
            int positionX, positionY;

            for (index = 0; index < corners.Length; index++)
            {
                positionX = (int)((corners[index].X));
                positionY = (int)((corners[index].Y));
                drawImageScenario(new Point(positionX - 5, positionY - 5), imgdot);   // el '-5' se debe a que la imagen que se dibuja es de 10 pixeles
            }

            //formEscenario.updateHomography();   // update the escenerio homography matrix with the new one
        }

        delegate void drawImageScenarioDelegate(PointF position, Image imagen);
        private void drawImageScenario(PointF position, Image imagen) //la escala para la cuando hay sesiones a distancia
        {
            
            if (img2.InvokeRequired)
            {
                drawImageScenarioDelegate d = new drawImageScenarioDelegate(drawImageScenario);
                BeginInvoke(d, new object[] { position, imagen });
            }
            else
            {
                Graphics g = img2.CreateGraphics();
                g.DrawImage(imagen, position.X, position.Y, imagen.Width, imagen.Height); // el '-6' se debe a que la imagen que se dibuja es de 12 pixeles
                
                g.Dispose();
            }
        }
    }
}
