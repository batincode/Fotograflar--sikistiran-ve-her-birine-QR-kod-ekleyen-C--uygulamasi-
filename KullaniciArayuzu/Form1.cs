using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;



namespace KullaniciArayuzu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;

            }
        }
        private Bitmap GenerateQRCode(string data, int width, int height)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new QrCodeEncodingOptions
            {
                Width = width,
                Height = height
            };

            Bitmap qrCodeBitmap = barcodeWriter.Write(data);
            return qrCodeBitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string kaynakKlasor = textBox1.Text;
            string hedefKlasor = textBox2.Text;

            int hedefGenislik = 1920;
            int hedefYukseklik = 1500;

            // Kaynak klasördeki fotoğrafları seç
            if (string.IsNullOrWhiteSpace(kaynakKlasor) || !Directory.Exists(kaynakKlasor))
            {
                MessageBox.Show("Geçersiz veya boş kaynak klasörü");
                return; // Hata durumunda işlemi sonlandır
            }

            string[] fotoDosyalar = Directory.GetFiles(kaynakKlasor);

            if (fotoDosyalar.Length == 0)
            {
                MessageBox.Show("Kaynak klasörde fotoğraf bulunamadı.");
                return; // Hata durumunda işlemi sonlandır
            }

            if (string.IsNullOrEmpty(hedefKlasor) || !Directory.Exists(hedefKlasor))
            {
                MessageBox.Show("Hedef klasör seçilmedi");
                return; // Hata durumunda işlemi sonlandır
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, ea) =>
            {
                for (int i = 0; i < fotoDosyalar.Length; i++)
                {
                    string fotoDosya = fotoDosyalar[i];

                    using (Image image = Image.FromFile(fotoDosya))
                    using (Bitmap yeniGoruntu = new Bitmap(hedefGenislik, hedefYukseklik))
                    using (Graphics grafik = Graphics.FromImage(yeniGoruntu))
                    {
                        grafik.CompositingQuality = CompositingQuality.HighQuality;
                        grafik.SmoothingMode = SmoothingMode.HighQuality;
                        grafik.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        grafik.DrawImage(image, 0, 0, hedefGenislik, hedefYukseklik);
                        string qrData = "QR Kod Verisi Buraya Gelecek"; // QR kodunuzun içeriğini buraya ekle
                        Bitmap qrCodeBitmap = GenerateQRCode(qrData, 100, 100);

                        // QR kodunu fotoğrafın üzerine ekler
                        grafik.DrawImage(qrCodeBitmap, 0, 0, 100, 100);

                        string yeniDosyaAdi = Path.GetFileNameWithoutExtension(fotoDosya) + "_kucuk.jpeg";
                        yeniGoruntu.Save(Path.Combine(hedefKlasor, yeniDosyaAdi), ImageFormat.Jpeg);
                    }

                    int progressPercentage = (i + 1) * 100 / fotoDosyalar.Length;
                    worker.ReportProgress(progressPercentage);
                }
            };

            worker.ProgressChanged += (s, ea) =>
            {
                progressBar1.Value = ea.ProgressPercentage;
                labelProgress.Text = $"İlerleme: {ea.ProgressPercentage}%"; // Yüzdeyi gösteren bir etiket 
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                // İşlem tamamlandığında ProgressBarı gizle
                progressBar1.Visible = false;
                MessageBox.Show("FOTOĞRAFLAR BAŞARIYLA SIKIŞTIRILDI");
            };

            progressBar1.Visible = true;
            labelProgress.Text = "İlerleme: 0%"; // Başlangıçta yüzdeyi sıfır olarak ayarla
            worker.RunWorkerAsync();
        }

            
        public void textBox1_TextChanged(object sender, EventArgs e)
        {

        } 
    
    }
    
}

