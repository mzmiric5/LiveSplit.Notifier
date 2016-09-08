using System;
using System.Net.WebSockets;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class NotifierSettings : UserControl
    {
        public bool Split { get; set; }
        public bool SplitAheadGaining { get; set; }
        public bool SplitAheadLosing { get; set; }
        public bool SplitBehindGaining { get; set; }
        public bool SplitBehindLosing { get; set; }
        public bool BestSegment { get; set; }
        public bool UndoSplit { get; set; }
        public bool SkipSplit { get; set; }
        public bool PersonalBest { get; set; }
        public bool NotAPersonalBest { get; set; }
        public bool Reset { get; set; }
        public bool Pause { get; set; }
        public bool Resume { get; set; }
        public bool StartTimer { get; set; }
        public string Url { get; set; }
        public bool isWS { get; set; }
        public bool isWH { get; set; }

        private NotifierComponent notifier;

        public NotifierSettings(object caller)
        {
            notifier = (NotifierComponent)caller;

            InitializeComponent();

            Split =
            SplitAheadGaining =
            SplitAheadLosing =
            SplitBehindGaining =
            SplitBehindLosing =
            BestSegment =
            UndoSplit =
            SkipSplit =
            PersonalBest =
            NotAPersonalBest =
            Reset =
            Pause =
            Resume =
            StartTimer = false;

            Url = "";
            isWS = false;
            isWH = true;

            txtSplitPath.DataBindings.Add("Checked", this, "Split");
            txtSplitAheadGaining.DataBindings.Add("Checked", this, "SplitAheadGaining");
            txtSplitAheadLosing.DataBindings.Add("Checked", this, "SplitAheadLosing");
            txtSplitBehindGaining.DataBindings.Add("Checked", this, "SplitBehindGaining");
            txtSplitBehindLosing.DataBindings.Add("Checked", this, "SplitBehindLosing");
            txtBestSegment.DataBindings.Add("Checked", this, "BestSegment");
            txtUndo.DataBindings.Add("Checked", this, "UndoSplit");
            txtSkip.DataBindings.Add("Checked", this, "SkipSplit");
            txtPersonalBest.DataBindings.Add("Checked", this, "PersonalBest");
            txtNotAPersonalBest.DataBindings.Add("Checked", this, "NotAPersonalBest");
            txtReset.DataBindings.Add("Checked", this, "Reset");
            txtPause.DataBindings.Add("Checked", this, "Pause");
            txtResume.DataBindings.Add("Checked", this, "Resume");
            txtStartTimer.DataBindings.Add("Checked", this, "StartTimer");
            txtURL.DataBindings.Add("Text", this, "Url");
            radioButton1.DataBindings.Add("Checked", this, "isWH");
            radioButton2.DataBindings.Add("Checked", this, "isWS");
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;

            Split = SettingsHelper.ParseBool(element["Split"]);
            SplitAheadGaining = SettingsHelper.ParseBool(element["SplitAheadGaining"]);
            SplitAheadLosing = SettingsHelper.ParseBool(element["SplitAheadLosing"]);
            SplitBehindGaining = SettingsHelper.ParseBool(element["SplitBehindGaining"]);
            SplitBehindLosing = SettingsHelper.ParseBool(element["SplitBehindLosing"]);
            BestSegment = SettingsHelper.ParseBool(element["BestSegment"]);
            UndoSplit = SettingsHelper.ParseBool(element["UndoSplit"]);
            SkipSplit = SettingsHelper.ParseBool(element["SkipSplit"]);
            PersonalBest = SettingsHelper.ParseBool(element["PersonalBest"]);
            NotAPersonalBest = SettingsHelper.ParseBool(element["NotAPersonalBest"]);
            Reset = SettingsHelper.ParseBool(element["Reset"]);
            Pause = SettingsHelper.ParseBool(element["Pause"]);
            Resume = SettingsHelper.ParseBool(element["Resume"]);
            StartTimer = SettingsHelper.ParseBool(element["StartTimer"]);
            Url = SettingsHelper.ParseString(element["Url"]);
            isWH = SettingsHelper.ParseBool(element["isWH"],true);
            isWS = SettingsHelper.ParseBool(element["isWS"]);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent) {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0.0") ^
            SettingsHelper.CreateSetting(document, parent, "Split", Split) ^
            SettingsHelper.CreateSetting(document, parent, "SplitAheadGaining", SplitAheadGaining) ^
            SettingsHelper.CreateSetting(document, parent, "SplitAheadLosing", SplitAheadLosing) ^
            SettingsHelper.CreateSetting(document, parent, "SplitBehindGaining", SplitBehindGaining) ^
            SettingsHelper.CreateSetting(document, parent, "SplitBehindLosing", SplitBehindLosing) ^
            SettingsHelper.CreateSetting(document, parent, "BestSegment", BestSegment) ^
            SettingsHelper.CreateSetting(document, parent, "UndoSplit", UndoSplit) ^
            SettingsHelper.CreateSetting(document, parent, "SkipSplit", SkipSplit) ^
            SettingsHelper.CreateSetting(document, parent, "PersonalBest", PersonalBest) ^
            SettingsHelper.CreateSetting(document, parent, "NotAPersonalBest", NotAPersonalBest) ^
            SettingsHelper.CreateSetting(document, parent, "Reset", Reset) ^
            SettingsHelper.CreateSetting(document, parent, "Pause", Pause) ^
            SettingsHelper.CreateSetting(document, parent, "Resume", Resume) ^
            SettingsHelper.CreateSetting(document, parent, "StartTimer", StartTimer) ^
            SettingsHelper.CreateSetting(document, parent, "isWS", isWS) ^
            SettingsHelper.CreateSetting(document, parent, "isWH", isWH) ^
            SettingsHelper.CreateSetting(document, parent, "Url", Url);
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Split");
        }

        private void btnAheadGaining_Click(object sender, EventArgs e)
        {
            notifier.sendTest("SplitAheadGaining");
        }

        private void btnAheadLosing_Click(object sender, EventArgs e)
        {
            notifier.sendTest("SplitAheadLosing");
        }

        private void btnBehindGaining_Click(object sender, EventArgs e)
        {
            notifier.sendTest("SplitBehindGaining");
        }

        private void btnBehindLosing_Click(object sender, EventArgs e)
        {
            notifier.sendTest("SplitBehindLosing");
        }

        private void btnBestSegment_Click(object sender, EventArgs e)
        {
            notifier.sendTest("BestSegment");
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Undo");
        }

        private void btnSkipSplit_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Skip");
        }

        private void btnPersonalBest_Click(object sender, EventArgs e)
        {
            notifier.sendTest("PersonalBest");
        }

        private void btnNotAPersonalBest_Click(object sender, EventArgs e)
        {
            notifier.sendTest("NotAPersonalBest");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Reset");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Pause");
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Resume");
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            notifier.sendTest("Start");
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            var radio = (RadioButton)sender;
            isWH = radio.Checked;
            if (radio.Checked && notifier.WebSocket.State == WebSocketState.Open) {
                notifier.DisconnectFromWS();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            var radio = (RadioButton)sender;
            isWS = radio.Checked;
            if(radio.Checked && notifier.WebSocket.State != WebSocketState.Open) {
                notifier.ConnectToWS(null);
            }
        }

        private void txtSplitPath_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            Split = checkbox.Checked;
        }

        private void txtSplitAheadGaining_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            SplitAheadGaining = checkbox.Checked;
        }

        private void txtSplitAheadLosing_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            SplitAheadLosing = checkbox.Checked;
        }

        private void txtSplitBehindGaining_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            SplitBehindGaining = checkbox.Checked;
        }

        private void txtSplitBehindLosing_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            SplitBehindLosing = checkbox.Checked;
        }

        private void txtBestSegment_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            BestSegment = checkbox.Checked;
        }

        private void txtUndo_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            UndoSplit = checkbox.Checked;
        }

        private void txtSkip_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            SkipSplit = checkbox.Checked;
        }

        private void txtPersonalBest_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            PersonalBest = checkbox.Checked;
        }

        private void txtNotAPersonalBest_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            NotAPersonalBest = checkbox.Checked;
        }

        private void txtReset_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            Reset = checkbox.Checked;
        }

        private void txtPause_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            Pause = checkbox.Checked;
        }

        private void txtResume_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            Resume = checkbox.Checked;
        }

        private void txtStartTimer_CheckedChanged(object sender, EventArgs e) {
            var checkbox = (CheckBox)sender;
            StartTimer = checkbox.Checked;
        }
    }
}
