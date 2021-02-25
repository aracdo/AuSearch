using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NAudio.Wave;
using NAudio.CoreAudioApi;

namespace BW.Diplom
{
    public partial class MainForm : Form
    {
        public Color myColor = Color.White;
        public bool overlay = false;
        private bool check = false;
        private FormOverlay frm = null;
        internal static Settings settings = new Settings();
        private MMDevice mmDevice;
        public MainForm()
        {
            InitializeComponent();
            colorDialog1.FullOpen = true;
            colorDialog1.Color = this.BackColor;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            FillSettings();
            SaveSettings();
        }
        private void FillAuduioDevicesList()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            mmDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            //AudioMeterInformationChannels aMIC = mmDevice.AudioMeterInformation.PeakValues;
            //float a = aMIC[0];
            //mmDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
            //progressBar1.Value = (int)(Math.Round(mmDevice.AudioMeterInformation.MasterPeakValue * 100));
            ////var deviceEnum = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            audioDevsList.Items.AddRange(devices.ToArray());
            //audioDevsList.DisplayMember = "FriendlyName";      
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FillAuduioDevicesList();
            LoadSettings();
           
        }
        private void FillSettings()
        {
            if (frm != null)
            {
                settings.Location = frm.Location;
                settings.Size = frm.Size;
                SaveSettings();
            }
        }
        private string GetPathToSettings()
        {
            return Path.Combine(Application.StartupPath, "settings.config");
        }
        private void SaveSettings()
        {
            string fp = GetPathToSettings();
            XmlSerializer formatter = new XmlSerializer(typeof(Settings));
            using (FileStream fs = new FileStream(fp, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, settings);
            }
        }
        private bool LoadSettings()
        {
            string fp = GetPathToSettings();
            if (File.Exists(fp))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(fp, FileMode.Open))
                {
                    settings = (Settings)formatter.Deserialize(fs);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void SetForm(bool stop, bool change)
        {
            if (check)
            {
                if (change)
                {
                    FillSettings();
                    frm.Close();
                    frm.Dispose();
                    frm = null;
                }
                runBtn.Text = "Stop";
                if (frm == null)
                    frm = new FormOverlay(this);
                frm.Show();
            }
            else
            {
                runBtn.Text = "Run";
                if (frm != null)
                {
                    FillSettings();
                    frm.Close();
                    frm.Dispose();
                    frm = null;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            check = !check;
            SetForm(check, false);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            overlay = !overlay;
            SetForm(check, true);
        }

        //private void timer1_tick(object sender, eventargs e)
        //{
        //    //if (audiodevslist.selecteditem != null)
        //    //{
        //        //waveformat = new waveformat(44100, 16, 2);
        //        //waveprovider = new bufferedwaveprovider(waveformat);
        //        ////volumeprovider = new volumewaveprovider16(waveprovider);
        //        //var device = (mmdevice)audiodevslist.selecteditem;
        //        progressbar1.value = (int)(math.round(mmDevice.audiometerinformation.masterpeakvalue * 100));
        //        //device.audioendpointvolume.onvolumenotification += audioendpointvolume_onvolumenotification; ;
        //        //waveprovider.addsamples()
        //        //var stereo = new monotostereoprovider16(waveprovider);


        //    //}
        //}

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            for (int i = 0; i < data.Channels; i++)
            {
                if (data.ChannelVolume[i] > 0)
                {

                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mmDevice != null)
            {
                progressBar1.Value = (int)(Math.Round(mmDevice.AudioMeterInformation.PeakValues[0] * 100));
                progressBar2.Value = (int)(Math.Round(mmDevice.AudioMeterInformation.PeakValues[1] * 100));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            myColor = colorDialog1.Color;
        }


        //private void audioDevsList_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //  if (audioDevsList.SelectedItem != null)
        //  {
        //    var device = (MMDevice)audioDevsList.SelectedItem;
        //    progressBar1.Value = (int)(Math.Round(device.AudioMeterInformation.MasterPeakValue * 100));
        //    device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
        //  }
        //}
    }
}
