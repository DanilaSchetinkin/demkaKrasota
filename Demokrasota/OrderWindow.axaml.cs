using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using Demokrasota.Models;

namespace Demokrasota;

public partial class OrderWindow : Window
{
    private List<ClientsPresenter> clients;
    private List<ServicesPresenter> services;
    
   
    public OrderWindow()
    {
        InitializeComponent();
        using var context = new User20Context();

        var legal_clients = context.LegalClients.ToList();
        var individual_clients = context.IndividualClients.ToList();
        var prosto_clients = context.Clients.ToList();
        var employee = context.Employees.ToList();
        var services_pupupu = context.Services.ToList();

        

        clients = prosto_clients.Select(clients => new ClientsPresenter
        {
            Type = clients.Type,
            CodeClient = clients.ClientCode
        }).ToList();

        services = services_pupupu.Select(services => new ServicesPresenter
        {
            ServiceId = services.Id,
            ServiceName = services.ServiceName
        }).ToList();
    }

    public class ClientsPresenter
    {
        public string Type {  get; set; }
        public string CodeClient {  get; set; }

        public string NameClient { get; set; }
    }

    public class ServicesPresenter
    {
        public int ServiceId {  get; set; }
        public string ServiceName { get; set; }


    }

    private void Add_Client(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }


    /*public void SearchBoxChanging(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text;
    }*/


}