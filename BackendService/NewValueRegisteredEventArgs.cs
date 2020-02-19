using System;

namespace BackendService
{
    public class NewValueRegisteredEventArgs : EventArgs
    {
        public int Id;
        public int Value;
        public DateTime RegistrationTime;

        public NewValueRegisteredEventArgs(int id, int value, DateTime registrationTime)
        {
            Id = id;
            Value = value;
            RegistrationTime = registrationTime;
        }
    }
}
