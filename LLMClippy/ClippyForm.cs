using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Markdig;

namespace LLMClippy
{
    public partial class ClippyForm : Form
    {
        // Import user32.dll methods  
        [DllImport("user32.dll")]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        public ClippyForm()
        {
            InitializeComponent();
            AddClipboardFormatListener(this.Handle);

            // Initialize the ToolTip
            clipboardToolTip.AutoPopDelay = 5000; // Tooltip display duration
            clipboardToolTip.InitialDelay = 400;  // Delay before showing tooltip
            clipboardToolTip.ReshowDelay = 100;   // Delay between tooltip reappearances

            // Attach MouseMove event to Clipboard_listBox
            Clipboard_listBox.MouseMove += Clipboard_listBox_MouseMove;

            // Attach DoubleClick event to Clipboard_listBox
            Clipboard_listBox.DoubleClick += Clipboard_listBox_DoubleClick;

            // Attach KeyDown event to promptTextBox
            promptTextBox.KeyDown += PromptTextBox_KeyDown;

            // Initialize the ContextMenuStrip
            InitializeContextMenu();

            InitializeWebView2Async();

            PopulateSystemMessageComboBox();

            InitializeAOAI();

            PopulateModelsComboBox();

            // Set the default tab to Web
            tabControl1.SelectedIndex = 1;
        }

