using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Enkripto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        UpdateProgressBarDelegate updatePbDelegate;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void browseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            browseButton.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void browseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            browseButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF171616"));
        }

        private void exitLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            exitLabel.Foreground = new SolidColorBrush(Colors.DarkRed);
        }

        private void exitLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            exitLabel.Foreground = new SolidColorBrush(Colors.White);
        }

        private void exitLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void minimizeLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void minimizeLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            minimizeLabel.Foreground = new SolidColorBrush(Colors.DarkRed);
        }

        private void minimizeLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            minimizeLabel.Foreground = new SolidColorBrush(Colors.White);
        }

        private void EnkriptoWindow_MouseDown(object sender, MouseEventArgs e)
        {
            //Allow window to drag
            DragMove();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".mp4";
            dlg.Filter = "MP4 file (.mp4)|*.mp4";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                filenameBox.Text = filename;
                passwordBox.Focus();
            }
        }

        // Encrypt a file. 
        public static void AddEncryption(string FileName)
        {
            File.Encrypt(FileName);
        }

        // Decrypt a file. 
        public static void RemoveEncryption(string FileName)
        {
            File.Decrypt(FileName);
        }

        private void encryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (filenameBox.Text != string.Empty)
            {
                StartEncryption();        
            }    
        }

        private void StartEncryption()
        {
            try
            {
                string inFile = filenameBox.Text;
                string outFile = filenameBox.Text + ".fcfe";
                string password = passwordBox.Text;
                progressBar.Visibility = System.Windows.Visibility.Visible;
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;

                updatePbDelegate = new UpdateProgressBarDelegate(progressBar.SetValue);

                CryptoProgressCallBack cb = new CryptoProgressCallBack(this.ProgressCallBackEncrypt);
                CryptoHelp.EncryptFile(inFile, outFile, password, cb);

                progressBar.Value = 0;
                filenameBox.Text = "";
                passwordBox.Text = "";
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                encryptButton.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show(ex.Message);
            }
        }       

        void ProgressCallBackEncrypt(int min, int max, int value)
        {
            var percentage = ((double)value / (double)max) * (double)100;
            //progressBar.Value = value;
            if (percentage > 0 && percentage < 100)
                Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, percentage });
        }

        private void Process()
        {
            //Configure the ProgressBar
            progressBar.Minimum = 0;
            progressBar.Maximum = short.MaxValue;
            progressBar.Value = 0;

            //Stores the value of the ProgressBar
            double value = 0;

            //Create a new instance of our ProgressBar Delegate that points
            // to the ProgressBar's SetValue method.
            UpdateProgressBarDelegate updatePbDelegate =
                new UpdateProgressBarDelegate(progressBar.SetValue);
            progressBar.Visibility = System.Windows.Visibility.Visible;
            //Tight Loop: Loop until the ProgressBar.Value reaches the max
            do
            {
                value += 1;

                /*Update the Value of the ProgressBar:
                    1) Pass the "updatePbDelegate" delegate
                       that points to the ProgressBar1.SetValue method
                    2) Set the DispatcherPriority to "Background"
                    3) Pass an Object() Array containing the property
                       to update (ProgressBar.ValueProperty) and the new value */
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

                if(progressBar.Value == progressBar.Maximum)
                {
                    filenameBox.Text = "";
                    passwordBox.Text = "";
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            while (progressBar.Value != progressBar.Maximum);
        }

        private void encryptButton_MouseEnter(object sender, MouseEventArgs e)
        {
            encryptButton.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void encryptButton_MouseLeave(object sender, MouseEventArgs e)
        {
            encryptButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF171616"));
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartEncryption();
                e.Handled = true;
            }
            else
                e.Handled = false;
        }
    }
}
