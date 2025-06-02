using Markdig;
using Microsoft.Web.WebView2.WinForms;
using System.Runtime.InteropServices;

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

            InitializeAOAI();

            InitializeContextMenu();

            InitializeWebView2Async();

            PopulateSystemMessageComboBox();

            PopulateModelsComboBox();

            // Set the default tab to Web
            tabControl1.SelectedIndex = 1;

            // Enable custom drawing for tabs and add close button
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += TabControl1_DrawItem;
            tabControl1.MouseDown += TabControl1_MouseDown;
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
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                PastePlainText();
            }
        }

        private void PastePlainText()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                int selectionIndex = promptTextBox.SelectionStart;
                int selectionLength = promptTextBox.SelectionLength;
                promptTextBox.Text = promptTextBox.Text.Remove(selectionIndex, selectionLength)
                    .Insert(selectionIndex, text);
                promptTextBox.SelectionStart = selectionIndex + text.Length;
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
            var modelNames = AppSettings.GetAzureModelNames().ToArray();
            if (!modelNames.Any())
                return;

            modelsComboBox.Items.AddRange(modelNames);

            // Optionally, set the first item as the default selected item
            if (modelsComboBox.Items.Count > 1)
                modelsComboBox.SelectedIndex = 1;

            modelsComboBox.SelectedIndexChanged += ModelsComboBox_SelectedIndexChanged;
        }

        private void PopulateSystemMessageComboBox()
        {

            var systemPrompts = AppSettings.GetSystemPrompts().ToArray();
            if (!systemPrompts.Any())
                return;

            promptBox.Items.AddRange(systemPrompts);

            // Optionally, set the first item as the default selected item
            if (promptBox.Items.Count > 0)
                promptBox.SelectedIndex = 0;
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
                if (itemText?.Length > 400)
                {
                    itemText = itemText.Substring(0, 400) + "...";
                }
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

        private async void ClipboardChanged()
        {
            //if (Clipboard.ContainsData(DataFormats.Html))
            //{
            //    AOAI.CreateChatMessageFromHtml(Clipboard.GetText(TextDataFormat.Html));
            //}
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
                    if (enabledCheckBox.Checked)
                        this.promptTextBox.AppendText(clipboardText + Environment.NewLine + Environment.NewLine);
                }
            }
            // Handle image
            else if (Clipboard.ContainsImage())
            {
                try
                {
                    var image = Clipboard.GetImage();
                    if (image != null)
                    {
                        //if (enabledCheckBox.Checked)
                        //    promptTextBox.Paste();

                        // Convert Image to PNG byte array
                        using (var ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;
                            var imageBytes = BinaryData.FromBytes(ms.ToArray());
                            string mediaType = "image/png";

                            // Convert image to base64 for HTML embedding
                            string base64 = Convert.ToBase64String(ms.ToArray());
                            string imgHtml = $"<img src=\"data:image/png;base64,{base64}\" style=\"max-width:80%;height:auto;\" />";

                            loadingIndicator.Visible = true;

                            // extract context from previous images in the listbox
                            string context = string.Empty;
                            for (int i = Clipboard_listBox.Items.Count - 1; i >= 0; i--)
                            {
                                if (Clipboard_listBox.Items[i] is string itemText && itemText.StartsWith("<img src=\"data:image/png;base64,"))
                                {
                                    // If the item is an image, use it as context
                                    imgHtml = Clipboard_listBox.Items[i].ToString() + "<br/>" + imgHtml;
                                    break;
                                }
                            }

                            // Call AOAI.AnalyzeImage and display the result
                            string result = await aoai.AnalyzeImage(imageBytes, mediaType, context);
                            responseTextBox.Text = result;

                            // Add the result to the Clipboard_listBox
                            Clipboard_listBox.Items.Add(result);

                            string styledHtml = CreateStyledHtml(result, imgHtml);
                            ShowResponseInNewTab(styledHtml, "Image");
                        }
                    }
                }
                catch (Exception ex)
                    {
                        responseTextBox.Text = $"Error analyzing image: {ex.Message}";
                    }
                    finally
                    {
                        loadingIndicator.Visible = false;
                    }
            }
        }

        private static string CreateStyledHtml(string result, string imgHtml = "")
        {
            // Render Markdown result to HTML
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string htmlContent = Markdig.Markdown.ToHtml(result, pipeline);

            if (!string.IsNullOrEmpty(imgHtml))
                imgHtml = $"<div>{imgHtml}</div><br/>";

            // Combine image and analysis in HTML
            string styledHtml = $@"
<html>
<head>
    <style>
        body {{
            font-family: 'Calibri', sans-serif;
            font-size: 14px;
        }}
    </style>
    <script src=""https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js""></script>
</head>
<body>
    {imgHtml}
    {htmlContent}
</body>
</html>";
            return styledHtml;
        }

        private void ShowResponseInNewTab(string htmlContent, string tabTitle = "Response")
        {
            // Create a new WebView2 control
            var webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Ensure WebView2 is initialized before navigation
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                webView.NavigateToString(htmlContent);
            };
            webView.EnsureCoreWebView2Async();

            // Create a new TabPage
            var tabPage = new TabPage
            {
                Text = $"{tabTitle}{tabControl1.TabPages.Count - 1}   "
            };
            tabPage.Controls.Add(webView);

            // Add the new tab and select it
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectedTab = tabPage;
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

        private AOAI? aoai;
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
            await GetLlmResponseAsync(promptTextBox.Text);
        }

        private async void explainButton_Click(object sender, EventArgs e)
        {
            await GetLlmResponseAsync(promptTextBox.Text + 
                Environment.NewLine + Environment.NewLine + 
                "Explain the above:");
        }

        private async void respondButton_Click(object sender, EventArgs e)
        {
            await GetLlmResponseAsync(promptTextBox.Text +
                Environment.NewLine + Environment.NewLine +
                "Without beting too aggreable, write ane effective response to the above :");
        }

        private async void summarizeButton_Click(object sender, EventArgs e)
        {
            await GetLlmResponseAsync(promptTextBox.Text +
                Environment.NewLine + Environment.NewLine +
                "Summarize the above and then state possible action items:");
        }

        private async void critiqueButton_Click(object sender, EventArgs e)
        {
            await GetLlmResponseAsync(promptTextBox.Text +
                Environment.NewLine + Environment.NewLine +
                "Review the above to provide constructive criticsm and new ideas:");
        }

        private async Task GetLlmResponseAsync(string prompt, string tabName = null)
        {
            try
            {
                loadingIndicator.Visible = true;
                string response = await aoai.GetResponseAsync(prompt);
                responseTextBox.Text = response;

                string styledHtml = CreateStyledHtml(response);

                if (newTabCheckbox.Checked)
                    ShowResponseInNewTab(styledHtml, tabName);
                else
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

        private void clearListButton_Click(object sender, EventArgs e)
        {
            Clipboard_listBox.Items.Clear();
            promptTextBox.Clear();
        }

        // Draw the tab text and the 'x' close button
        private void TabControl1_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tabRect = tabControl1.GetTabRect(e.Index);
            var tabText = tabControl1.TabPages[e.Index].Text;


            Rectangle textRect = new Rectangle(tabRect.X+2, tabRect.Y, tabRect.Width, tabRect.Height);
            TextRenderer.DrawText(e.Graphics, tabText, e.Font, textRect, Color.Black, TextFormatFlags.Left);

            // Draw 'x' close button on all tabs except the first two
            if (e.Index > 1)
            {
                Rectangle closeRect = new Rectangle(tabRect.Right - 18, tabRect.Top + 6, 12, 12);
                using (Font closeFont = new Font("Arial", 8, FontStyle.Bold))
                {
                    // Center the 'X' in the closeRect
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString("×", closeFont, Brushes.Black, closeRect, sf);
                }
            }
        }

        // Handle mouse click to close tab if 'X' is clicked
        private void TabControl1_MouseDown(object? sender, MouseEventArgs e)
        {
            for (int i = 2; i < tabControl1.TabPages.Count; i++) // skip first two tabs
            {
                var tabRect = tabControl1.GetTabRect(i);
                Rectangle closeRect = new Rectangle(tabRect.Right - 18, tabRect.Top + 6, 12, 12);
                if (closeRect.Contains(e.Location))
                {
                    var tabToRemove = tabControl1.TabPages[i];
                    tabControl1.TabPages.Remove(tabToRemove);
                    tabToRemove.Dispose();
                    break;
                }
            }
        }

        public void AddTeamsTab()
        {
            var tabPage = new TabPage("Teams");
            var webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            tabPage.Controls.Add(webView);
            tabControl1.TabPages.Add(tabPage);

            // Set the new tab as the selected tab
            tabControl1.SelectedTab = tabPage;

            // Initialize WebView2, set user agent, and navigate
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (webView.CoreWebView2 != null)
                {
                    // Emulate Microsoft Edge user agent (update version as needed)
                    webView.CoreWebView2.Settings.UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0";
                    webView.CoreWebView2.Navigate("https://teams.microsoft.com/v2/");
                }
            };
            webView.EnsureCoreWebView2Async();
        }
    }
}
