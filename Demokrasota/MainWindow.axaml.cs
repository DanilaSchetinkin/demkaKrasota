using System.Linq;
using Avalonia.Controls;
using Demokrasota.Models;


namespace Demokrasota
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            using var context = new User20Context();

            

        }

        private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string email = Log.Text;
            string password = Pass.Text;

            using (var context = new User20Context())
            {
                var employee = context.Employees
                    .FirstOrDefault(e => e.Email == email && e.Password == password);

                if (employee != null)
                {

                    var orderWindow = new OrderWindow(employee.Id);
                    orderWindow.Show();
                    this.Close();
                }
                else
                {
                    ErrorBox.IsVisible = true;
                }
            }
        }
    }
}