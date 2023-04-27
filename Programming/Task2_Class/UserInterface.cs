using System;
using System.Collections.Generic;
namespace Task2_Class;

public interface IUserAction<T> where T: class
{
    List<T> ViewList();
    T ViewById(int id);
    List<T> Search(string query);
    List<T> Sort(string sortBy);
    T Create(T student);
    T Edit(T student);
    void Delete(int id);
}

