using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Utility
{
    public class JSonUtility
    {
        public static string ConvertObjectTOJSon<T>(T obj)
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, obj);
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return jsonString;
            }
            catch
            {
                throw;
            }
        }

        public static T ConvertJSonToObject<T>(string jsonString)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                T obj = (T)serializer.ReadObject(ms);
                return obj;
            }
            catch
            {
                throw;
            }
        }
    }

    public class RetornarCampos : DefaultContractResolver
    {
        private readonly string[] _campos;

        public RetornarCampos(string[] campos)
        {
            _campos = campos;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> propriedades = base.CreateProperties(type, memberSerialization);

            var retorno = propriedades.Where(p => _campos.Contains(p.PropertyName)).ToList();

            return retorno;
        }
    }
}
