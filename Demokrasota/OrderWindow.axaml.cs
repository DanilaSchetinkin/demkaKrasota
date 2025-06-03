using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace Demokrasota;

public partial class OrderWindow : Window
{

    [Content]
    public TextBox SearchBox { get; set; }
    public OrderWindow()
    {
        InitializeComponent();
    }

   
    public void SearchBoxChanging(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text;
    }
}