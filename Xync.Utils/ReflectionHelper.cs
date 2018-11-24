using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public static class ReflectionHelper
    {
        public static object GetNestedValue(this object obj, string key)
        {
            if (obj == null)
            {
                return null;
            }

            string[] keys = key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (keys.Length == 1)
            {
                return obj.GetType().GetProperty(keys[0]).GetValue(obj);
            }
            Type type = obj.GetType();
            foreach (string k in keys)
            {
                PropertyInfo info = type.GetProperty(k);
                if (info == null)
                {
                    return null;
                }
                obj = info.GetValue(obj);
            }
            return obj;
        }
        public static Type GetNestedType(this object obj, string key)
        {
            if (obj == null)
            {
                return null;
            }
            string[] keys = key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = obj.GetType();
            foreach (string k in keys)
            {
                PropertyInfo info = type.GetProperty(k);
                if (info == null)
                {
                    return null;
                }
                type = info.PropertyType;
            }
            return type;
        }
        public static PropertyInfo GetNestedProperty(this object obj, string key)
        {
            if (obj == null)
            {
                return null;
            }
            string[] keys = key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = obj.GetType();
            PropertyInfo info = null;
            foreach (string k in keys)
            {

                info = type.GetProperty(k);
                type = info.PropertyType;
            }
            return info;
        }
        public static void SetNestedValue(this object obj, string key, object value)
        {
            if (obj == null)
            {
                return;
            }
            string[] keys = key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            Type type = obj.GetType();
            PropertyInfo info = null;

            for (int i = 0; i < keys.Length; i++)
            {
                string k = keys[i];
                info = type.GetProperty(k);
                type = info.PropertyType;
                //obj=department
                if (i == keys.Length - 1)
                {
                    info.SetValue(obj, value);
                    return;

                }
                else
                {
                    object currentValue = info.GetValue(obj);
                    info.SetValue(obj, currentValue);
                    obj = currentValue;
                }
            }

        }
    }
}
