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
            if (user.Role == Roles.Admin)
            {
                return userActions.Create(student);
            }
            else
            {
                throw new Exception("Only admin can handle this operation");
            }
        }

        public T Edit(T student)
        {
            if (user.Role == Roles.Admin)
            {
                return userActions.Edit(student);
            }
            else
            {
                throw new Exception("Only admin can handle this operation");
            }
        }

        public void Delete(int id)
        {
            if (user.Role == Roles.Admin)
            {
                userActions.Delete(id);
            }
            else
            {
                throw new Exception("Only admin can handle this operation");
            }
        }
    }
}

