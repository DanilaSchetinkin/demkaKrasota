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
            using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            try
            {
                string clientCode = GenerateClientCode(ClientTypeComboBox.SelectedIndex == 0 ? "ЮЛ" : "ФЛ");

                // Проверка на дубликат
                if (context.Clients.Any(c => c.ClientCode == clientCode))
                    throw new Exception($"Клиент с кодом {clientCode} уже существует!");

                if (ClientTypeComboBox.SelectedIndex == 0) // Юр. лицо
                {
                    if(string.IsNullOrWhiteSpace(CompanyNameBox.Text)||
                        string.IsNullOrWhiteSpace(AddressBox.Text)||
                        string.IsNullOrWhiteSpace(INNBox.Text)||
                        string.IsNullOrWhiteSpace(RSBox.Text)||
                        string.IsNullOrWhiteSpace(BIKBox.Text)||
                        string.IsNullOrWhiteSpace(DirectorBox.Text)||
                        string.IsNullOrWhiteSpace(ContactBox.Text)||
                        string.IsNullOrWhiteSpace(PhoneBox.Text)||
                        string.IsNullOrWhiteSpace(EmailBox.Text)||
                        string.IsNullOrWhiteSpace(ClientCodeBox.Text)
                        )
                    {




                                    return;
                    }
                    
                    
                   

                    var legalClient = new LegalClient {
                    CompanyName = CompanyNameBox.Text,
                    Address = AddressBox.Text,  
                    Inn = INNBox.Text,
                    BankAccount = RSBox.Text,
                    DirectorName = DirectorBox.Text,
                    ContactPerson = ContactBox.Text,
                    ContactPhone = PhoneBox.Text,
                    Email = EmailBox.Text,
                    ClientCode = ClientCodeBox.Text


                    };
                    context.LegalClients.Add(legalClient);

                    // Сначала сохраняем LegalClient
                    context.SaveChanges();

                    var client = new Client
                    {
                        ClientCode = clientCode,
                        Type = "ЮЛ",
                        ClientCodeLegal = clientCode
                    };
                    context.Clients.Add(client);
                }
                else // Физ. лицо
                {

                    if(string.IsNullOrWhiteSpace(FLFIOBox.Text)||
                        string.IsNullOrWhiteSpace(BirthBox.Text)||
                        string.IsNullOrWhiteSpace(AddresFlBox.Text)||
                        string.IsNullOrWhiteSpace(PassportBox.Text)||
                        string.IsNullOrWhiteSpace(EmailFLBox.Text)||
                        string.IsNullOrWhiteSpace(ClientCodeFLBox.Text)
                        )
                    {
                        return;
                    }

                    var individualClient = new IndividualClient {
                    FullName = FLFIOBox.Text,
                    BirthDate = DateOnly.Parse(BirthBox.Text),
                    Address = AddresFlBox.Text,
                    Password = PassportBox.Text,
                    Email = EmailFLBox.Text,
                    ClientCode = ClientCodeFLBox.Text
                    
                    };
                    context.IndividualClients.Add(individualClient);

                    // Сначала сохраняем IndividualClient
                    context.SaveChanges();

                    var client = new Client
                    {
                        ClientCode = clientCode,
                        Type = "ФЛ",
                        ClientCodeIndivid = clientCode
                    };
                    context.Clients.Add(client);
                }

                context.SaveChanges();
                transaction.Commit();
                this.Close();
            }
            catch (DbUpdateException dbEx)
            {
                transaction.Rollback();
                ShowError($"Ошибка БД: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            var errorWindow = new Window
            {
                Title = "Ошибка",
                Content = new TextBlock { Text = message },
                Width = 400,
                Height = 200
            };
            errorWindow.ShowDialog(this);
        }

        private string GenerateClientCode(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                prefix = "CLT"; // Резервный префикс

            using var context = new User20Context();
            var lastCode = context.Clients
                .Where(c => c.ClientCode.StartsWith(prefix))
                .OrderByDescending(c => c.ClientCode)
                .FirstOrDefault()?.ClientCode;

            return lastCode == null
                ? $"{prefix}1"
                : $"{prefix}{int.Parse(lastCode[prefix.Length..]) + 1}";
        }
    }
}