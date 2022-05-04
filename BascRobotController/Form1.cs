using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BascRobotController
{
    public partial class Form1 : Form
    {
        // declares variables and instantiates the "Robot" class at runtime
        Robot robot;
        private VideoCapture _capture;
        private Thread _capturethread;
        private int _thresholdHue = 0;
        private int _thresholdSat = 0;
        private int _thresholdVal = 0;
        private int _thresholdHueMin = 0;
        private int _thresholdSatMin = 0;
        private int _thresholdValMin = 0;
        private int _thresholdRedHMin = 0;
        private int _thresholdRedSMin = 0;
        private int _thresholdRedVMin = 0;
        private int _thresholdRedHMax = 0;
        private int _thresholdRedSMax = 0;
        private int _thresholdRedVMax = 0;
        public Form1()
        {
            InitializeComponent();
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            // creates class reference and sets the COM port for serial communication. Port may need to be changed with new connections
            robot = new Robot("COM5");
            //creates videocapture using the webcam (denoted by the 1 for the respective port)
            _capture = new VideoCapture(1);
            _capturethread = new Thread(DisplayWebcam);
            _capturethread.Start();
        }
        
        private void DisplayWebcam()
        {
            while (_capture.IsOpened)
            {
                //creates a "mat" that stores the images gatered by webcam. This "raw image" is used as the base in following mats 
                //and filters
                Mat frame = _capture.QueryFrame();

                //resizes image to fit picture box.
                int newHeight = (frame.Size.Height * pictureBox5.Size.Width) / frame.Size.Width;
                Size newSize = new Size(pictureBox5.Size.Width, newHeight);
                CvInvoke.Resize(frame, frame, newSize);
                pictureBox5.Image = frame.Bitmap;
                
                //creaetes base frame to set up HSV Filter to find yellow
                Mat hsvFrame = new Mat();
                CvInvoke.CvtColor(frame, hsvFrame, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
                //splits the base hsv frame into the three H.S.V branches
                Mat[] hsvChannels = hsvFrame.Split();

                Mat Huefilter = new Mat();
                CvInvoke.InRange(hsvChannels[0], new ScalarArray(20), new ScalarArray(179), Huefilter);
                pictureBox2.Image = Huefilter.Bitmap;

                Mat Satfilter = new Mat();
                CvInvoke.InRange(hsvChannels[1], new ScalarArray(75), new ScalarArray(200), Satfilter);
                pictureBox3.Image = Satfilter.Bitmap;

                Mat Valfilter = new Mat();
                CvInvoke.InRange(hsvChannels[2], new ScalarArray(200), new ScalarArray(255), Valfilter);
                pictureBox4.Image = Valfilter.Bitmap;
                //merges the hue, saturation, and values back into a solitary frame which will detect the 
                // desired olor, in this case yellow
                Mat MergedImage = new Mat();
                CvInvoke.BitwiseAnd(Huefilter, Satfilter, MergedImage);
                CvInvoke.BitwiseAnd(MergedImage, Valfilter, MergedImage);
                pictureBox1.Image = MergedImage.Bitmap;

                //repeats above HSV steps for the color red
                Mat RedHsv = new Mat();
                CvInvoke.CvtColor(frame, RedHsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
                Mat[] RedHsvChan = RedHsv.Split();

                Mat RedHue = new Mat();
                CvInvoke.InRange(RedHsvChan[0], new ScalarArray(0), new ScalarArray(12), RedHue);
                pictureBox7.Image = RedHue.Bitmap;

                Mat RedSat = new Mat();
                CvInvoke.InRange(RedHsvChan[1], new ScalarArray(125), new ScalarArray(255), RedSat);
                pictureBox8.Image = RedSat.Bitmap;

                Mat RedVal = new Mat();
                CvInvoke.InRange(RedHsvChan[2], new ScalarArray(125), new ScalarArray(255), RedVal);
                pictureBox9.Image = RedVal.Bitmap;
                
                //merges the red hsv filters
                Mat MergedRed = new Mat();
                CvInvoke.BitwiseAnd(RedHue, RedSat, MergedRed);
                CvInvoke.BitwiseAnd(MergedRed, RedVal, MergedRed);
                pictureBox6.Image = MergedRed.Bitmap;

                //display the pixels deemed red as white and all others as black
                //then counts how many white pixels are in the frame at any time
                Image<Gray, byte> img2 = MergedRed.ToImage<Gray, byte>();

                int RedPix = 0;
                for (int y = 0; y< MergedRed.Height; y++)
                {
                    for (int x = 0; x < MergedRed.Width; x++)
                    {
                        if (img2.Data[y, x, 0] == 255) RedPix++;
                        
                    }
                }

                //displays red pixel count on label
                Invoke(new Action(() => { label7.Text = $"{RedPix} red pixels in frame"; }));

                //displays all yellow pixels as white and others as black.
                Image<Gray, byte> img = MergedImage.ToImage<Gray, byte>();
                int MergedImageSlice = MergedImage.Width / 7;
                int WhitePixLLLF = 0;
                int WhitePixLLF = 0;
                int WhitePixLF = 0;
                int WhitePixM = 0;
                int WhitePixRF = 0;
                int WhitePixRRF = 0;
                int WhitePixRRRF = 0;
                
                //slices the image into 7 equal vertical sections and tallies white pixels
                for (int y = 0; y < MergedImage.Height; y++)
                {
                    for (int x = 0; x < MergedImageSlice; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixLLLF++;
                        
                    }
                    for (int x = MergedImageSlice; x < MergedImageSlice * 2; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixLLF++;
                        
                    }
                    for (int x = MergedImageSlice * 2; x < MergedImageSlice * 3; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixLF++;
                        
                    }
                    for (int x = MergedImageSlice * 3; x < MergedImageSlice * 4; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixM++;
                        
                    }
                    for (int x = MergedImageSlice * 4; x < MergedImageSlice*5; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixRF++;
                        
                    }
                    for (int x = MergedImageSlice*5; x < MergedImageSlice * 6; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixRRF++;
                    }
                    for (int x = MergedImageSlice*6; x< MergedImage.Width; x++)
                    {
                        if (img.Data[y, x, 0] == 255) WhitePixRRRF++;
                    }
                }
                Invoke(new Action(() =>
                {
                    //updates labels with white pixel count for yellow
                    label1.Text = $"{WhitePixLLF} pixels in slice one";
                    label2.Text = $"{WhitePixLF} pixels in slice two";
                    label3.Text = $"{WhitePixM} pixels in slice three";
                    label4.Text = $"{WhitePixRF} pixels in slice four";
                    label5.Text = $"{WhitePixRRF} pixels in slice 5";
                    label8.Text = $"{_thresholdHueMin} hue min slider";
                    label9.Text = $"{_thresholdRedHMax} red hue max slider";
                    label11.Text = $"{_thresholdRedVMin} red val min slider";
                    label12.Text = $"{_thresholdRedVMax} red val max slider";
                }));
                //uses white pixel count of red and white to direct the robot to either drive straight, slightly left
                // abrubtly left, slightly right, abruptly right, or stopping.
                if (RedPix > 1000) { robot.Move(Robot.STOP); Invoke(new Action(() => { label6.Text = "Stopping"; })); }
                else if (WhitePixM > 600 ) { robot.Move(Robot.STRAIGHT); Invoke(new Action(() => { label6.Text = "Driving Straight"; })); }
                else if ( WhitePixLF > 500) { robot.Move(Robot.LEFTSLOW); Invoke(new Action(() => { label6.Text = "Slightly turning left"; })); }
                else if (WhitePixLLF > 500) { robot.Move(Robot.LEFT); Invoke(new Action(() => {label6.Text = "Turning left abruptly"; })); }
                else if ( WhitePixLLLF > 500) { robot.Move(Robot.LEFT); Invoke(new Action(() => { label6.Text = "Turning left abruptly"; })); }
                else if ( WhitePixRF > 500) { robot.Move(Robot.RIGHTSLOW); Invoke(new Action(() => { label6.Text = "Slightly turning right"; })); }
                else if ( WhitePixRRF > 500) { robot.Move(Robot.RIGHT); Invoke(new Action(() => { label6.Text = "Turning right abruptly"; })); }
                else if ( WhitePixRRRF > 500) { robot.Move(Robot.RIGHT); Invoke(new Action(() => { label6.Text = "Turning right abruptly"; })); }
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _thresholdHue = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            _thresholdSat = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _thresholdVal = trackBar3.Value;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _capturethread.Abort();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            _thresholdHueMin = trackBar4.Value;
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            _thresholdSatMin = trackBar5.Value;
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            _thresholdValMin = trackBar6.Value;
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            _thresholdRedHMin = trackBar7.Value;
        }

        private void trackBar8_Scroll(object sender, EventArgs e)
        {
            _thresholdRedSMin = trackBar8.Value;
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            _thresholdRedVMin = trackBar9.Value;
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            _thresholdRedHMax = trackBar10.Value;
        }

        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            _thresholdRedSMax = trackBar11.Value;
        }

        private void trackBar12_Scroll(object sender, EventArgs e)
        {
            _thresholdRedVMax = trackBar12.Value;
        }

        
    }
}
