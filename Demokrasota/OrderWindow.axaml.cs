
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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


        public OrderWindow() : this(0)
        {
        }

        public OrderWindow(int employeeId)
        {
            InitializeComponent();
            currentEmployeeId = employeeId;
            LoadData();
            SetupEventHandlers();
        }

        public void LoadData()
        {
            using var context = new User20Context();

            // Загрузка клиентов
            var legalClients = context.LegalClients.ToList();
            var individualClients = context.IndividualClients.ToList();

            clients = individualClients
                .Select(c => new ClientViewModel
                {
                    Type = "individual",
                    CodeClient = c.ClientCode,
                    NameClient = c.FullName
                })
                .Concat(legalClients.Select(c => new ClientViewModel
                {
                    Type = "legal",
                    CodeClient = c.ClientCode,
                    NameClient = c.CompanyName
                }))
                .ToList();

            ComboBoxClients.ItemsSource = clients;
            ComboBoxClients.DisplayMemberBinding = new Avalonia.Data.Binding("NameClient");

            // Загрузка услуг
            services = context.Services
                .Select(s => new ServiceViewModel
                {
                    ServiceId = s.Id,
                    ServiceName = s.ServiceName,
                    IsSelected = false
                })
                .ToList();

            ListBoxService.ItemsSource = services;

            // Установка следующего номера заказа
            SetNextOrderNumber();
        }

        public void SetupEventHandlers()
        {
            VesselCodeTextBox.GotFocus += (s, e) => ShowNextOrderHint();
            ClientTypeComboBox.SelectionChanged += (s, e) => FilterClientsByType();
        }

        public void ShowNextOrderHint()
        {
            VesselCodeHint.Text = $"Рекомендуемый номер: {GenerateNextOrderNumber()}";
        }

        public void FilterClientsByType()
        {
            string clientType = ClientTypeComboBox.SelectedIndex == 0 ? "ФЛ" : "ЮЛ";
            ComboBoxClients.ItemsSource = clients.Where(c => c.Type == clientType).ToList();
        }

        public void VesselCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidateVesselCode();
                e.Handled = true;
            }
        }

        public void ValidateVesselCode()
        {
            string enteredCode = VesselCodeTextBox.Text;

            using var context = new User20Context();
            bool codeExists = context.Orders
                .Any(o => o.OrderNumber == enteredCode && o.Status != "Архив");

            if (codeExists)
            {
                ShowStatus("Номер уже существует!", Brushes.Red);
            }
            else
            {
                ShowStatus("Номер доступен", Brushes.Green);
            }
        }

        public void Add_Client(object sender, RoutedEventArgs e)
        {
            var addClientWindow = new AddClientWindow();
            addClientWindow.Closed += (s, args) =>
            {
                LoadData(); // Обновляем данные после закрытия окна добавления клиента
            };
            addClientWindow.ShowDialog(this);
        }

        public void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                CreateNewOrder();
                ShowStatus("Заказ успешно создан!", Brushes.Green);
                ResetForm();
            }
            catch (Exception ex)
            {
                ShowStatus($"Ошибка: {ex.Message}", Brushes.Red);
            }
        }

        public bool ValidateForm()
        {
            bool isValid = true;

            // Проверка кода сосуда
            if (string.IsNullOrWhiteSpace(VesselCodeTextBox.Text))
            {
                ShowStatus("Введите код лабораторного сосуда", Brushes.Red);
                isValid = false;
            }

            // Проверка клиента
            if (ComboBoxClients.SelectedItem == null)
            {
                ErrorClientsTextBlock.IsVisible = true;
                isValid = false;
            }
            else
            {
                ErrorClientsTextBlock.IsVisible = false;
            }

            // Проверка услуг
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

        public void CreateNewOrder()
        {
            try
            {
                if (ComboBoxClients.SelectedItem == null)
                {
                    ShowStatus("Выберите клиента!", Brushes.Red);
                    return;
                }

                var selectedServices = services.Where(s => s.IsSelected).ToList();
                if (!selectedServices.Any())
                {
                    ShowStatus("Выберите хотя бы одну услугу!", Brushes.Red);
                    return;
                }

                using var context = new User20Context();
                var client = (ClientViewModel)ComboBoxClients.SelectedItem;
                var lastId = context.Orders.OrderByDescending(o=>o.Id).FirstOrDefault();
                int chelickId = lastId.Id + 1;
                var newOrder = new Order
                {
                    Id = chelickId,
                    OrderNumber = VesselCodeTextBox.Text,
                    CreationDate = DateOnly.FromDateTime(DateTime.Now),
                    ClientCode = client.CodeClient,
                    Services = string.Join(",", selectedServices.Select(s => s.ServiceId)),
                    Status = "Новая",
                    EmployeeId = currentEmployeeId,
                    ExecutionTime = "8 ч" // Временное значение
                };


                context.Orders.Add(newOrder);
                context.SaveChanges();

                ShowStatus("Заказ успешно создан!", Brushes.Green);
                ResetForm();
            }
            catch (Exception ex)
            {
                ShowStatus($"Ошибка: {ex.Message}", Brushes.Red);
            }
        }

        public string CalculateExecutionTime(List<ServiceViewModel> selectedServices)
        {
            
            return "8 ч"; // Временная заглушка
        }

        public void SetNextOrderNumber()
        {
            VesselCodeTextBox.Text = GenerateNextOrderNumber();
        }

        public string GenerateNextOrderNumber()
        {
            using var context = new User20Context();

            // Получаем последний номер заказа
            var lastOrder = context.Orders
                .OrderByDescending(o => o.Id)
                .FirstOrDefault();

            int lastNumber = lastOrder != null ?
                int.Parse(lastOrder.OrderNumber.Split('/')[0]) : 0;

            return $"{lastNumber + 1}/{DateTime.Now:dd.MM.yyyy}";
        }

        public void ShowStatus(string message, IBrush color)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = color;
            StatusTextBlock.IsVisible = true;
        }

        public void ResetForm()
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