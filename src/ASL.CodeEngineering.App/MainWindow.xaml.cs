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

            ResponseTextBox.Text = "Sending...";
            string response = await _aiProvider.SendChatAsync(prompt, CancellationToken.None);
            ResponseTextBox.Text = response;
        }
    }
}
