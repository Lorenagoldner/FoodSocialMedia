using BlazorAppFood.Models;
using System;


namespace BlazorAppFood.Data
{
    public class UserState
    {
        public User? User { get; private set; }

        public event Action? OnChange;

        public void SetUser(User user)
        {
            User = user;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

