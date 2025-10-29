using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Info");
    }

}
