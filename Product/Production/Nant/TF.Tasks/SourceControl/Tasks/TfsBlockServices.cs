using System;
using System.Collections.Generic;

namespace TF.Tasks.SourceControl.Tasks
{

    public class TfsBlockServices : IServiceProvider
    {

        #region Factory Method

        private static IServiceProvider _Self;
        private static IServiceProvider Self
        {
            get
            {
                if (_Self == null)
                    _Self = new TfsBlockServices();
                return _Self;
            }
            set
            {
                _Self = value;
            }
        }

        public static IServiceProvider GetInstance()
        {
            return Self;
        }

        #endregion

        #region Fields

        private List<Object> _Services;
        private Type _RequestedService;

        #endregion

        #region Properties

        private Type RequestedService
        {
            get
            {
                return _RequestedService;
            }
            set
            {
                _RequestedService = value;
            }
        }

        private List<Object> Services
        {
            get
            {
                if (_Services == null)
                    _Services = new List<object>();
                return _Services;
            }
            set
            {
                _Services = value;
            }
        }

        #endregion

        #region Methods

        public void Clear()
        {
            this.Services.Clear();
        }

        public void AddService(Object service)
        {
            this.Services.Add(service);
        }

        public object GetService(Type serviceType)
        {
            this.RequestedService = serviceType;
            return this.Services.Find(this.SearchForService);
        }

        public bool SearchForService(Object service)
        {
            if (this.RequestedService.IsInstanceOfType(service))
                return true;
            return false;
        }

        #endregion

    }

}