        private void PromptTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true; // Prevents the default beep
                ShowPromptTextBoxFindDialog();
            }
            else if (e.KeyCode == Keys.F3)
            {
                e.SuppressKeyPress = true;
                if (!string.IsNullOrEmpty(lastSearchTerm))
                {
                    FindInPromptTextBox(lastSearchTerm);
                }
                else
                {
                    // Optionally, show the find dialog if no previous search term exists
                    ShowPromptTextBoxFindDialog();
                }
            }
        }

        private string? lastSearchTerm = null;

        private void ShowPromptTextBoxFindDialog()
        {
            using (var findForm = new Form())
            {
                findForm.Text = "Find Text";
                findForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                findForm.StartPosition = FormStartPosition.CenterParent;
                findForm.Width = 400;
                findForm.Height = 150;
                findForm.MaximizeBox = false;
                findForm.MinimizeBox = false;
                findForm.KeyPreview = true; // Enable key preview to handle Escape key

                var label = new Label { Text = "Find:", Left = 4, Top = 15, Width = 50 };
                var textBox = new TextBox { Left = 55, Top = 12, Width = 300 };
                var findNextButton = new Button { Text = "Find Next", Left = 200, Top = 45, Width = 160, Height = 32 };

                findForm.Controls.Add(label);
                findForm.Controls.Add(textBox);
                findForm.Controls.Add(findNextButton);
                findForm.AcceptButton = findNextButton;

                // Restore last search term if available
                if (!string.IsNullOrEmpty(lastSearchTerm))
                    textBox.Text = lastSearchTerm;

                // Find Next button logic
                findNextButton.Click += (s, e) =>
                {
                    string searchText = textBox.Text;
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        lastSearchTerm = searchText;
                        FindInPromptTextBox(searchText);
                    }
                };

                // Handle Escape key to close the dialog
                findForm.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        findForm.DialogResult = DialogResult.Cancel;
                        findForm.Close();
                    }
                };

                findForm.ShowDialog(this);
            }
        }

        private void FindInPromptTextBox(string searchText)
        {
            int start = promptTextBox.SelectionStart + promptTextBox.SelectionLength;
            int index = promptTextBox.Find(searchText, start, RichTextBoxFinds.None);
            if (index >= 0)
            {
                promptTextBox.Select(index, searchText.Length);
                promptTextBox.ScrollToCaret();
                promptTextBox.Focus();
            }
            else
            {
                MessageBox.Show("Text not found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void PopulateModelsComboBox()
        {
            // Retrieve the AzureOpenAISettings section
            var azureOpenAISettings = (NameValueCollection)ConfigurationManager.GetSection("AzureOpenAISettings");

            // Create a HashSet to store unique model names
            var modelNames = new HashSet<string>();

            // Iterate through all keys in the AzureOpenAISettings section
            foreach (string key in azureOpenAISettings.AllKeys)
            {
                // Extract the model name (e.g., "GPT4o" from "GPT4o.Endpoint")
                if (key.Contains("."))
                {
                    string modelName = key.Split('.')[0];
                    modelNames.Add(modelName);
                }
            }

            // Add the unique model names to the modelsComboBox
            modelsComboBox.Items.AddRange(modelNames.ToArray());

            // Optionally, set the first item as the default selected item
            if (modelsComboBox.Items.Count > 0)
            {
                modelsComboBox.SelectedIndex = 0;
            }
            modelsComboBox.SelectedIndexChanged += ModelsComboBox_SelectedIndexChanged;
        }

        private void ModelsComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Get the selected model from the modelsComboBox  
            string selectedModel = modelsComboBox.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedModel))
            {
                // Reinitialize aoai with the selected model  
                string selectedSystemMessage = promptBox.SelectedItem?.ToString() ?? "Default system message.";
                aoai = new AOAI(selectedSystemMessage, selectedModel);
            }
        }

        private void PopulateSystemMessageComboBox()
        {
            // Iterate through all app settings  
            foreach (string? key in ConfigurationManager.AppSettings.AllKeys) // Use nullable string for the key
            {
                // Check if the key starts with "systemmessage"  
                if (key != null && key.StartsWith("SystemPrompt", StringComparison.OrdinalIgnoreCase)) // Add null check for key
                {
                    string? value = ConfigurationManager.AppSettings[key]; // Use nullable string  

                    // Add the value to the ComboBox if it's not null or empty  
                    if (!string.IsNullOrEmpty(value))
                    {
                        promptBox.Items.Add(value);
                    }
                }

                // Optionally, set the first item as the default selected item
                if (promptBox.Items.Count > 0)
                {
                    promptBox.SelectedIndex = 0;
                }
            }
        }


        private async void InitializeWebView2Async()
        {
            try
            {
                await responseView.EnsureCoreWebView2Async();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Clipboard_listBox_DoubleClick(object sender, EventArgs e)
        {
            // Check if an item is selected  
            if (Clipboard_listBox.SelectedItem != null)
            {
                // Safely handle possible null value with null-coalescing operator  
                string selectedText = Clipboard_listBox.SelectedItem?.ToString() ?? string.Empty;

                // Get the current cursor position in promptTextBox
                int cursorPosition = promptTextBox.SelectionStart;

                // Insert the selected text at the cursor position
                promptTextBox.Text = promptTextBox.Text.Insert(cursorPosition, selectedText + Environment.NewLine);

                // Move the cursor to the end of the inserted text
                promptTextBox.SelectionStart = cursorPosition + selectedText.Length;
            }
        }
        private void InitializeContextMenu()
        {
            clipboardContextMenu = new ContextMenuStrip();

            // Add "Delete" menu item
            var deleteMenuItem = new ToolStripMenuItem("Delete");
            deleteMenuItem.Click += DeleteMenuItem_Click;
            clipboardContextMenu.Items.Add(deleteMenuItem);

            // Attach the ContextMenuStrip to the Clipboard_listBox
            Clipboard_listBox.ContextMenuStrip = clipboardContextMenu;

            // Handle right-click selection
            Clipboard_listBox.MouseDown += Clipboard_listBox_MouseDown;
        }

        private void Clipboard_listBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Select the item under the mouse cursor  
                int index = Clipboard_listBox.IndexFromPoint(e.Location);
                if (index >= 0 && index < Clipboard_listBox.Items.Count)
                {
                    Clipboard_listBox.SelectedIndex = index;
                }
            }
        }

        private void DeleteMenuItem_Click(object? sender, EventArgs e)
        {
            // Check if there are any selected items
            if (Clipboard_listBox.SelectedItems.Count > 0)
            {
                // Create a list to store the selected items
                var itemsToRemove = Clipboard_listBox.SelectedItems.Cast<object>().ToList();

                // Remove each selected item
                foreach (var item in itemsToRemove)
                {
                    Clipboard_listBox.Items.Remove(item);
                }
            }
        }

        private int lastHoveredIndex = -1; // Track the last hovered item index
        private void Clipboard_listBox_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the index of the item under the mouse pointer
            int index = Clipboard_listBox.IndexFromPoint(e.Location);

            // Only update the tooltip if the hovered item has changed
            if (index != lastHoveredIndex && index >= 0 && index < Clipboard_listBox.Items.Count)
            {
                lastHoveredIndex = index; // Update the last hovered index
                string? itemText = Clipboard_listBox.Items[index]?.ToString();
                clipboardToolTip.SetToolTip(Clipboard_listBox, itemText ?? string.Empty);
            }
            else if (index < 0 || index >= Clipboard_listBox.Items.Count)
            {
                lastHoveredIndex = -1; // Reset the last hovered index if no item is under the mouse
                clipboardToolTip.SetToolTip(Clipboard_listBox, string.Empty); // Clear the tooltip
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                ClipboardChanged();
            }
        }

        private void ClipboardChanged()
        {
            if (!this.enabledCheckBox.Checked)
                return;

            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText().Trim();

                // Only add the clipboard text if it is not null or empty
                if (!string.IsNullOrEmpty(clipboardText) && !Clipboard_listBox.Items.Contains(clipboardText))
                {
                    if (Clipboard_listBox.Items.Count >= 80)
                    {
                        for (int i = 0; i < 10; i++)
                            Clipboard_listBox.Items.RemoveAt(0);
                        this.promptTextBox.Clear();
                    }

                    // Store clipboardText in Clipboard_listBox
                    this.Clipboard_listBox.Items.Add(clipboardText);

                    // Append the clipboardText to promptTextBox
                    this.promptTextBox.AppendText(clipboardText + Environment.NewLine + Environment.NewLine);
                }
            }
            // Similarly check for images or other formats if needed  
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            RemoveClipboardFormatListener(this.Handle);
            base.OnFormClosed(e);
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.promptTextBox.Clear();
        }

        private AOAI aoai;
        private void InitializeAOAI()
        {
            // Ensure there is a selected item in promptBox
            if (promptBox.SelectedItem != null)
            {
                string selectedSystemMessage = promptBox.SelectedItem.ToString();
                aoai = new AOAI(selectedSystemMessage);
            }
            else
            {
                // Fallback to a default system message if no selection is made
                aoai = new AOAI("Default system message.");
            }
        }

        private async void generateButton_Click(object sender, EventArgs e)
        {
            // Get the prompt text from promptTextBox
            string prompt = promptTextBox.Text;

            // Call the extracted method
            await CallGetResponseAsync(prompt);
        }

        private async void explainButton_Click(object sender, EventArgs e)
        {
            // Get the prompt text from promptTextBox  
            string prompt = promptTextBox.Text + Environment.NewLine + Environment.NewLine + "Explain the above:";

            // Call the extracted method  
            await CallGetResponseAsync(prompt);
        }

        private async Task CallGetResponseAsync(string prompt)
        {
            try
            {
                loadingIndicator.Visible = true;
                string response = await aoai.GetResponseAsync(prompt);
                responseTextBox.Text = response;

                // Enable table support in the Markdown pipeline
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions() // This includes table support and more
                    .Build();

                string htmlContent = Markdown.ToHtml(response, pipeline);

                // Inject custom CSS for font styling
                string styledHtml = $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Calibri', sans-serif;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    {htmlContent}
</body>
</html>";

                // Display the styled HTML in the WebView2 control
                responseView.NavigateToString(styledHtml);

            }
            catch (Exception ex)
            {
                // Handle any errors and display them in responseTextBox
                responseTextBox.Text = $"Error: {ex.Message}";

            }
            finally
            {
                // Hide the loading indicator
                loadingIndicator.Visible = false;
            }
        }

        private async void respondButton_Click(object sender, EventArgs e)
        {
            // Get the prompt text from promptTextBox  
            string prompt = promptTextBox.Text + Environment.NewLine + Environment.NewLine + "Write an effective response to the above:";

            // Call the extracted method  
            await CallGetResponseAsync(prompt);
        }

        private async void summarizeButton_Click(object sender, EventArgs e)
        {
            // Get the prompt text from promptTextBox  
            string prompt = promptTextBox.Text + Environment.NewLine + Environment.NewLine + "Summarize the above and possible action items:";

            // Call the extracted method  
            await CallGetResponseAsync(prompt);
        }

        private void clearListButton_Click(object sender, EventArgs e)
        {
            Clipboard_listBox.Items.Clear();
        }

        private async void critiqueButton_Click(object sender, EventArgs e)
        {
            // Get the prompt text from promptTextBox  
            string prompt = promptTextBox.Text + Environment.NewLine + Environment.NewLine + "Review the above to provide constructive criticsm and new ideas:";

            // Call the extracted method  
            await CallGetResponseAsync(prompt);
        }
    }
}
