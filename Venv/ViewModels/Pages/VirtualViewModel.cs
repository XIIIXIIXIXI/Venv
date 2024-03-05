using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;


namespace Venv.ViewModels.Pages
{
    public partial class VirtualViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Person> people;

        public VirtualViewModel()
        {
            // Initialize the collection with some data
            People = new ObservableCollection<Person>
        {
            new Person { Name = "John Doe", Age = 30 },
            new Person { Name = "Jane Doe", Age = 28 }
            // Add more items as necessary
        };
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    
}

