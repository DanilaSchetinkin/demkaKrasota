using Avalonia.Controls;
using Avalonia.Interactivity;
using Demokrasota.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Demokrasota
{
    public partial class AddClientWindow : Window
    {
        public AddClientWindow()
        {
            InitializeComponent();
            ClientTypeComboBox.SelectionChanged += ClientTypeComboBox_SelectionChanged;
        }

        private void ClientTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LegalFieldsPanel.IsVisible = ClientTypeComboBox.SelectedIndex == 0;
            IndividualFieldsPanel.IsVisible = ClientTypeComboBox.SelectedIndex == 1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using var context = new User20Context();

            try
            {
                string clientCode = GenerateClientCode(ClientTypeComboBox.SelectedIndex == 0 ? "ЮЛ" : "ФЛ");

                if (ClientTypeComboBox.SelectedIndex == 0) // Юридическое лицо
                {
                    var legalClient = new LegalClient
                    {
                        ClientCode = clientCode,
                        CompanyName = CompanyNameBox.Text,
                        Address = AddressBox.Text,
                        Inn = INNBox.Text,
                        BankAccount = RSBox.Text,
                        Bik = BIKBox.Text,
                        DirectorName = DirectorBox.Text,
                        ContactPerson = ContactBox.Text,
                        ContactPhone = PhoneBox.Text,
                        Email = EmailBox.Text,
                        Password = "default123"
                    };
                    context.LegalClients.Add(legalClient);

                    var client = new Client
                    {
                        ClientCode = clientCode,
                        Type = "ЮЛ",
                        ClientCodeLegal = clientCode // Устанавливаем ClientCodeLegal
                    };
                    context.Clients.Add(client);
                }
                else // Физическое лицо
                {
                    var individualClient = new IndividualClient
                    {
                        ClientCode = clientCode,
                        FullName = FLFIOBox.Text,
                        PassportData = PassportBox.Text,
                        BirthDate = DateOnly.FromDateTime(DateTime.Parse(BirthBox.Text)),
                        Address = AddresFlBox.Text,
                        Email = EmailFLBox.Text,
                        Password = "default123"
                    };
                    context.IndividualClients.Add(individualClient);

                    var client = new Client
                    {
                        ClientCode = clientCode,
                        Type = "ФЛ",
                        ClientCodeIndivid = clientCode // Устанавливаем ClientCodeIndivid
                    };
                    context.Clients.Add(client);
                }

                context.SaveChanges();
                this.Close();
            }
            catch (Exception ex)
            {
                var errorWindow = new Window
                {
                    Title = "Ошибка",
                    Content = new TextBlock { Text = $"Ошибка при сохранении: {ex.Message}" },
                    Width = 300,
                    Height = 150
                };
                errorWindow.ShowDialog(this);
            }
        }

        private string GenerateClientCode(string prefix)
        {
            using var context = new User20Context();
            var lastCode = context.Clients
                .Where(c => c.ClientCode.StartsWith(prefix))
                .OrderByDescending(c => c.ClientCode)
                .FirstOrDefault()?.ClientCode;

            if (string.IsNullOrEmpty(lastCode))
                return $"{prefix}-0001";

            var number = int.Parse(lastCode.Split('-')[1]) + 1;
            return $"{prefix}-{number:D4}";
        }
    }
}