using System;
using System.Collections.Generic;
using System.Data;

namespace _3M.Comodato.Business
{
    public abstract class BusinessBase<T>
    {
        public abstract T MontarObjeto(DataTableReader reader);


        internal virtual List<T> MontarLista(DataTableReader reader)
        {
            var lista = new List<T>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lista.Add(MontarObjeto(reader));
                }
            }

            if (reader != null)
            {
                reader.Dispose();
            }

            return lista;
        }

        public virtual T ObterPorCodigo(object codigo)
        {
            throw new NotImplementedException();
        }

        public virtual List<T> Listar(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
