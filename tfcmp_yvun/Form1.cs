using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tfcmp_yvun //youtube video upload notification
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            if (Environment.Is64BitOperatingSystem) // 운영체제 종류 확인 (64비트)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", Application.ProductName + ".exe", 11001);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", Application.ProductName + ".vshost.exe", 11001);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", Application.ProductName + ".exe", 11001);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", Application.ProductName + ".vshost.exe", 11001);
            }
            */
            webBrowser1.Navigate("https://www.naver.com");
        }

        void main()
        {
            webBrowser1.Navigate("https://www.youtube.com/channel/UCTSaxXnhUcrhv984bVpDr6Q/videos");

            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            string video_data_raw = Regex.Match(webBrowser1.DocumentText, "(?<=<h3 class=\"yt-lockup-title \">)(.*?)(?=</span></h3>)", RegexOptions.Singleline).Value;
            string link = Regex.Match(video_data_raw, "(?<=href=\"/watch\\?v\\=)(.*?)(?=\")").Value;
            string title = Regex.Match(video_data_raw, "(?<=rel=\"nofollow\">)(.*?)(?=</a>)").Value;

            Video_Data web_video_data = new Video_Data(link, title);
            string file_video_data_link = File.ReadAllText("test.txt");

            if (web_video_data.link != file_video_data_link)
            {
                //notification
                upload_cafe_article(web_video_data);
                File.WriteAllText("test.txt", web_video_data.link);
            }
            else
            {

            }
        }
        void upload_cafe_article(Video_Data video_data)
        {
            webBrowser1.Navigate("https://m.cafe.naver.com/ArticleWrite.nhn?m=write&clubid=29441533&menuid=");

            while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 2000)
            {
                Application.DoEvents();
            }
            sw.Stop();

            //article category select
            webBrowser1.Document.GetElementsByTagName("select")[0].SetAttribute("selectedIndex", "4");
            webBrowser1.Document.GetElementById("subject").SetAttribute("value", "[유튜브 영상] " + video_data.title);
            webBrowser1.Document.Window.Frames["frame"].Document.Body.InnerHtml = 
                "<iframe width=\"560\" height=\"315\" src=\"" +
                "https://www.youtube.com/embed/" + video_data.link +
                "\" frameborder=\"0\" allow=\"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";

            HtmlElementCollection hec = webBrowser1.Document.GetElementsByTagName("a");

            foreach (HtmlElement a in hec)
            {
                if (a.InnerText == "등록")
                {
                    a.InvokeMember("click");
                    break;
                }
            }
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            if(timer1.Enabled == false)
            {
                timer1.Start();
                label1.Text = "RUN";
                label1.BackColor = Color.FromArgb(128, 192, 255);
                main();
            }
            else
            {
                timer1.Stop();
                label1.Text = "STOP";
                label1.BackColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            main();
        }
    }

    public class Video_Data
    {
        public string link;
        public string title;

        public Video_Data(string link, string title)
        {
            this.link = link;
            this.title = title;
        }
    }
}
