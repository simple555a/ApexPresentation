using System;
using System.Collections.Generic;
using System.Text;

namespace ApexPresentation
{
    public class Settings
    {
        public String SQLConnectionString;
        public bool SQLInitialized;
        public bool SQLWindowsAuthorization;
        public String SQLLogin;
        public String SQLPassword;


        public bool OPCInitialized;
        public String OPCConnectionString;
        public String OPCGroupName;
        public String OPCRingsCounterName;

    }
}
