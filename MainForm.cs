using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace XqLineNotifier
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string lineToken = "";
        private static string AppJsonFilePath = "./lineToken.json";
        private static string XqMonitorPath = "./";
        private static string XqLogFileExtension = "*.xqlog";

        public void CreateFileWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Add event handlers.
            // Only watch *.xqlog files
            watcher.Filter = XqLogFileExtension;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreatedAsync);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private async void OnCreatedAsync(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            
            try
            {
                if (this.lineToken.Trim() != "")
                {
                    string message = File.ReadAllText(e.FullPath, Encoding.GetEncoding("Big5"));

                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.lineToken);
                    var form = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("message", message)
                });
                    HttpResponseMessage result = await client.PostAsync(new Uri("https://notify-api.line.me/api/notify"), form);
                }
                File.Delete(e.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        public class AppSettings
        {
            public string LineToken { get; set; }
        }

        private AppSettings readAppSettings()
        {
            var jsonString = System.IO.File.ReadAllText(AppJsonFilePath);
            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(jsonString);
            return appSettings;
        }

        private void checkXqLineNotifyPathExists()
        {
            if (!Directory.Exists(XqMonitorPath))
            {
                Directory.CreateDirectory(XqMonitorPath);
            }
        }
        private void createAppSettingsFile()
        {
            var appJsonObj = new AppSettings() { LineToken = "" };
            System.IO.File.WriteAllText(AppJsonFilePath, JsonConvert.SerializeObject(appJsonObj));
        }

        private void initAppSettings()
        {

            try
            {
                if (!System.IO.File.Exists(AppJsonFilePath))
                {
                    this.createAppSettingsFile();
                }
                AppSettings appSettings = this.readAppSettings();
                lineToken = appSettings.LineToken;
                txtLineToken.Text = lineToken;
            }
            catch (Exception e)
            {
                this.createAppSettingsFile();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            checkXqLineNotifyPathExists();
            initAppSettings();
            createXqMonitorPath();
            deleteAllXqLog();
            CreateFileWatcher(XqMonitorPath);
        }

        private void createXqMonitorPath() {
            if (!System.IO.Directory.Exists(XqMonitorPath)) {
                System.IO.Directory.CreateDirectory(XqMonitorPath);
            }
        }

        private void deleteAllXqLog() {
            foreach (string sFile in System.IO.Directory.GetFiles(XqMonitorPath, XqLogFileExtension))
            {
                System.IO.File.Delete(sFile);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            txtLineToken.Text = txtLineToken.Text.Trim();
            AppSettings appSettings = this.readAppSettings();
            appSettings.LineToken = txtLineToken.Text;
            string jsonString = JsonConvert.SerializeObject(appSettings);
            System.IO.File.WriteAllText(AppJsonFilePath, jsonString);
            this.lineToken = txtLineToken.Text;
        }
    }
}
