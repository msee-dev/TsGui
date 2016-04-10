﻿using System;

namespace TsGui
{
    public class SccmConnector: ITsVariableOutput
    {
        dynamic objTSProgUI;
        dynamic objTSEnv;
        //bool hidden;

        public SccmConnector()
        {
            //hidden = false;
            objTSEnv = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TSEnvironment"));
            objTSProgUI = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.SMS.TsProgressUI"));
        }

        public void AddVariable(TsVariable Variable)
        {
            objTSEnv.Value[Variable.Name] = Variable.Value;
        }

        public void Hide()
        {           
            objTSProgUI.CloseProgressDialog();
            //this.hidden = true;
        }

        public void Release()
        {
            //if (this.hidden == false)
            //{
                // Release the comm objects.
                if (System.Runtime.InteropServices.Marshal.IsComObject(this.objTSProgUI) == true)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.objTSProgUI);
                }

                if (System.Runtime.InteropServices.Marshal.IsComObject(this.objTSEnv) == true)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(this.objTSEnv);
                }
            //}
        }

        public string GetVariable(string Variable)
        {
            return objTSEnv.Value[Variable];
        }
    }
}
