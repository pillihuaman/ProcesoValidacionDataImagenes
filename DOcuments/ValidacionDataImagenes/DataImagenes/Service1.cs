using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using DataImagenes;


namespace DataImagenes
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            //   System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Launch();
            timer = new Timer();
            this.timer.Interval = 240000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.mailSend);
            timer.Enabled = true;
            //string path11 = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //string InitialPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Entrada\\";
            //string FinalPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Procesado\\";
            //string Procesarrespuesta = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Salida\\";

            //Procesos.DowloadFilesUnique(Procesarrespuesta);

            ////string IdValidarCliente="757";

            //Procesos.GenerarReporteXSLClientesValidos(IdValidarCliente);
        }

        protected override void OnStop()
        {
        }

        private void mailSend(object sender, ElapsedEventArgs g)
        {



            string path11 = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string InitialPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Entrada\\";
            string FinalPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Procesado\\";
            string Procesarrespuesta = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Salida\\";

            Procesos.Procesar();
            Procesos.MoveFiletoOtherPath(InitialPath, FinalPath);
            Procesos.DowloadFilesUnique(Procesarrespuesta);

            Procesos.EnviarMailing();





        }
    }
}
