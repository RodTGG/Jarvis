using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Speech.Recognition;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace TextToDiary
{
    public partial class Main : Form
    {

        SpeechRecognitionEngine mySRE = new SpeechRecognitionEngine();
        string[] lSites;
        string[] lCommands = { "exit", "new tab", "stop", "jarvis" };
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        bool bRunning = true;

        public Main()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bRunning = false;
            Disable();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            bRunning = true;
            Enable();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WindowSetup();
            LoadFile();

            Choices Commands = new Choices(lCommands);
            Grammar gm = new Grammar(Commands);

            mySRE.RequestRecognizerUpdate();
            mySRE.SetInputToDefaultAudioDevice();
            mySRE.LoadGrammar(gm);
            mySRE.SpeechRecognized += mySR_SpeechRecognized;
            mySRE.RecognizeAsync(RecognizeMode.Multiple);

        }

        void mySR_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "jarvis")
            {
                bRunning = true;
                Enable();
            }

            if (bRunning)
            {
                try
                {
                    switch (e.Result.Text)
                    {
                        case "exit":
                            Application.Exit();
                            break;
                        case "new tab":
                            Process.Start("chrome", "https://www.google.com.au/");
                            break;
                        case "stop":
                            bRunning = false;
                            Disable();
                            break;
                    }

                    if (e.Result.Text == "plex")
                    {
                        Process.Start("chrome", "http://192.168.1.201:32400/web/index.html");

                    }
                    else if (lSites.Contains(e.Result.Text))
                    {
                        Process.Start("chrome", "http://www." + e.Result.Text + ".com");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString());
                }
            }
        }

        void WindowSetup()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Activate();
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            btnStart.Enabled = !btnStart.Enabled;
            btnStop.Enabled = !btnStart.Enabled;
        }

        private void menuItem2_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        private void menuItem3_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            this.Close();
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void ntiTxtDiary_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.BringToFront();
        }

        private void Enable()
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void Disable()
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void LoadFile()
        {
            DialogResult msgBoxResult;
            string path = "websites.txt";

            if (!File.Exists(path))
            {
                msgBoxResult = MessageBox.Show("Error websites.txt not found\n Create default websites?", "Error in IO", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (msgBoxResult == DialogResult.Cancel)
                {
                    MessageBox.Show("Closing", "Closing");
                    Application.Exit();
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        string lSites = "facebook\ntwitch\nyoutube\nplex\nnetflix\nreddit";
                        sw.Write(lSites);
                    }
                }
            }

            using (StreamReader sr = File.OpenText(path))
            {
                string sLine = "";
                List<string> temp = new List<string>();

                for (int i = 0; (sLine = sr.ReadLine()) != null; i++)
                {
                    temp.Add(sLine);
                }

                lSites = new string[temp.Count];

                for (int i = 0; i < temp.Count; i++)
                {
                    lSites[i] = temp[i];
                }
            }

            SetupCommand();
        }

        void SetupCommand()
        {
            List<string> tempCommand = new List<string>();


            for (int i = 0; i < lCommands.Length; i++)
            {
                tempCommand.Add(lCommands[i]);
            }

            for (int i = 0; i < lSites.Length; i++)
            {
                tempCommand.Add(lSites[i]);
            }

            lCommands = new string[lCommands.Length + lSites.Length];
            for (int i = 0; i < tempCommand.Count; i++)
            {
                lCommands[i] = tempCommand[i];
            }
        }
    }
}
