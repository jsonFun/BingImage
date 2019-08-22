using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace bing壁纸抓取
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        string local;
        private void button1_Click(object sender, EventArgs e)
        {
            int days =int.Parse(!string.IsNullOrEmpty(comboBox1.Text) ? comboBox1.Text : "0");
            if(days < 0)
            {
                days = 0;
            }
            local = "";
            string bing = "http://cn.bing.com";
            string url = "http://cn.bing.com/HPImageArchive.aspx?idx="+ days + "&n=1";
            //获取网页源码
            string txtHtml = GetWebClient(url);
            var ds = ConvertXMLToDataSet(txtHtml);
            var urlimage = ds.Tables[0].Rows[0]["url"].ToString();
            string imgurl = bing + urlimage;
            DownImage(imgurl);
            pictureBox1.BackgroundImage = Image.FromFile(local);//加入这句；
            ClearMemory();
        }

        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    //FrmAll为我窗体的类名
                    Form1.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("释放内存，异常："+ex.ToString());

            }

        }
        #endregion

        public void DownImage(string imgurl)
        {
            WebClient webClient = new WebClient();
            Byte[] imgData = webClient.DownloadData(imgurl);
            Stream ms = new MemoryStream(imgData);
            ms.Position = 0;
            Image img = Image.FromStream(ms);
            string name = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string path = AppDomain.CurrentDomain.BaseDirectory + "\\bing图片\\";
            //local = path + name + ".jpg";
            string subPath = "D:\\bing图片\\";
            if (false == System.IO.Directory.Exists(subPath))
            {
                //创建文件夹
                System.IO.Directory.CreateDirectory(subPath);
            }
            local = "D:\\bing图片\\"+ name + ".jpg";
            img.Save(local, ImageFormat.Jpeg);
        }

        public  DataSet ConvertXMLToDataSet(string xmlData)
        {

            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);

                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private string GetWebClient(string url)
        {
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
            strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 7;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(local))
            {
                int temp = SystemParametersInfo(20, 0, local, 0);
               // File.Delete(local);
            }
        }

        #region System Innerface
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
         );
        #endregion
    }
}
