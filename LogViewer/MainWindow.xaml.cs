using System.Windows;
using LogViewer.ViewModels;

namespace LogViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        /// <summary>
        /// Occurs when the element is laid out, rendered, and ready for interaction
        /// </summary>
        /// <param name="sender">The object where the event handler is attached</param>
        /// <param name="e">The event data</param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) =>
            ((AnalysisViewModel) DataContext).ValidateSettings();
    }
}
