using System;
using System.Collections.Generic;
using System.Web;

namespace IFM.DataServices.API.ResponseObjects.Common
{

    [System.Serializable]
    public class ServiceResult :  BaseResult
    {
        public object ResponseData { get; set; }
    }

    [System.Serializable]
    public class ServiceResult<T> : BaseResult
    {
        private T _responseData;
        public T ResponseData { 
            get 
            {
                if (_responseData == null)
                {
                    if (typeof(T).GetConstructor(Type.EmptyTypes) == null)
                    {
                        _responseData = default(T);
                    }
                    else
                    {
                        _responseData = (T)Activator.CreateInstance(typeof(T));
                    }
                }
                return _responseData;
            }
            set
            {
                _responseData = value;
            }
        }
    }
}