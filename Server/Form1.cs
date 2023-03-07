using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Server.Doi_tuong;
using Server.Thu_Vien;
using System.Security.Cryptography;

namespace Server
{
    public partial class Form1 : Form
	{
		IPEndPoint IP;
		Socket server;
		Socket client;
		string Messagecurrent="start";
		int MaHoaHayGiaiMa = 1;
		bool FileHayChuoi = false;
		DES64Bit MaHoaDES64;
		Khoa Khoa;
		Thread thread;
		public static string TenTienTrinh = "";
		public static int GiaiDoan = -1;
		private static int Dem = 0;
		public Form1()
        {
            InitializeComponent();
			Connect();
			progressBar1.Visible= false;
		}
	
        private void Form1_Load(object sender, EventArgs e)
        {

		}
		void Connect()
		{
			IP = new IPEndPoint(IPAddress.Any, 9999);
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			server.Bind(IP);

			Thread listen = new Thread(() =>
			{
				try
				{
					while (true)
					{
						server.Listen(100);
						client = server.Accept();
						Thread receive = new Thread(Receive);
						receive.IsBackground = true;
						receive.Start(client);
					}
				}
				catch
				{
					IP = new IPEndPoint(IPAddress.Any, 9999);
					server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
				}
			}
				);
			listen.IsBackground = true;
			listen.Start();

		}
		void Receive(object obj)
		{
			Socket client = obj as Socket;
			try
			{
				int size;
				while (true)
				{
					byte[] data = new byte[1024];
					size = client.Receive(data);
					string[] s = Encoding.UTF8.GetString(data, 0, size).Split(new char[] { ',' }); // nhan ten file, duong dan, size.
					long length = long.Parse(s[1]);
					byte[] buffer = new byte[1024];
					byte[] fsize = new byte[length]; //khai bao mang byte de chua du lieu
					long n = length / buffer.Length;  // tính số frame sẽ được gửi qua
					for (int i = 0; i <n; i++)
					{
						size = client.Receive(fsize, fsize.Length, SocketFlags.None);
						Console.WriteLine("Received frame {0}/{1}", i, n);
					}
					FileStream fs = new FileStream("C:/Users/hungdz/Desktop/server" + "/" + s[0], FileMode.Create);  // luu file s[0] vao duong dan s[1]
					fs.Write(fsize, 0, fsize.Length);
					fs.Close();
					Console.WriteLine("Done."); break;
				}
			}
			catch(Exception e)
			{
				Messagecurrent = e.Message;
			}

		}



		private void timer1_Tick(object sender, EventArgs e)
        {
			
        }
   

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }
		private void Chay()
		{
			ThreadStart start = new ThreadStart(MaHoa);
			thread = new Thread(start);
			thread.Start();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			/*	FileDialog fd = new OpenFileDialog();
				if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{

					string path = "";
					string fileName = fd.FileName.Replace("\\", "/");
					while (fileName.IndexOf("/") > -1)
					{
						path += fileName.Substring(0, fileName.IndexOf("/") + 1);
						fileName = fileName.Substring(fileName.IndexOf("/") + 1);
					}
					FileInfo file = new FileInfo(path + fileName);
					byte[] data = new byte[1024];
					byte[] fsize = new byte[file.Length]; // tạo mảng chứa dữ liệu
					FileStream fs = new FileStream(path + fileName, FileMode.Open); // đọc thông tin file đã nhập
					fs.Read(fsize, 0, fsize.Length);

					fs.Close();
					while (true)
					{
						client.Send(Encoding.UTF8.GetBytes(fileName + "," + file.Length.ToString()));
						long n = file.Length / data.Length;  //tính số frame phải gửi
						progressBar1.Minimum = 0;
						progressBar1.Maximum = (int)n;
						progressBar1.Visible = true;
						for (int i = 0; i < n; i++)
						{
							progressBar1.Value = i;
							Console.WriteLine("Sending frame {0}/{1}", i, n);
							client.Send(fsize, fsize.Length, 0);
						}
						Form2 form = new Form2();
						progressBar1.Visible = false;
						form.ShowDialog();
						Console.WriteLine("Done."); break;
					}

				};*/
			FileHayChuoi = true;
			MaHoaHayGiaiMa = 1;
			Chay();
		}
		private void MaHoa()
		{
			//try
			//{

			MaHoaDES64 = new DES64Bit();

			TenTienTrinh = "";

			GiaiDoan = 0;
			Dem = 0;

			if (FileHayChuoi)
			{
				Khoa = new Khoa("12345678901234");
				if (MaHoaHayGiaiMa == 1)
				{

					GiaiDoan = 0;
					ChuoiNhiPhan chuoi = DocFileTxt.FileReadToBinary("C:/Users/hungdz/Desktop/lab1.txt");
					GiaiDoan = 1;
					ChuoiNhiPhan KQ = MaHoaDES64.ThucHienDES(Khoa, chuoi, 1);
					GiaiDoan = 2;
					DocFileTxt.WriteBinaryToFile("C:/Users/hungdz/Desktop/lab2.txt", KQ);
					GiaiDoan = 3;
					Console.WriteLine("OK");
					MessageBox.Show("Mã hóa file thành công");
				}
				else
				{
					GiaiDoan = 0;
					ChuoiNhiPhan chuoi = DocFileTxt.FileReadToBinary("C:/Users/hungdz/Desktop/298456285_586964226438656_6165240154062365101_n.jpg");
					GiaiDoan = 1;
					ChuoiNhiPhan KQ = MaHoaDES64.ThucHienDES(Khoa, chuoi, -1);
					if (KQ == null)
					{
						MessageBox.Show("Lỗi giải mã . kiểm tra khóa ");
						return;
					}
					GiaiDoan = 2;
					DocFileTxt.WriteBinaryToFile("C:/Users/hungdz/Desktop/298456285_586964226438656_6165240154062365101_n1.jpg", KQ);
					GiaiDoan = 3;
					MessageBox.Show("Giải mã file thành công");
				}
			}
		
			//}
			//catch (Exception ex)
			//{

			//    MessageBox.Show(ex.ToString()); 
			//}

		}

		private void progressBar1_Click(object sender, EventArgs e)
		{

		}
	}
}
