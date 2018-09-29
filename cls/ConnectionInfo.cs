using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SuperSarar.cls
{
    public class ConnectionInfo
    {

        private string DataSource1 = ".";

        public string DataSource
        {
            get { return DataSource1; }
            set { DataSource1 = value; }
        }
        private string DataBaseName1 = "SPSARAR";

        public string DataBaseName
        {
            get { return DataBaseName1; }
            set { DataBaseName1 = value; }
        }
        private string UserName1 = "sa";

        public string UserName
        {
            get { return UserName1; }
            set { UserName1 = value; }
        }

       
        private string Password1 = "3217460";

        public string Password
        {
            get { return Password1; }
            set { Password1 = value; }
        }

    }
}
