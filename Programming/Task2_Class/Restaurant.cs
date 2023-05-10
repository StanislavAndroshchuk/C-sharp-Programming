using System.Reflection;

namespace Task2_Class
{
    public class Restaurant : IClassEntity
    {
        public int Id { get; set; }
        public int Workers { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public StatesEnum ReadedState { get; set; }

        public Restaurant()
        {
            
        }

        public Restaurant(int id, int workers,string url, string name, DateTime date, StatesEnum state = StatesEnum.Draft)
        {
            Id = id;
            Workers = workers;
            Url = url;
            Title = name;
            Date = date;
            ReadedState = state;
        }
        public Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = new Dictionary<string, Delegate>
            {
                {"Id", Validation.ValidPositiveInt},
                {"Workers", Validation.ValidPositiveInt},
                {"Url", Validation.ValidUrl},
                {"Title", Validation.ValidString},
                {"Date", Validation.ValidDateTime}
            };
            return fieldValid;

        }
        public override string ToString()
        {
            string toReturn = "";
            foreach (PropertyInfo x in this.GetType().GetProperties())
            {
                toReturn += x.Name + " - " + x.GetValue(this) + "\n"; 
            }
            return toReturn;
        }

        public void ToWrite(int count)
        {
            ReadedState = StatesEnum.Draft;
            PropertyInfo[] properties = this.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Console.WriteLine("Enter value for " + property.Name + ":");
                string input = Console.ReadLine()!;
                object value = Convert.ChangeType(input, property.PropertyType);
                property.SetValue(this, value);
            }
        }
        private State _currentState => State.ConvertEnum(ReadedState, this);

        public void ChangeState(StatesEnum state)
        {
            ReadedState = state;
        }
        public void Publishing(User user)
        {
            _currentState.Publishing(user);
        }
    }
}

