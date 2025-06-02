namespace LLMClippy
{
    partial class ClippyForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Clipboard_listBox = new ListBox();
            clipboardToolTip = new ToolTip(components);
            clipboardContextMenu = new ContextMenuStrip(components);
            promptTextBox = new RichTextBox();
            responseTextBox = new RichTextBox();
            generateButton = new Button();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            newTabCheckbox = new CheckBox();
            critiqueButton = new Button();
            loadingIndicator = new ProgressBar();
            summarizeButton = new Button();
            respondButton = new Button();
            explainButton = new Button();
            clearButton = new Button();
            tabControl1 = new TabControl();
            textTab = new TabPage();
            webTab = new TabPage();
            responseView = new Microsoft.Web.WebView2.WinForms.WebView2();
            promptBox = new ComboBox();
            clearListButton = new Button();
            enabledCheckBox = new CheckBox();
            modelsComboBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabControl1.SuspendLayout();
            textTab.SuspendLayout();
            webTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)responseView).BeginInit();
            SuspendLayout();
            // 
            // Clipboard_listBox
            // 
            Clipboard_listBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Clipboard_listBox.BackColor = SystemColors.WindowText;
            Clipboard_listBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Clipboard_listBox.ForeColor = SystemColors.Window;
            Clipboard_listBox.FormattingEnabled = true;
            Clipboard_listBox.ItemHeight = 28;
            Clipboard_listBox.Location = new Point(3, 1);
            Clipboard_listBox.Name = "Clipboard_listBox";
            Clipboard_listBox.SelectionMode = SelectionMode.MultiExtended;
            Clipboard_listBox.Size = new Size(193, 536);
            Clipboard_listBox.TabIndex = 0;
            // 
            // clipboardContextMenu
            // 
            clipboardContextMenu.ImageScalingSize = new Size(24, 24);
            clipboardContextMenu.Name = "clipboardContextMenu";
            clipboardContextMenu.Size = new Size(61, 4);
            // 
            // promptTextBox
            // 
            promptTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            promptTextBox.BackColor = SystemColors.WindowText;
            promptTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            promptTextBox.ForeColor = SystemColors.Window;
            promptTextBox.Location = new Point(0, 0);
            promptTextBox.Name = "promptTextBox";
            promptTextBox.Size = new Size(979, 204);
            promptTextBox.TabIndex = 1;
            promptTextBox.Text = "";
            // 
            // responseTextBox
            // 
            responseTextBox.BackColor = SystemColors.WindowText;
            responseTextBox.Dock = DockStyle.Fill;
            responseTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            responseTextBox.ForeColor = SystemColors.Window;
            responseTextBox.Location = new Point(3, 3);
            responseTextBox.Name = "responseTextBox";
            responseTextBox.Size = new Size(958, 271);
            responseTextBox.TabIndex = 2;
            responseTextBox.Text = "";
            // 
            // generateButton
            // 
            generateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            generateButton.Location = new Point(564, 210);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(52, 34);
            generateButton.TabIndex = 3;
            generateButton.Text = "&Go";
            generateButton.UseVisualStyleBackColor = true;
            generateButton.Click += generateButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(3, 45);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Clipboard_listBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1186, 576);
            splitContainer1.SplitterDistance = 199;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            splitContainer2.BackColor = SystemColors.ControlDark;
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(generateButton);
            splitContainer2.Panel1.Controls.Add(newTabCheckbox);
            splitContainer2.Panel1.Controls.Add(critiqueButton);
            splitContainer2.Panel1.Controls.Add(loadingIndicator);
            splitContainer2.Panel1.Controls.Add(summarizeButton);
            splitContainer2.Panel1.Controls.Add(respondButton);
            splitContainer2.Panel1.Controls.Add(explainButton);
            splitContainer2.Panel1.Controls.Add(clearButton);
            splitContainer2.Panel1.Controls.Add(promptTextBox);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tabControl1);
            splitContainer2.Size = new Size(983, 576);
            splitContainer2.SplitterDistance = 251;
            splitContainer2.TabIndex = 0;
            // 
            // newTabCheckbox
            // 
            newTabCheckbox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            newTabCheckbox.AutoSize = true;
            newTabCheckbox.Location = new Point(625, 212);
            newTabCheckbox.Margin = new Padding(2, 2, 2, 2);
            newTabCheckbox.Name = "newTabCheckbox";
            newTabCheckbox.Size = new Size(105, 29);
            newTabCheckbox.TabIndex = 10;
            newTabCheckbox.Text = "New Tab";
            newTabCheckbox.UseVisualStyleBackColor = true;
            // 
            // critiqueButton
            // 
            critiqueButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            critiqueButton.Location = new Point(380, 210);
            critiqueButton.Name = "critiqueButton";
            critiqueButton.Size = new Size(95, 34);
            critiqueButton.TabIndex = 9;
            critiqueButton.Text = "&Critique";
            critiqueButton.UseVisualStyleBackColor = true;
            critiqueButton.Click += critiqueButton_Click;
            // 
            // loadingIndicator
            // 
            loadingIndicator.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            loadingIndicator.Location = new Point(478, 210);
            loadingIndicator.Name = "loadingIndicator";
            loadingIndicator.Size = new Size(82, 34);
            loadingIndicator.Style = ProgressBarStyle.Marquee;
            loadingIndicator.TabIndex = 4;
            loadingIndicator.Visible = false;
            // 
            // summarizeButton
            // 
            summarizeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            summarizeButton.Location = new Point(270, 210);
            summarizeButton.Name = "summarizeButton";
            summarizeButton.Size = new Size(108, 34);
            summarizeButton.TabIndex = 8;
            summarizeButton.Text = "&Summarize";
            summarizeButton.UseVisualStyleBackColor = true;
            summarizeButton.Click += summarizeButton_Click;
            // 
            // respondButton
            // 
            respondButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            respondButton.Location = new Point(173, 210);
            respondButton.Name = "respondButton";
            respondButton.Size = new Size(96, 34);
            respondButton.TabIndex = 7;
            respondButton.Text = "&Respond";
            respondButton.UseVisualStyleBackColor = true;
            respondButton.Click += respondButton_Click;
            // 
            // explainButton
            // 
            explainButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            explainButton.Location = new Point(85, 210);
            explainButton.Name = "explainButton";
            explainButton.Size = new Size(88, 34);
            explainButton.TabIndex = 6;
            explainButton.Text = "&Explain";
            explainButton.UseVisualStyleBackColor = true;
            explainButton.Click += explainButton_Click;
            // 
            // clearButton
            // 
            clearButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            clearButton.Location = new Point(3, 210);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(81, 34);
            clearButton.TabIndex = 5;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += clearButton_Click;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(textTab);
            tabControl1.Controls.Add(webTab);
            tabControl1.Location = new Point(3, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(972, 315);
            tabControl1.TabIndex = 4;
            // 
            // textTab
            // 
            textTab.Controls.Add(responseTextBox);
            textTab.Location = new Point(4, 34);
            textTab.Name = "textTab";
            textTab.Padding = new Padding(3, 3, 3, 3);
            textTab.Size = new Size(964, 277);
            textTab.TabIndex = 0;
            textTab.Text = "Text";
            textTab.UseVisualStyleBackColor = true;
            // 
            // webTab
            // 
            webTab.Controls.Add(responseView);
            webTab.Location = new Point(4, 34);
            webTab.Name = "webTab";
            webTab.Padding = new Padding(3, 3, 3, 3);
            webTab.Size = new Size(964, 277);
            webTab.TabIndex = 1;
            webTab.Text = "Web";
            webTab.UseVisualStyleBackColor = true;
            // 
            // responseView
            // 
            responseView.AllowExternalDrop = true;
            responseView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            responseView.CreationProperties = null;
            responseView.DefaultBackgroundColor = Color.White;
            responseView.Location = new Point(6, 6);
            responseView.Name = "responseView";
            responseView.Size = new Size(952, 261);
            responseView.TabIndex = 3;
            responseView.ZoomFactor = 1D;
            // 
            // promptBox
            // 
            promptBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            promptBox.FormattingEnabled = true;
            promptBox.Location = new Point(655, 6);
            promptBox.Name = "promptBox";
            promptBox.Size = new Size(531, 33);
            promptBox.TabIndex = 5;
            // 
            // clearListButton
            // 
            clearListButton.Location = new Point(7, 6);
            clearListButton.Name = "clearListButton";
            clearListButton.Size = new Size(91, 34);
            clearListButton.TabIndex = 6;
            clearListButton.Text = "Clear All";
            clearListButton.UseVisualStyleBackColor = true;
            clearListButton.Click += clearListButton_Click;
            // 
            // enabledCheckBox
            // 
            enabledCheckBox.AutoSize = true;
            enabledCheckBox.Checked = true;
            enabledCheckBox.CheckState = CheckState.Checked;
            enabledCheckBox.Location = new Point(106, 9);
            enabledCheckBox.Name = "enabledCheckBox";
            enabledCheckBox.Size = new Size(159, 29);
            enabledCheckBox.TabIndex = 7;
            enabledCheckBox.Text = "Add to Prompt";
            enabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // modelsComboBox
            // 
            modelsComboBox.FormattingEnabled = true;
            modelsComboBox.Location = new Point(501, 5);
            modelsComboBox.Name = "modelsComboBox";
            modelsComboBox.Size = new Size(148, 33);
            modelsComboBox.TabIndex = 8;
            // 
            // ClippyForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1192, 625);
            Controls.Add(modelsComboBox);
            Controls.Add(enabledCheckBox);
            Controls.Add(clearListButton);
            Controls.Add(promptBox);
            Controls.Add(splitContainer1);
            Name = "ClippyForm";
            Text = "LLM Clippy";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            textTab.ResumeLayout(false);
            webTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)responseView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox Clipboard_listBox;
        private ToolTip clipboardToolTip;
        private ContextMenuStrip clipboardContextMenu;
        private RichTextBox promptTextBox;
        private RichTextBox responseTextBox;
        private Button generateButton;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ProgressBar loadingIndicator;
        private Button respondButton;
        private Button explainButton;
        private Button clearButton;
        private Button summarizeButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 responseView;
        private TabControl tabControl1;
        private TabPage textTab;
        private TabPage webTab;
        private ComboBox promptBox;
        private Button clearListButton;
        private CheckBox enabledCheckBox;
        private ComboBox modelsComboBox;
        private Button critiqueButton;
        private CheckBox newTabCheckbox;
    }
}
