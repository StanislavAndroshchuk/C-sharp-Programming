namespace Task2_Class;

public interface IUserAction<T> where T: class
{
    bool IsHavePermission();
    List<T> ViewList();
    T ViewById(int id);
    List<T> Search(string query);
    List<T> Sort(string sortBy);
    T Create(T student);
    T Edit(int getId, string attribute, object value);
    T Delete(int id);
    void Publishing(int id);
}

