﻿using System;
using System.ServiceProcess;
using System.Threading;
using Esp.Tools.OpenVPN.SharedUI;

namespace Esp.Tools.OpenVPN.Configuration.UI.ViewModel
{
    public class RestartServiceViewModel : ViewModelBase
    {
        private readonly IViewModelDialogs _dialogs;

        private string _errorText;

        private string _operationText;
        private readonly Thread _thread;

        public RestartServiceViewModel(IViewModelDialogs pDialogs)
        {
            _dialogs = pDialogs;
            _thread = new Thread(Run);
            _thread.Start();
        }

        public string ErrorText
        {
            get => _errorText;
            private set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        public string OperationText
        {
            get => _operationText;
            set
            {
                _operationText = value;
                Dispatch(() => OnPropertyChanged("OperationText"));
            }
        }

        private void Run()
        {
            OperationText = "Finding Service.....";

            var serviceController = new ServiceController(Configuration.Current.ServiceName);


            if (serviceController.CanStop)
            {
                OperationText = "Stopping Service.....";
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                OperationText = "Starting Service.....";
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
                OperationText = "Done";
                if (Completed != null)
                    Dispatch(() => Completed());
            }
            else
            {
                OperationText = "Cannot stop service.";
                Dispatch(() => _dialogs.ShowError("Cannot stop service"));
            }
        }

        public event Action Completed;
    }
}