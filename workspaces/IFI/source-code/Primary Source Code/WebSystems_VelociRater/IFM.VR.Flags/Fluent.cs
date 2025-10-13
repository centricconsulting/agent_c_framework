using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFM.ControlFlags
{
    public static class Fluent
    {
        public static ElseResult<T> When<T>(this T flagObj, Func<T, bool> eval) where T:IFeatureFlag
        {
            ElseResult<T> retval;
         
            if (flagObj != null && eval(flagObj))
            {
                retval = new TrueResult<T>(flagObj);
            }
            else
            {
                retval = new ElseResult<T>(flagObj);
            }

            return retval;
        }

        public static ElseResult<T> Do<T>(this ElseResult<T> resultObj, Action doThis) where T : IFeatureFlag
        {
            if(resultObj is TrueResult<T>)
            {
                doThis();
            }

            return resultObj;
        }

        public static void Else<T>(this ElseResult<T> resultObj, Action doThis) where T : IFeatureFlag
        {
            if (!(resultObj is TrueResult<T>))
            {
                doThis();
            }
        }

        public static T With<T>(this IEnumerable<IFeatureFlag> flags) where T:class, IFeatureFlag 
        {
            return flags?.FirstOrDefault(flagObj => flagObj is T) as T;
        }
        public static T WithFlags<T>(this IEnumerable<IFeatureFlag> flags) where T : class, IFeatureFlag
        {
            return flags.With<T>();
        }
    }
    
    public class ElseResult<FF>  where FF : IFeatureFlag
    {
        protected FF _flagObj;
        protected internal ElseResult(FF flagObj)
        {
            this._flagObj = flagObj;
        }
    }
    public class TrueResult<FF> : ElseResult<FF> where FF : IFeatureFlag
    {
        protected internal TrueResult(FF flagObj) : base(flagObj) { }
    }
}
