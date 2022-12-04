using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Person_Info
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetDiscText("");
        }

        // Unnecessary piece of code
        private void CreateAddress(string personName) => SendRequest("http://mr-maksis.ru/Index.php?FULLNAME=" + personName);

        /// <summary>
        /// Set image to image box into form
        /// </summary>
        private void SetImage(string address)
        {
            Uri path = new Uri(address);

            BitmapImage image = new BitmapImage(path);

            imagebox.Source = image;
        }

        /// <summary>
        /// Send request for get response :)
        /// </summary>
        private async void SendRequest(string address)
        {
            HttpClient httpClient = SetupClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, address);

            using var response = await httpClient.SendAsync(request);

            var responseText = await response.Content.ReadAsStringAsync();

            if(responseText.Length > 0)
                GetResponse(responseText);
            else
                MessageBox.Show("The data entered is not correct");
        }

        /// <summary>
        /// Create new Client with valid httpClientHandler
        /// </summary>
        private HttpClient SetupClient()
        {
            HttpClientHandler httpClientHandler= new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            HttpClient client = new HttpClient(httpClientHandler);

            return client;
        } 

        /// <summary>
        /// Call method ParseText by response
        /// </summary>
        private async void GetResponse(string text)
        {
            HttpClient httpClient = SetupClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, text);

            using var response = await httpClient.SendAsync(request);

            var responseText = await response.Content.ReadAsStringAsync();

            ParseText(responseText);
        }

        /// <summary>
        /// Set to all textboxes the text obtained from the attributes
        /// </summary>
        private void ParseText(string doc)
        {
            SetImage(GetAttributeBody("image", doc));

            tb_name.Text = GetAttributeBody("fullname", doc);

            tb_age.Text = GetAttributeBody("personage", doc);

            SetDiscText(GetAttributeBody("persondisc", doc));
        }

        /// <summary>
        /// RichTextBox...
        /// </summary>
        private void SetDiscText(string text)
        {
            FlowDocument mcFlowDoc = new FlowDocument();

            Paragraph para = new Paragraph();

            para.Inlines.Add(new Run(text));

            mcFlowDoc.Blocks.Add(para);

            tb_disc.Document = mcFlowDoc;
        }

        /// <summary>
        /// Get the body of current attribute
        /// </summary>
        private string GetAttributeBody(string attributeName, string baseText)
        {
            string startAttr = "<" + attributeName + ">";

            string endAttr = "</" + attributeName + ">";

            var pretext = baseText.Substring(baseText.IndexOf(startAttr) + startAttr.Length);

            pretext = pretext.Remove(pretext.IndexOf(endAttr));   

            return pretext;
        }

        private void search_button_Click(object sender, RoutedEventArgs e)
        {
            if(tb_search.Text.Length > 0)
                CreateAddress(tb_search.Text);
            else
                MessageBox.Show("enter a name, please");
        }
    } 
}
