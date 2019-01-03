using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OCMApp.Favorites
{
    /// <summary>
    /// Interaction logic for FavoriteItem.xaml
    /// </summary>
    public partial class FavoriteItem : UserControl
    {
        public FavoriteItem()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is FavoriteItemViewModel model)
            {
                ContentWrapper.Children.Clear();
                if (model.IsTextContent)
                {
                    var txt = new TextBox
                    {
                        IsReadOnly = true,
                        Width = 150,
                        TextWrapping = TextWrapping.NoWrap,
                    };
                    ContentWrapper.Children.Add(txt);

                    Binding myBinding = new Binding
                    {
                        Source = model,
                        Path = new PropertyPath("TextContent"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    BindingOperations.SetBinding(txt, TextBox.TextProperty, myBinding);
                }
                else
                {
                    var img = new Image
                    {
                        Width = 150,
                    };
                    ContentWrapper.Children.Add(img);

                    Binding myBinding = new Binding
                    {
                        Source = model,
                        Path = new PropertyPath("ImageContent"),
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    BindingOperations.SetBinding(img, Image.SourceProperty, myBinding);
                }
            }
        }
    }
}
