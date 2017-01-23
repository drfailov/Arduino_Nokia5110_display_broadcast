using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nokia5100_ScreenCast_server
{
    public partial class Form1 : Form
    {
        SerialPort sp = null;
        int height = 48;
        int width = 84;
        String com = "COM9";

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
        }
        static public Bitmap Copy(Bitmap srcBitmap, Rectangle section)
        {
            // Create the new bitmap and associated graphics object
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the specified section of the source bitmap to the new one
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

            // Clean up
            g.Dispose();

            // Return the bitmap
            return bmp;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                com = (String)comboBox1.SelectedItem;
                Console.WriteLine("com = " + com);

                if(com != null && !com.Equals(""))
                while (true)
                {
                    drawScreenshot();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void drawScreenshot()
        {
            Bitmap bmToDraw = new Bitmap(width, height);

            using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0,
                        bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);
                }
                bmToDraw = Copy(bmpScreenCapture, new Rectangle(300, 300, width, height));
            }
            drawBitmap(bmToDraw);
        }

        private void drawBitmap(Bitmap bitmap)
        {
            int packet = 0;
            int packetMax = 7;
            bool[] pack = new bool[packetMax];
            sendClear();
            Thread.Sleep(10);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    pack[packet] = pixel.GetBrightness() < 0.8;
                    packet++;
                    if (packet >= packetMax)
                    {
                        packet = 0;
                        sendBinary(pack[6],pack[5],pack[4],pack[3],pack[2],pack[1],pack[0], false);
                        //Thread.Sleep(1);
                    }
                }
            }
            sendUpdate();
        }
        private void sendUpdate()
        {
            sendBinary(true, false, false, false, false, false, false, true);
        }

        private void sendClear()
        {
            sendBinary(false, true, false, false, false, false, false, true);
        }
        private void init()
        {
            if (sp == null)
            {
                try
                {
                    Console.WriteLine("com = " + com);
                    sp = new SerialPort(com, 250000);
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception = " + ex);
                    sp = null;
                }
            }
        }
        private void sendBinary(bool b1, bool b2, bool b3, bool b4, bool b5, bool b6, bool b7, bool b8)
        {
            init();
            if (sp != null)
            {
                int number = 0;
                number += b1 ? 1 : 0;
                number += b2 ? 2 : 0;
                number += b3 ? 4 : 0;
                number += b4 ? 8 : 0;
                number += b5 ? 16 : 0;
                number += b6 ? 32 : 0;
                number += b7 ? 64 : 0;
                number += b8 ? 128 : 0;
                //Console.WriteLine("num = " + number);
                byte c = (byte)number;
                try
                {
                    sp.Write(new byte[] { c }, 0, 1);
                }
                catch (Exception)
                {
                    sp = null;
                }
            }
        }
        private void close()
        {
            try
            {
                sp.Close();
            }
            catch (Exception)
            {

            }
        }
        private static void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
        }
    }
}
