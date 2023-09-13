using DineDash.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineDash.Interface
{
    public interface IGoogleManager
    {
        void Login(Action<GoogleUser, string> OnLoginComplete);

        void Logout();
    }
}
