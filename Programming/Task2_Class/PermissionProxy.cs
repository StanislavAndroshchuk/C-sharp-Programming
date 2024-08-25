using System.Collections.Generic;
namespace Task2_Class
{
    public class PermissionProxy<T> : IUserAction<T> where T :class 
    {
        private readonly User user;
        private readonly IUserAction<T> userActions;
        public PermissionProxy(User user, IUserAction<T> userActions)
        {
            this.user = user;
            this.userActions = userActions;
        }

        public bool IsHavePermission()
        {
            return user.Role == Roles.Admin;
        }
        public List<T> ViewList()
        {
            return userActions.ViewList();
        }

        public T ViewById(int id)
        {
            return userActions.ViewById(id);
        }

        public List<T> Search(string query)
        {
            return userActions.Search(query);
        }

        public List<T> Sort(string sortBy)
        {
            return userActions.Sort(sortBy);
        }

        public T Create(T student)
        {
            if (user.IsManager() || user.IsAdmin())
            {
                return userActions.Create(student);
            }
            else
            {
                throw new Exception("Error: No permission!");
            }
        }

        public T Edit(int getId, string attribute, object value)
        {
            if (user.IsManager() || user.IsAdmin())
            {
                return userActions.Edit(getId,attribute,value);
            }
            else
            {
                throw new Exception("Error: No permission!");
            }
        }

        public T Delete(int id)
        {
            if (user.IsAdmin())
            {
                return userActions.Delete(id);
            }
            else
            {
                throw new Exception("Error: No permission!");
            }
        }

        public void Publishing(int id)
        {
            if (user.IsManager() || user.IsAdmin())
            {
                userActions.Publishing(id);
            }
            else
            {
                throw new Exception("Error: No permission!");
            }
        }
    }
}

