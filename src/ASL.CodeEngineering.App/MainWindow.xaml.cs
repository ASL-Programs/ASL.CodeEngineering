using System;
using System.Threading;
using System.Windows;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering
{
    public partial class MainWindow : Window
    {

        private readonly IAIProvider _aiProvider = new EchoAIProvider();


        public MainWindow()
        {
            InitializeComponent();
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string prompt = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            SendButton.IsEnabled = false;
            StatusTextBlock.Text = "Sending...";

            try
            {
                string response = await _aiProvider.SendChatAsync(prompt, CancellationToken.None);
                ResponseTextBox.Text = response;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
            }
            finally
            {
                SendButton.IsEnabled = true;
            }
        }
    }
}
