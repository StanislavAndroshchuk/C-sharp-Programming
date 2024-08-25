namespace Task2_Class;

public interface IContext
{
    void ChangeState(StatesEnum state);
    void Publishing(User user);
}

public interface IClassEntity : IContext
{
    int Id { get; }
    Dictionary<string, Delegate> ToValidFields();
}



