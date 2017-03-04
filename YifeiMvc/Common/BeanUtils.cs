using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toeye.Web.Common
{
    public class BeanUtils
    {
        /// <summary>
        /// 把源对象里的各个字段的内容直接赋值给目标对象（只是字段复制，两个对象的字段名和类型都必须一致）
        /// </summary>
        /// <param name="dest">目标对象</param>
        /// <param name="src">源对象</param>
        public static void CopyObject(object dest, object src)
        {
            if (null == src) { throw new ArgumentNullException("src can't be null"); }
            if (null == dest) { throw new ArgumentNullException("dest can't be null"); }

            Type srcType = src.GetType();
            Type destType = dest.GetType();

            PropertyInfo[] srcInfo = srcType.GetProperties();
            PropertyInfo[] destInfo = destType.GetProperties();

            for (int i = 0; i < srcInfo.Length; i++)
            {
                string name = srcInfo[i].Name;
                object val = srcInfo[i].GetValue(src);

                for (int j = 0; j < destInfo.Length; j++)
                {
                    string targetName = destInfo[j].Name;

                    if (name.Equals(targetName))
                    {
                        destInfo[j].SetValue(dest, val);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 从一个对象里复制属性到另一个对象的同名属性
        /// </summary>
        /// <param name="dest">目标对象</param>
        /// <param name="src">源对象</param>
        /// <param name="fieldName">要复制的属性名字</param>
        public static void CopyProperty(ref object dest, object src, string fieldName)
        {
            if (null == src || null == dest || null == fieldName)
            {
                throw new ArgumentNullException("one argument is null!");
            }
            Type srcType = src.GetType();
            Type destType = dest.GetType();
            PropertyInfo srcInfo = srcType.GetProperty(fieldName);
            PropertyInfo destInfo = destType.GetProperty(fieldName);
            object val = srcInfo.GetValue(src);
            destInfo.SetValue(dest, val);
        }


        /// <summary>
        /// 用于设置对象的属性值
        /// </summary>
        /// <param name="dest">目标对象</param>
        /// <param name="fieldName">属性名字</param>
        /// <param name="value">属性里要设置的值</param>
        public static void setProperty(ref object dest, string fieldName, object value)
        {
            if (null == dest || null == fieldName || null == value)
            {
                throw new ArgumentNullException("one argument is null!");
            }
            Type t = dest.GetType();
            FieldInfo f = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            f.SetValue(dest, value);
        }


        /// <summary>
        /// 反射打印出对象有的方法以及调用参数
        /// </summary>
        /// <param name="obj">传入的对象</param>
        public static void printMethod(ref object obj)
        {
            Type t = obj.GetType();
            MethodInfo[] mif = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < mif.Length; i++)
            {
                Trace.WriteLine("方法名字：  {0} ", mif[i].Name);
                Trace.WriteLine("方法来自：  {0}", mif[i].Module.Name);
                ParameterInfo[] p = mif[i].GetParameters();
                for (int j = 0; j < p.Length; j++)
                {
                    Trace.WriteLine("参数名:  " + p[j].Name);
                    Trace.WriteLine("参数类型:  " + p[j].ParameterType.ToString());
                }
                Trace.WriteLine("******************************************");
            }
        }


        public static string DataTable2Json(System.Data.DataTable dt, int total = 0)
        {
            System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
            jsonBuilder.Append("{");
            if (total == 0)
            {
                jsonBuilder.AppendFormat("\"total\":{0}, ", dt.Rows.Count);
            }
            else
            {
                jsonBuilder.AppendFormat("\"total\":{0}, ", total);
            }

            jsonBuilder.Append("\"rows\":[ ");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

    }
}
