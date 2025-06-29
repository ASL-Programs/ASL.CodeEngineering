using System;
using System.Windows;

namespace ASL.CodeEngineering;

public partial class LoginWindow : Window
{
    public string UserName => UserTextBox.Text.Trim();
    public Role SelectedRole => (Role)RoleComboBox.SelectedItem!;

    public LoginWindow()
    {
        InitializeComponent();
        RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
        RoleComboBox.SelectedIndex = 0;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
