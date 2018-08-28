using System;

namespace TestWebJob.Utils
{
    public class ServiceLocator
    {
        private static IServiceProvider _provider;

        public static IServiceProvider Provider
        {
            set
            {
                if (_provider != default(IServiceProvider))
                    throw new Exception("It can be initialized once");
                _provider = value;
            }
        }

        public static T GetService<T>() where T : class
        {
            var result = (T) _provider.GetService(typeof(T));
            return result;
        }
    }
}