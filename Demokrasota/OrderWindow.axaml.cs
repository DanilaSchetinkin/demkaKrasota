
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Demokrasota.Models;

namespace Demokrasota
{
    public partial class OrderWindow : Window
    {
        private List<ClientViewModel> clients;
        private List<ServiceViewModel> services;
        private int currentEmployeeId;

        public OrderWindow(int employeeId)
        {
            InitializeComponent();
            currentEmployeeId = employeeId;
            LoadData();
            SetupEventHandlers();
        }

        private void LoadData()
        {
            using var context = new User20Context();

            // �������� ��������
            var legalClients = context.LegalClients.ToList();
            var individualClients = context.IndividualClients.ToList();

            clients = individualClients
                .Select(c => new ClientViewModel
                {
                    Type = "��",
                    CodeClient = c.ClientCode,
                    NameClient = c.FullName
                })
                .Concat(legalClients.Select(c => new ClientViewModel
                {
                    Type = "��",
                    CodeClient = c.ClientCode,
                    NameClient = c.CompanyName
                }))
                .ToList();

            ComboBoxClients.ItemsSource = clients;
            ComboBoxClients.DisplayMemberBinding = new Avalonia.Data.Binding("NameClient");

            // �������� �����
            services = context.Services
                .Select(s => new ServiceViewModel
                {
                    ServiceId = s.Id,
                    ServiceName = s.ServiceName,
                    IsSelected = false
                })
                .ToList();

            ListBoxService.ItemsSource = services;

            // ��������� ���������� ������ ������
            SetNextOrderNumber();
        }

        private void SetupEventHandlers()
        {
            VesselCodeTextBox.GotFocus += (s, e) => ShowNextOrderHint();
            ClientTypeComboBox.SelectionChanged += (s, e) => FilterClientsByType();
        }

        private void ShowNextOrderHint()
        {
            VesselCodeHint.Text = $"������������� �����: {GenerateNextOrderNumber()}";
        }

        private void FilterClientsByType()
        {
            string clientType = ClientTypeComboBox.SelectedIndex == 0 ? "��" : "��";
            ComboBoxClients.ItemsSource = clients.Where(c => c.Type == clientType).ToList();
        }

        private void VesselCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateVesselCode();
                e.Handled = true;
            }
        }

        private void ValidateVesselCode()
        {
            string enteredCode = VesselCodeTextBox.Text;

            using var context = new User20Context();
            bool codeExists = context.Orders
                .Any(o => o.OrderNumber == enteredCode && o.Status != "�����");

            if (codeExists)
            {
                ShowStatus("����� ��� ����������!", Brushes.Red);
            }
            else
            {
                ShowStatus("����� ��������", Brushes.Green);
            }
        }

        private void Add_Client(object sender, RoutedEventArgs e)
        {
            var addClientWindow = new AddClientWindow();
            addClientWindow.Closed += (s, args) =>
            {
                LoadData(); // ��������� ������ ����� �������� ���� ���������� �������
            };
            addClientWindow.ShowDialog(this);
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                CreateNewOrder();
                ShowStatus("����� ������� ������!", Brushes.Green);
                ResetForm();
            }
            catch (Exception ex)
            {
                ShowStatus($"������: {ex.Message}", Brushes.Red);
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // �������� ���� ������
            if (string.IsNullOrWhiteSpace(VesselCodeTextBox.Text))
            {
                ShowStatus("������� ��� ������������� ������", Brushes.Red);
                isValid = false;
            }

            // �������� �������
            if (ComboBoxClients.SelectedItem == null)
            {
                ErrorClientsTextBlock.IsVisible = true;
                isValid = false;
            }
            else
            {
                ErrorClientsTextBlock.IsVisible = false;
            }

            // �������� �����
            var selectedServices = services.Where(s => s.IsSelected).ToList();
            if (selectedServices.Count == 0)
            {
                ErrorServicesTextBlock.IsVisible = true;
                isValid = false;
            }
            else
            {
                ErrorServicesTextBlock.IsVisible = false;
            }

            return isValid;
        }

        private void CreateNewOrder()
        {
            using var context = new User20Context();

            var selectedClient = (ClientViewModel)ComboBoxClients.SelectedItem;
            var selectedServices = services.Where(s => s.IsSelected).ToList();

            var newOrder = new Order
            {
                OrderNumber = VesselCodeTextBox.Text,
                CreationDate = DateOnly.FromDateTime(DateTime.Now),
                ClientCode = selectedClient.CodeClient,
                Services = string.Join(", ", selectedServices.Select(s => s.ServiceId)),
                Status = "�����",
                EmployeeId = currentEmployeeId,
                ExecutionTime = CalculateExecutionTime(selectedServices)
            };

            context.Orders.Add(newOrder);
            context.SaveChanges();
        }

        private string CalculateExecutionTime(List<ServiceViewModel> selectedServices)
        {
            // ����� ����� ����������� ������ ������� ������� ����������
            // �� ������ ��������� �����
            return "8 �"; // ��������� ��������
        }

        private void SetNextOrderNumber()
        {
            VesselCodeTextBox.Text = GenerateNextOrderNumber();
        }

        private string GenerateNextOrderNumber()
        {
            using var context = new User20Context();

            // �������� ��������� ����� ������ (�������� ��������)
            var lastOrder = context.Orders
                .Where(o => o.Status != "�����")
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            int lastNumber = lastOrder != null ?
                int.Parse(lastOrder.OrderNumber.Split('/')[0]) : 0;

            return $"{lastNumber + 1}/{DateTime.Now:dd.MM.yyyy}";
        }

        private void ShowStatus(string message, IBrush color)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = color;
            StatusTextBlock.IsVisible = true;
        }

        private void ResetForm()
        {
            VesselCodeTextBox.Text = GenerateNextOrderNumber();
            ComboBoxClients.SelectedIndex = -1;
            foreach (var service in services)
            {
                service.IsSelected = false;
            }
            ListBoxService.ItemsSource = null;
            ListBoxService.ItemsSource = services;
            ErrorClientsTextBlock.IsVisible = false;
            ErrorServicesTextBlock.IsVisible = false;
        }
    }

    public class ClientViewModel
    {
        public string Type { get; set; }
        public string CodeClient { get; set; }
        public string NameClient { get; set; }
    }

    public class ServiceViewModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public bool IsSelected { get; set; }
    }
}