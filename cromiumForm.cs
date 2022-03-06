using System;
using System.Windows.Forms;
using System.IO;
using CefSharp.WinForms;
using CefSharp;

namespace FreeBitcoinBot2
{
    public partial class cromiumForm : Form
    {
        private readonly ChromiumWebBrowser browser;

        private Timer hourTimer = new Timer();

        public cromiumForm()
        {
            InitializeComponent();

            hourTimer.Tick += new EventHandler(hourTimer_Tick);
            hourTimer.Interval = 60000 + getSec();

            browser = new ChromiumWebBrowser("https://freebitco.in/?op=home");

            //toolStripContainer.ContentPanel.Controls.Add(browser);
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.ConsoleMessage += OnBrowserConsoleMessage;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;

            hourTimer.Start();
        }

        private int getSec()
        {
            Random rnd = new Random();
            return rnd.Next(0, 1) * 1000;
        }

        private void hourTimer_Tick(object sender, EventArgs e)
        {
            int heures = DateTime.Now.Hour;

            hourTimer.Interval = 60000 + getSec();
            browser.Load("https://freebitco.in/?op=home");
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            var b = ((ChromiumWebBrowser)sender);

            //this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {

        }

        private async void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            if (!args.IsLoading)
            {
                try
                {
                    browser.ExecuteScriptAsync("SwitchPageTabs('free_play')");
                    sleep(2000);

                    string script = "document.getElementById('free_play_form_button').getAttribute('style')";
                    JavascriptResponse response = await browser.EvaluateScriptAsync(script);

                    if (response.Result == null)
                    {
                        browser.ExecuteScriptAsync("document.getElementById('free_play_form_button').click();");
                        sleep(1000);
                    }

                }
                catch (Exception e) {
                    Console.Write("Erreur : " + e.Message);
                }
            }
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            //this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {

        }

        private void SetCanGoBack(bool canGoBack)
        {

        }

        private void SetCanGoForward(bool canGoForward)
        {

        }

        public void DisplayOutput(string output)
        {

        }

        private void GoButtonClick(object sender, EventArgs e)
        {

        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        private void cromiumForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Application.Exit();
        }

        private void sleep(int numMilliSeconds)
        {
            System.Threading.Thread.Sleep(numMilliSeconds);
        }
    }
}
