using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Security;
using Magicodes.Admin.Core.Runtime;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Magicodes.Admin.Core.DataStorage
{
    /// <summary>
    /// Uses Xamarin.Forms Application Properties to save data.
    /// If you need to store secure values such as password, use ISecureStorage.
    /// </summary>
    public class DataStorageManager : ISingletonDependency, IDataStorageManager
    {
        private static void StorePrimitive(string key, object value)
        {
            Application.Current.Properties[key] = value;
        }

        private static void StoreObject(string key, object value)
        {
            Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
        }

        private T GetPrimitive<T>(string key, T defaultValue = default(T))
        {
            if (!HasKey(key))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(Application.Current.Properties[key], typeof(T));
        }

        private T RetrieveObject<T>(string key, T defaultValue = default(T))
        {
            return !HasKey(key) ?
                defaultValue :
                JsonConvert.DeserializeObject<T>(Convert.ToString(Application.Current.Properties[key]));
        }

        public bool HasKey(string key)
        {
            return Application.Current.Properties.ContainsKey(key);
        }

        public T Retrieve<T>(string key, T defaultValue = default(T), bool shouldDecrpyt = false)
        {
            var value = TypeHelperExtended.IsPrimitive(typeof(T), false) ?
                GetPrimitive(key, defaultValue) :
                RetrieveObject(key, defaultValue);

            if (!shouldDecrpyt)
            {
                return value;
            }

            var decrypted = SimpleStringCipher.Instance.Decrypt(Convert.ToString(value));
            return (T)Convert.ChangeType(decrypted, typeof(T));
        }

        public async Task StoreAsync<T>(string key, T value, bool shouldEncrypt = false)
        {
            if (TypeHelperExtended.IsPrimitive(typeof(T), false))
            {
                if (shouldEncrypt)
                {
                    StorePrimitive(key, SimpleStringCipher.Instance.Encrypt(Convert.ToString(value)));
                }
                else
                {
                    StorePrimitive(key, value);
                }
            }
            else
            {
                StoreObject(key, value);
            }

            await Application.Current.SavePropertiesAsync();
        }

        public void RemoveIfExists(string key)
        {
            if (HasKey(key))
            {
                Application.Current.Properties.Remove(key);
            }
        }
    }
}