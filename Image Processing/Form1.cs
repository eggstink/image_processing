using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;


namespace Image_Processing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap loaded, result, imageA, imageB, colorgreen;

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1) && result != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        result.Save(saveFileDialog1.FileName + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1))
            {
                result = new Bitmap(loaded.Width, loaded.Height);

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        Color pixel = loaded.GetPixel(x, y);
                        result.SetPixel(x, y, pixel);
                    }
                }

                pictureBox2.Image = result;
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1))
            {
                result = new Bitmap(loaded.Width, loaded.Height);

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        Color pixel = loaded.GetPixel(x, y);
                        int gray = (pixel.R + pixel.G + pixel.B) / 3;
                        pixel = Color.FromArgb(gray, gray, gray);
                        result.SetPixel(x, y, pixel);
                    }
                }

                pictureBox2.Image = result;
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1))
            {
                result = new Bitmap(loaded.Width, loaded.Height);

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        Color pixel = loaded.GetPixel(x, y);
                        int fixedByte = 255;
                        pixel = Color.FromArgb(fixedByte - pixel.R, fixedByte - pixel.G, fixedByte - pixel.B);
                        result.SetPixel(x, y, pixel);
                    }
                }

                pictureBox2.Image = result;
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                functionsToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }

                    imageB = new Bitmap(openFileDialog2.FileName);
                    loaded = imageB;
                    pictureBox1.Image = imageB;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog3.ShowDialog() == DialogResult.OK)
            {
                functionsToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image.Dispose();
                    }

                    imageA = new Bitmap(openFileDialog3.FileName);
                    pictureBox2.Image = imageA;
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentDevice != null)
                {
                    currentDevice.Sendmessage();
                    IDataObject data = Clipboard.GetDataObject();

                    if (data != null && data.GetDataPresent(DataFormats.Bitmap))
                    {
                        Image temp = (Image)(data.GetData("System.Drawing.Bitmap", true));
                        imageA = new Bitmap(temp, imageB.Width, imageB.Height);
                    }
                    else
                    {
                        throw new Exception("Failed to capture frame from the webcam.");
                    }
                }
                else if (pictureBox2.Image != null)
                {
                    imageA = new Bitmap((Bitmap)pictureBox2.Image.Clone(), imageB.Width, imageB.Height);
                }
                else
                {
                    throw new Exception("No frame to capture.");
                }

                result = new Bitmap(imageB.Width, imageB.Height);
                Color mygreen = Color.FromArgb(0, 0, 255);
                int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
                int threshold = 5;

                for (int x = 0; x < imageB.Width; x++)
                {
                    for (int y = 0; y < imageB.Height; y++)
                    {
                        Color pixel = imageB.GetPixel(x, y);
                        Color backpixel = imageA.GetPixel(x, y);
                        int grey = (pixel.R + pixel.G + pixel.B) / 3;
                        int subtractvalue = Math.Abs(grey - greygreen);
                        if (subtractvalue > threshold)
                            result.SetPixel(x, y, pixel);
                        else
                            result.SetPixel(x, y, backpixel);
                    }
                }
                pictureBox3.Image = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing the images: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        Device currentDevice;
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Device[] devices = DeviceManager.GetAllDevices();
            currentDevice = devices[0];
            currentDevice.ShowWindow(pictureBox2);

            //IDataObject data;
            //Image temp;
            //Device d = devices[0];
            //d.Sendmessage();
            //data = Clipboard.GetDataObject();
            //temp = (Image)(data.GetData("System.Drawing.Bitmap", true));
            //Bitmap b = new Bitmap(temp);
            //pictureBox2.Image = b;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDevice.Stop();
            currentDevice = null;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1))
            {
                result = new Bitmap(loaded.Width, loaded.Height);

                int[] histogramData = new int[256];

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        Color pixel = loaded.GetPixel(x, y);
                        int gray = (pixel.R + pixel.G + pixel.B) / 3;
                        result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                        histogramData[gray]++;
                    }
                }

                Bitmap mydata = new Bitmap(256, 800);

                for (int x = 0; x < mydata.Width; x++)
                {
                    for (int y = 0; y < mydata.Height; y++)
                    {
                        mydata.SetPixel(x, y, Color.White);
                    }
                }

                for (int x = 0; x < mydata.Width; x++)
                {
                    for (int y = 0; y < Math.Min(histogramData[x] / 5, mydata.Height); y++)
                    {
                        mydata.SetPixel(x, (mydata.Height - 1) - y, Color.Black);
                    }
                }

                pictureBox2.Image = mydata;
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsPictureBoxNullOrEmpty(pictureBox1))
            {
                result = new Bitmap(loaded.Width, loaded.Height);

                for (int x = 0; x < loaded.Width; x++)
                {
                    for (int y = 0; y < loaded.Height; y++)
                    {
                        Color pixel = loaded.GetPixel(x, y);

                        int newR = (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
                        int newG = (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
                        int newB = (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);

                        newR = Math.Min(255, newR);
                        newG = Math.Min(255, newG);
                        newB = Math.Min(255, newB);

                        result.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                    }
                }

                pictureBox2.Image = result;
            }
            else
            {
                MessageBox.Show("No picture loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                functionsToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }

                    loaded = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = loaded;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsPictureBoxNullOrEmpty(PictureBox pictureBox)
        {
            return pictureBox == null || pictureBox.Image == null;
        }

    }
}
