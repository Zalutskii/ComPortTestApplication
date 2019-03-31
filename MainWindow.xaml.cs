namespace ComPortTestApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {       
        public MainWindow()
        {
            InitializeComponent();

            ComPortViewLeft1.DataContext = new ComPortViewModels();
            ComPortViewLeft2.DataContext = new ComPortViewModels();
        }
    }
}