using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailServices;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using WinSCP;
using System.Net.Mail;

//using System.Web.Mvc;
namespace DataImagenes
{
     class Procesos
    {


        public static void Procesar()
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {

                string Nombrearchivo = string.Empty;

                insertClientToBDSoporteI();
                Nombrearchivo = GreateCSV_ValidarClientes().ToString();

                if (Nombrearchivo != "NO ESTA DENTRO DEL PASE")
                {
                    Mail ObjMail = new Mail();
                    ObjMail._NetworkCredencialUser = ConfigurationManager.AppSettings["NetworkCredencialUser"];
                    ObjMail._NetworkCredencialPass = ConfigurationManager.AppSettings["NetworkCredencialPass"];
                    ObjMail._CorreoGmailOrigen = "pillihuamanhz@gmail.com";
                    ObjMail._NombreGmailOrigen = "Proceso de Validacion  con DataImagenes";
                    ObjMail._ListaCorreos = new Queue<string>();
                    ObjMail._ListaCorreos.Enqueue("zarmir.pillihuaman@orbis.com.pe");
                    ObjMail._ListaCorreos.Enqueue("pillihuamanhz@gmail.com");
                    ObjMail._ListaCorreos.Enqueue("universo_infinito88@hotmail.com");
                    ObjMail._Subject = "Proceso de Validacion con DataImagenes  de la fecha  " + DateTime.Now.ToString();
                    ObjMail._BodyHtml = "<html><body><h5>" + "Proceso de Validacion de clientes con Dataimagenes con fecha  " + DateTime.Now.ToString() + "</h5> <br> Nombre del archivo ingresado al SFTP Dataimagenes   " + Nombrearchivo + "  <br>Saludos" + "</body></html>";
                    //ObjMail.File_Path_ToAtacch = R + ".txt";
                    ObjMail._DisponibleHTMLBODy = true;
                    ObjMail._Puerto = ConfigurationManager.AppSettings["Puerto"];
                    MailOperaciones.SenMailNoAttachments(ObjMail);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public static string insertClientToBDSoporteI()
        {
            string Mensaje = string.Empty;
            string NombreArchivo = string.Empty;


            try
            {

                SqlDataReader dr = null;



                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_InsertClientedePEV", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        dr = cmd.ExecuteReader();

                        //while (dr.Read())
                        //{

                        //    //Mensaje = dr["FLUJO"].ToString();
                        //    if (Mensaje != "NO ESTA DENTRO DEL PASE")
                        //    {
                        //   NombreArchivo=GreateCSV_ValidarClientes(Mensaje);

                        //    }
                        //}


                    }


                    dr.Close();

                }



            }

            catch (Exception ex)
            {
                throw ex;


            }
            return NombreArchivo;
        }
        public static string GreateCSV_ValidarClientes()
        {

            string mensaje = string.Empty;
            string NombreArchivo = string.Empty;
            Queue<string> ListarResultadoDIInsert = new Queue<string>();

            try
            {
                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_GetLastClientesValidarDI", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            NombreArchivo = dr["NOMBRE_ARCHIVO"].ToString();

                            ListarResultadoDIInsert.Enqueue(dr["REPORTE_DATAIMAGENES"].ToString());
                        }


                    }


                    dr.Close();

                }


                if (ListarResultadoDIInsert.Count > 1)
                {

                    string path11 = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    using (StreamWriter sw = File.CreateText(path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Entrada\\" + NombreArchivo))
                    {

                        foreach (string s in ListarResultadoDIInsert)
                        {
                            if (s != null)
                            {
                                sw.WriteLine(s);

                            }

                        }
                        sw.Close();


                    }

                    // enviar Informacion A SFTP  imagesn
                    //SaveFileValidaDataImagenes();
                    ////////////
                    string PathFTPServerSalida = ConfigurationManager.AppSettings["PathFTPServerSalida"];
                    string PathFTPServerEntrada = ConfigurationManager.AppSettings["PathFTPServerEntrada"];
                    //string LocalPathTosaveFiles = ConfigurationManager.AppSettings["LocalPathTosaveFiles"];
                    string LocalPathTosaveFiles = ConfigurationManager.AppSettings["LocalPathTosaveFiles"];
                    string LocalPathToPutFiles = ConfigurationManager.AppSettings["LocalPathToPutFiles"];
                    string PortNumber = ConfigurationManager.AppSettings["PortNumber"];
                    string PassworDataImagenes = ConfigurationManager.AppSettings["PassworDataImagenes"];
                    string UserNameDataImagenes = ConfigurationManager.AppSettings["UserNameDataImagenes"];
                    string HostNameDataImagenes = ConfigurationManager.AppSettings["HostNameDataImagenes"];
                    string SshHostKeyFingerprintDataImagenes = ConfigurationManager.AppSettings["SshHostKeyFingerprintDataImagenes"];
                    string SshPrivateKeyPathDataImagenesConfiguration = ConfigurationManager.AppSettings["SshPrivateKeyPathDataImagenesConfiguration"];
                    string SshPrivateKeyPassphraseDataImages = ConfigurationManager.AppSettings["SshPrivateKeyPassphraseDataImages"];
                    string LocalPathToPutFiles1 = string.Empty;
                    foreach (string s in GetAllFilesFromLocalPath(LocalPathToPutFiles))
                    {
                        LocalPathToPutFiles1 = s; ;
                        SaveFileValidaDataImagenes(LocalPathToPutFiles1, PathFTPServerEntrada, PortNumber, PassworDataImagenes, UserNameDataImagenes, HostNameDataImagenes, SshHostKeyFingerprintDataImagenes, SshPrivateKeyPathDataImagenesConfiguration, SshPrivateKeyPassphraseDataImages);
                    }

                    //= GetAllFilesFromLocalPath(LocalPathToPutFiles);

                    //string FinalPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Salida\\";

                    //GetAllFilesSFTP(PathFTPServerSalida, FinalPath, PortNumber, PassworDataImagenes, UserNameDataImagenes, HostNameDataImagenes, SshHostKeyFingerprintDataImagenes, SshPrivateKeyPathDataImagenesConfiguration, SshPrivateKeyPassphraseDataImages);

                    ///////




                }




            }

            catch (Exception ex)
            {

                throw ex;
            }

            return NombreArchivo;

        }

        public static string EnviarMailing()
        {
            //System.Diagnostics.Debugger.Launch();


            try
            {
                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBDLocal"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_EnviarMailing", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        dr = cmd.ExecuteReader();

                        //while (dr.Read())
                        ////{
                        ////    NombreArchivo = dr["NOMBRE_ARCHIVO"].ToString();

                        ////    ListarResultadoDIInsert.Enqueue(dr["REPORTE_DATAIMAGENES"].ToString());
                        //}


                    }


                    dr.Close();

                }

            }

            catch (Exception ex)
            {

                throw ex;
            }

            return "";

        }

        public static void GetAllFilesSFTP(string PathFTPServerSalida, string LocalPathTosaveFiles, string PortNumber, string PassworDataImagenes, string UserNameDataImagenes, string HostNameDataImagenes, string SshHostKeyFingerprintDataImagenes, string SshPrivateKeyPathDataImagenesConfiguration, string SshPrivateKeyPassphraseDataImages)
        {
            try
            {

                SessionOptions Sessionobj = new SessionOptions();
                Sessionobj.Protocol = Protocol.Sftp;
                Sessionobj.PortNumber = int.Parse(PortNumber);
                Sessionobj.Password = PassworDataImagenes;
                Sessionobj.UserName = UserNameDataImagenes;
                Sessionobj.HostName = HostNameDataImagenes;
                Sessionobj.SshHostKeyFingerprint = SshHostKeyFingerprintDataImagenes;
                Sessionobj.SshPrivateKeyPath = SshPrivateKeyPathDataImagenesConfiguration;
                Sessionobj.SshPrivateKeyPassphrase = SshPrivateKeyPassphraseDataImages;

                using (Session sessiones = new Session())
                {
                    sessiones.AddRawConfiguration("AgentFwd", "1");
                    sessiones.Open(Sessionobj);

                    TransferOptions OpDescargas = new TransferOptions();
                    OpDescargas.TransferMode = TransferMode.Binary;
                    TransferOperationResult ResultTranfer;
                    ResultTranfer = sessiones.GetFiles(PathFTPServerSalida, LocalPathTosaveFiles, false, OpDescargas);
                    ResultTranfer.Check();
                    foreach (TransferEventArgs Tran in ResultTranfer.Transfers)
                    {

                        Console.WriteLine("Descargado de  {0} exito ", Tran.FileName);
                    }

                };



            }
            catch (Exception ex)
            {

                throw ex;



            }
        }

        public static void SaveFileValidaDataImagenes(string LocalPathToPutFiles, string PathFTPServerEntrada, string PortNumber, string PassworDataImagenes, string UserNameDataImagenes, string HostNameDataImagenes, string SshHostKeyFingerprintDataImagenes, string SshPrivateKeyPathDataImagenesConfiguration, string SshPrivateKeyPassphraseDataImages)
        {
            //Ingresar solo registro Unicos
            string estadoDuplicado = string.Empty;

            try
            {

                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_ValidacionEnvioCSVDataImagenes", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            estadoDuplicado = dr["BOLEAN"].ToString();


                        }


                    }


                    dr.Close();

                }

                if (estadoDuplicado == "TRUE")
                { }
                else
                {
                    //Enviaara archivo a SFTP si no es envio antes 

                    SessionOptions Sessionobj = new SessionOptions();
                    Sessionobj.Protocol = Protocol.Sftp;
                    Sessionobj.PortNumber = int.Parse(PortNumber);
                    Sessionobj.Password = PassworDataImagenes;
                    Sessionobj.UserName = UserNameDataImagenes;
                    Sessionobj.HostName = HostNameDataImagenes;
                    Sessionobj.SshHostKeyFingerprint = SshHostKeyFingerprintDataImagenes;
                    Sessionobj.SshPrivateKeyPath = SshPrivateKeyPathDataImagenesConfiguration;
                    Sessionobj.SshPrivateKeyPassphrase = SshPrivateKeyPassphraseDataImages;

                    using (Session sessiones = new Session())
                    {
                        sessiones.AddRawConfiguration("AgentFwd", "1");
                        sessiones.Open(Sessionobj);

                        TransferOptions OpDescargas = new TransferOptions();
                        OpDescargas.TransferMode = TransferMode.Binary;
                        TransferOperationResult ResultTranfer;
                        ResultTranfer = sessiones.PutFiles(LocalPathToPutFiles, PathFTPServerEntrada, false, OpDescargas);
                        ResultTranfer.Check();
                        foreach (TransferEventArgs Tran in ResultTranfer.Transfers)
                        {

                            Console.WriteLine("Descargado al SFTP de  {0} exito ", Tran.FileName);
                        }

                    };

                }


            }



            catch (Exception ex)
            {

                throw ex;
            }
            ////////////////
        }

        public static Queue<string> GetAllFilesFromLocalPath(string LocalPath)
        {

            var listDirectories = Directory.GetFiles(@LocalPath);
            Queue<string> ListFiles = new Queue<string>(listDirectories);
            return ListFiles;
        }

        public static void MoveFiletoOtherPath(string InitialPath, string FinalPath)
        {
            try
            {
                Queue<string> ListFiles = new Queue<string>();


                DirectoryInfo d = new DirectoryInfo(InitialPath);
                ListFiles = GetAllFilesFromLocalPath(FinalPath);



                foreach (var file in d.GetFiles("*"))
                {
                    bool Files = false;
                    Files = File.Exists(FinalPath + file.Name);

                    if (Files == true)
                    {
                        File.Delete(InitialPath + file.Name);

                    }


                    if (Files != true)
                    {


                        Directory.Move(file.FullName, FinalPath + file.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        public static string ProcesarRespuestaDI(string LocalPath, string NombreArchivo)
        {
            //Recibir nombre del archivo y la ruta

            //System.Diagnostics.Debugger.Launch();
            string Mensaje = string.Empty;
            string SeparadordeComas = ",";
            Queue<string> ListCSVContent = new Queue<string>();
            Queue<string> GetCSVFromLocalpath = new Queue<string>();
            Queue<string> listCSVTable = new Queue<string>();
            Queue<string> listSplitComas = new Queue<string>();
            String NombreArchivoEnvia = string.Empty;



            GetCSVFromLocalpath = GetAllFilesFromLocalPath(LocalPath);



            var arra = new List<string>(GetCSVFromLocalpath);
            var arraNewList = new List<string>();
            string Salida = arra.Find(x => x.Contains(NombreArchivo));



            foreach (string s in GetCSVFromLocalpath)
            {
                NombreArchivoEnvia = s.Substring(s.Length - 45, 45);

                //if (NombreArchivoEnvia==)
                if (s.Contains(Salida))
                {


                    using (StreamReader read = new StreamReader(s, System.Text.UTF8Encoding.Default))
                    {




                        while (!read.EndOfStream)
                        {
                            string ReplaceComilla = read.ReadLine().ToString().Replace("'", "");
                            ListCSVContent.Enqueue(ReplaceComilla);





                        }

                        ListCSVContent.Peek();
                        Queue<string> Spliopcio = new Queue<string>(); ;

                        foreach (string input in ListCSVContent)
                        {
                            string Cadena = string.Empty;


                            if (input.Contains("ApellidoPaterno") || input.Contains("DNI"))
                            { }
                            else
                            {
                                int contador = 0;

                                string[] result = input.Split(new string[] { "," }, StringSplitOptions.None);
                                foreach (string ss in result)
                                {


                                    if (contador >= 3 && contador <= 5 || contador == 7)
                                    { }
                                    //else if (contador == 7)
                                    //{ }
                                    else
                                    {

                                        if (ss == "")
                                        {

                                            Cadena = Cadena + "null" + ",";
                                        }
                                        else
                                        {
                                            //Spliopcio.Enqueue(ss);
                                            Cadena = Cadena + ss + ",";
                                        }



                                    }
                                    contador++;
                                }

                                Spliopcio.Enqueue(Cadena.Substring(0, Cadena.Length - 1));

                            }

                        }

                        String CadenaBase = "INSERT INTO @Tabla (DNI_Existe, Nombre_Existe, FecEmision_Existe, DNI, Nombre, ApeP, APeM, FechaEmisionReniec, fechaNacimiento, sexo, gradoInstruccion, departamentoNacimiento, provinciaNacimiento, distritoNacimiento, nombrePadre, nombreMadre, estatura, estadoCivil, fechaInscripcion, domicilio, departamentoDomicilio, provinciaDomicilio, distritoDomicilio) VALUES (";
                        //Creacion de cadena con la respuesta
                        foreach (string ss in Spliopcio)
                        {
                            string FirstReplace = string.Empty;
                            string SecondReplace = string.Empty;
                            FirstReplace = ss.Replace(",", "','").ToString() + "');   ";
                            SecondReplace = FirstReplace.Replace("'null'", "null").ToString();


                            listCSVTable.Enqueue(CadenaBase + "'" + SecondReplace);

                        }



                    }
                }
            }

            //Queue<string> ListFiles = new Queue<string>(); 
            //ListFiles=  GetAllFilesFromLocalPath(LocalPath);
            if (listCSVTable.Count > 0)
            {
                foreach (string s in listCSVTable)
                {
                    Mensaje = Mensaje + s;
                }
                insertClienteValidado(NombreArchivoEnvia, Mensaje);
            }
            return Mensaje;

        }

        public static string insertClienteValidado(string NombreArchivo, string CadenaValidacion)
        {








            string Mensaje = string.Empty;
            string Cadena = "Declare @nombreArchivoRespuesta varchar(60)='" + NombreArchivo + "';" +


"declare @Tabla as table " +
"( " +
   "id int identity, " +
   "DNI_Existe	char(2) NULL, " +
   "Nombre_Existe varchar(2) NULL, " +
   "FecEmision_Existe varchar(2) NULL, " +
   "DNI char(8) NULL, " +
   "Nombre varchar(50) NULL, " +
   "ApeP varchar(50) NULL, " +
   "APeM varchar(50) NULL, " +
   "FechaEmisionReniec date NULL,  " +
   "fechaNacimiento date NULL, " +
   "sexo varchar(9) NULL, " +
   "gradoInstruccion  varchar(200) NULL, " +
   "departamentoNacimiento   varchar(100) NULL,  " +
   "provinciaNacimiento      varchar(100) NULL, " +
   "distritoNacimiento       varchar(100) NULL, " +
   "nombrePadre              varchar(100) NULL, " +
   "nombreMadre              varchar(100) NULL, " +
   "estatura                 integer NULL, " +
   "estadoCivil              varchar(25) NULL, " +
   "fechaInscripcion         date NULL, " +
   "domicilio                varchar(250) NULL, " +
   "departamentoDomicilio    varchar(100) NULL, " +
   "provinciaDomicilio       varchar(100) NULL, " +
   "distritoDomicilio        varchar(250) NULL " +

") " +

CadenaValidacion +


   "DECLARE @Indice AS INT = 1,  " +
           "@Fin AS INT = (SELECT MAX(ID) FROM @Tabla),  " +
           "@idValidarCliente int,@cantidadRegistros int,@Estado int;  " +

   "set @idValidarCliente=(SELECT distinct idValidarCliente FROM DetValidarCliente where DNI in (SELECT DNI FROM @Tabla))  " +
   "(select @cantidadRegistros=CantRegistro,@Estado=Estado from ValidarCliente where idValidarCliente=@idValidarCliente )  " +

"select @idValidarCliente Idvalidacioncliente,@Fin QRegistrosInsertar,@cantidadRegistros cantidadRegistrosBD , @nombreArchivoRespuesta NombreArchivoRespuesta,@Estado Estado  " +
"Print @Fin  " +
"Print @cantidadRegistros  " +
"Print @Estado  " +
"IF(@Fin=@cantidadRegistros and @Estado=0)  " +
   "begin   " +
   "If(@idValidarCliente is not null)  " +
       "begin  " +

           "WHILE(@Indice <= @Fin)  " +
           "begin  " +
               "Declare   " +
               "@DNI_Existe	int,  " +
               "@Nombre_Existe int,  " +
               "@FecEmision_Existe int,  " +
               "@DNI char(8),  " +
               "@Nombre varchar(50),  " +
               "@ApeP varchar(50),  " +
               "@APeM varchar(50),  " +
               "@FechaEmisionReniec date,  " +
               "@fechaNacimiento date,  " +
              " @sexo varchar(9),  " +
              " @gradoInstruccion  varchar(150),  " +
               "@departamentoNacimiento   varchar(100) ,  " +
               "@provinciaNacimiento      varchar(100) ,  " +
               "@distritoNacimiento       varchar(100) ,  " +
               "@nombrePadre              varchar(100) ,  " +
               "@nombreMadre              varchar(100) ,  " +
               "@estatura                 integer ,  " +
               "@estadoCivil              varchar(25) ,  " +
               "@fechaInscripcion         date ,  " +
               "@domicilio                varchar(250) ,  " +
               "@departamentoDomicilio    varchar(100) ,  " +
               "@provinciaDomicilio       varchar(100) ,  " +
               "@distritoDomicilio        varchar(250)   " +

               "select   " +
               "@DNI_Existe=(Case when DNI_Existe='SI'then 1 else 0 end) ,   " +
               "@Nombre_Existe=(Case when Nombre_Existe='SI'then 1 else 0 end),   " +
               "@FecEmision_Existe=(Case when FecEmision_Existe='SI'then 1 else 0 end),   " +
               "@DNI=DNI,@Nombre=Nombre,@ApeP=ApeP,   " +
               "@APeM=APeM ,@FechaEmisionReniec=FechaEmisionReniec,@fechaNacimiento=fechaNacimiento,   " +
               "@sexo=sexo,@gradoInstruccion=gradoInstruccion,   " +
               "@departamentoNacimiento = departamentoNacimiento  ,   " +
               "@provinciaNacimiento    = provinciaNacimiento     ,   " +
               "@distritoNacimiento     = distritoNacimiento      ,   " +
               "@nombrePadre            = nombrePadre             ,   " +
               "@nombreMadre            = nombreMadre             ,   " +
               "@estatura               = estatura                ,   " +
               "@estadoCivil            = estadoCivil             ,   " +
               "@fechaInscripcion       = fechaInscripcion        ,   " +
               "@domicilio              = domicilio               ,   " +
               "@departamentoDomicilio  = departamentoDomicilio   ,   " +
               "@provinciaDomicilio     = provinciaDomicilio      ,   " +
               "@distritoDomicilio      = distritoDomicilio    " +
               "from @Tabla   " +
               "where ID=@Indice   " +


                   "update DetValidarCliente   " +
                   "set fechaModificacion=GETDATE(),    " +
                   "idUsuarioModificacion=1,   " +
                   "reniecNombre=@Nombre,   " +
                   "reniecApellidoP=@ApeP,	   " +
                   "reniecApellidoM=@APeM,   " +
                   "reniecFechaEmision= @FechaEmisionReniec,   " +
                   "fechaNacimiento   =@fechaNacimiento,   " +
                   "sexo                =cast(@sexo as char(1)),   " +
                  " gradoInstruccion   =@gradoInstruccion,   " +
                   "departamentoNacimiento = @departamentoNacimiento  ,   " +
                   "provinciaNacimiento    = @provinciaNacimiento     ,   " +
                   "distritoNacimiento     = @distritoNacimiento      ,   " +
                   "nombrePadre            = @nombrePadre             ,   " +
                   "nombreMadre            = @nombreMadre             ,   " +
                   "estatura               = @estatura                ,   " +
                   "estadoCivil            = @estadoCivil             ,   " +
                   "fechaInscripcion       = @fechaInscripcion        ,   " +
                   "domicilio              = @domicilio               ,   " +
                   "departamentoDomicilio  = @departamentoDomicilio   ,   " +
                   "provinciaDomicilio     = @provinciaDomicilio      ,   " +
                   "distritoDomicilio      = @distritoDomicilio       ,   " +
                   "validaDNI=@DNI_Existe,    " +
                   "validaNombres=@Nombre_Existe,    " +
                   "validaFechaEmision=    " +
                   "case when LTRIM(RTRIM(FechaEmision)) = right('00'+cast(MONTH(@FechaEmisionReniec)as varchar),2)+'/'+right('0000'+cast(YEAR(@FechaEmisionReniec)as varchar),4) then 1 else 0 end,    " +
                   "Estado=(CASE WHEN @Nombre_Existe=1 AND @DNI_Existe=1 and LTRIM(RTRIM(FechaEmision)) = right('00'+cast(MONTH(@FechaEmisionReniec)as varchar),2)+'/'+right('0000'+cast(YEAR(@FechaEmisionReniec)as varchar),4) THEN 3 ELSE 4 END),   " +
                   "idTipoValidacion=3   " +
                   "WHERE DNI=@DNI   " +


                   "set @Indice=@Indice+1   " +
               "end   " +

               "Update ValidarCliente    " +
               "set nombreArchivoRespuesta=@nombreArchivoRespuesta, Estado=1, fechaModificacion=GETDATE(),idUsuarioModificacion=1   " +
               "where idValidarCliente=@idValidarCliente and nombreArchivoRespuesta =''   " +

       "end   " +
       "select COUNT(idCliente) from DetValidarCliente where idValidarCliente=@idValidarCliente   " +
        "and  validaDNI is not null   " +
   "end";


            try
            {


                int CantidadValidacionII = 0;
                int CantidadValidacionI = 0;
                int CantidadValidacionIII = 0;
                //System.Diagnostics.Debugger.Launch();

                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_Get_ClientesValidados", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //System.Diagnostics.Debugger.Launch();
                        //cmd.Parameters.AddWithValue("@NOMBREARCHIVO", NombreArchivo);
                        cmd.Parameters.AddWithValue("@CADENA", Cadena);
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {

                            //Procesos.GenerarReporteXSLClientesValidos(6);
                            GenerarReporteXSLClientesValidos(dr["Idvalidacioncliente"].ToString());


                        }


                    }


                    dr.Close();

                }
                string path11 = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string InitialPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Entrada\\";
                string FinalPath = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Procesado\\";
                string Procesarrespuesta = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\Salida\\";
                MoveFiletoOtherPath(Procesarrespuesta, FinalPath);

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Mensaje;
        }

        public static string DowloadFilesUnique(string PathFinalProccessFile)
        {
            string NombreArchivo = string.Empty;
            Queue<string> ListaNombrearchivos = new Queue<string>();
            Queue<string> ListLocalFiles = new Queue<string>();
            Queue<string> ListLocalFilesToProccess = new Queue<string>();
            String FiLeNameDontProccess = string.Empty;
            // enviar Informacion A SFTP  imagesn
            //SaveFileValidaDataImagenes();
            ////////////
            string PathFTPServerSalida = ConfigurationManager.AppSettings["PathFTPServerSalida"];
            string PathFTPServerEntrada = ConfigurationManager.AppSettings["PathFTPServerEntrada"];
            //string LocalPathTosaveFiles = ConfigurationManager.AppSettings["LocalPathTosaveFiles"];
            string LocalPathTosaveFiles = ConfigurationManager.AppSettings["LocalPathTosaveFiles"];
            string LocalPathToPutFiles = ConfigurationManager.AppSettings["LocalPathToPutFiles"];
            string PortNumber = ConfigurationManager.AppSettings["PortNumber"];
            string PassworDataImagenes = ConfigurationManager.AppSettings["PassworDataImagenes"];
            string UserNameDataImagenes = ConfigurationManager.AppSettings["UserNameDataImagenes"];
            string HostNameDataImagenes = ConfigurationManager.AppSettings["HostNameDataImagenes"];
            string SshHostKeyFingerprintDataImagenes = ConfigurationManager.AppSettings["SshHostKeyFingerprintDataImagenes"];
            string SshPrivateKeyPathDataImagenesConfiguration = ConfigurationManager.AppSettings["SshPrivateKeyPathDataImagenesConfiguration"];
            string SshPrivateKeyPassphraseDataImages = ConfigurationManager.AppSettings["SshPrivateKeyPassphraseDataImages"];
            string LocalPathToPutFiles1 = string.Empty;


            GetAllFilesSFTP(PathFTPServerSalida, PathFinalProccessFile, PortNumber, PassworDataImagenes, UserNameDataImagenes, HostNameDataImagenes, SshHostKeyFingerprintDataImagenes, SshPrivateKeyPathDataImagenesConfiguration, SshPrivateKeyPassphraseDataImages);

            ///////


            ListLocalFiles = GetAllFilesFromLocalPath(PathFinalProccessFile);






            try
            {


                //Buscar file en BD en la Carpeta Y en La Bd
                bool StadoFileBD = false;
                bool StadoFileFolder = false;


                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();
                    string SQL = "select  top 500 	idValidarCliente,	nombreArchivoEnvio,	nombreArchivoRespuesta	,CantRegistro	,Estado from	  dbo.ValidarCliente  order by 1 desc";

                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {


                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            ListaNombrearchivos.Enqueue(dr["nombreArchivoRespuesta"].ToString());
                        }



                    }


                    dr.Close();
                }


                // Find a correct Values

                var arra = new List<string>(ListaNombrearchivos);
                var arraNewList = new List<string>();

                foreach (string ss in ListLocalFiles)
                {
                    string NombreFile = ss.Substring(ss.Length - 45, 45);
                    string Salida = string.Empty;
                    Salida = arra.Find(x => x.Contains(NombreFile));
                    if (Salida == null)
                    {
                        FiLeNameDontProccess = NombreFile;
                        Procesos.ProcesarRespuestaDI(PathFinalProccessFile, NombreFile);
                    }






                }

            }

            catch (Exception exz)
            {
                throw exz;
            }

            return FiLeNameDontProccess;

        }

        public static string GenerarReporteXSLClientesValidos(string IdValidarCliente)
        {
            DateTime HOydia = DateTime.Now;
            string Fecha = HOydia.ToString("yyyy-MM-dd  hh:mm:ss");
            string path11 = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string NombreArchivo1 = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\FinalExcel\\" + "Reporte_Wallet _Usuario_web_Validados_" + Fecha.Replace(":", "-").ToString() + ".csv";
            string NombreArchivo2 = path11.Substring(0, path11.Length - 27) + "\\HomeValidarClientes\\FinalExcel\\" + "Reporte_Wallet_Usuario_en_la_web_Validacion_Test_input_sac" + Fecha.Replace(":", "-").ToString() + ".csv";


            try
            {


                int CantidadValidacionII = 0;
                int CantidadValidacionI = 0;
                int CantidadValidacionIII = 0;
                Queue<string> ListarReporteClientesValidado1 = new Queue<string>();
                Queue<string> ListarReporteClientesValidado2 = new Queue<string>();

                SqlDataReader dr = null;
                String Conexio_String = ConfigurationManager.ConnectionStrings["cnBD"].ConnectionString;
                using (SqlConnection con = new SqlConnection(Conexio_String))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_Get_ClientesValidados_Final_XLS", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        //cmd.Parameters.AddWithValue("@NOMBREARCHIVO", NombreArchivo);
                        cmd.Parameters.AddWithValue("@IDvalidarCliente", int.Parse(IdValidarCliente));
                        dr = cmd.ExecuteReader();
                        string CadenaInicio = "idCliente" + "," + "FechaRegistro" + "," + "numeroDocumento" + "," + "fechaEmisionDocumento" + "," + "Nombres" + "," + "ApellidoPaterno" + "," + "ApellidoMaterno" + "," + "tipo_envio" + "," + "Stand" + "," + "Horario_entrega" + "," + "Direccion" + "," + "Departamento" + "," + "Provincia" + "," + "Distrito" + "," + "Referencia" + "," + "NumeroMovil" + "," + "Email" + "," + "reniecNombre" + "," + "reniecApellidoP" + "," + "reniecApellidoM" + "," + "reniecFechaEmision" + "," + "datoValidado" + "," + "motivos" + "," + "codigo_seguimiento" + "," + "estado_pedido" + "," + "comentario" + "," + "IdTipoOperador" + "," + "RecibeBoletin" + "," + "AceptaPolitica" + "," + "CodigoAfiliacion" + "," + "CodigoConfirmacionEmail" + "," + "URL" + "," + "RangoDiasEntrega" + "," + "RangoHorasEntrega" + "," + "canal" + "," + "usuarioEstado" + "," + "RecibeTarjeta" + "," + "IdEstadoCliente" + "," + "FechaRegistro" + "," + "FechaModificacion" + "," + "FechaEntrega" + "," + "mes" + "," + "dia" + "," + "hora" + "," + "IdTipoDocumento" + "," + "primer_nombre";
                        ListarReporteClientesValidado1.Enqueue(CadenaInicio);
                        while (dr.Read())
                        {

                            ListarReporteClientesValidado1.Enqueue(dr["idCliente"].ToString().Replace(",", " ") + "," + dr["FechaRegistro"].ToString().Replace(",", " ") + "," + dr["numeroDocumento"].ToString().Replace(",", " ") + "," + dr["fechaEmisionDocumento"].ToString().Replace(",", " ") + "," + dr["Nombres"].ToString().Replace(",", " ") + "," + dr["ApellidoPaterno"].ToString().Replace(",", " ") + "," + dr["ApellidoMaterno"].ToString().Replace(",", " ") + "," + dr["tipo_envio"].ToString().Replace(",", " ") + "," + dr["Stand"].ToString().Replace(",", " ") + "," + dr["Horario_entrega"].ToString().Replace(",", " ") + "," + dr["Direccion"].ToString().Replace(",", " ") + "," + dr["Departamento"].ToString().Replace(",", " ") + "," + dr["Provincia"].ToString().Replace(",", " ") + "," + dr["Distrito"].ToString().Replace(",", " ") + "," + dr["Referencia"].ToString().Replace(",", " ") + "," + dr["NumeroMovil"].ToString().Replace(",", " ") + "," + dr["Email"].ToString().Replace(",", " ") + "," + dr["reniecNombre"].ToString().Replace(",", " ") + "," + dr["reniecApellidoP"].ToString().Replace(",", " ") + "," + dr["reniecApellidoM"].ToString().Replace(",", " ") + "," + dr["reniecFechaEmision"] + "," + dr["datoValidado"].ToString().Replace(",", " ") + "," + dr["motivos"].ToString().Replace(",", " ") + "," + dr["codigo_seguimiento"] + "," + dr["estado_pedido"].ToString().Replace(",", " ") + "," + dr["comentario"].ToString().Replace(",", " ") + "," + dr["IdTipoOperador"].ToString().Replace(",", " ") + "," + dr["RecibeBoletin"].ToString().Replace(",", " ") + "," + dr["AceptaPolitica"] + "," + dr["CodigoAfiliacion"] + "," + dr["CodigoConfirmacionEmail"] + "," + dr["URL"] + "," + dr["RangoDiasEntrega"] + "," + dr["RangoHorasEntrega"].ToString().Replace(",", " ") + "," + dr["canal"].ToString().Replace(",", " ") + "," + dr["usuarioEstado"].ToString().Replace(",", " ") + "," + dr["RecibeTarjeta"].ToString().Replace(",", " ") + "," + dr["IdEstadoCliente"].ToString().Replace(",", " ") + "," + dr["FechaRegistro"].ToString().Replace(",", " ") + "," + dr["FechaModificacion"].ToString().Replace(",", " ") + "," + dr["FechaEntrega"].ToString().Replace(",", " ") + "," + dr["mes"].ToString().Replace(",", " ") + "," + dr["dia"].ToString().Replace(",", " ") + "," + dr["hora"].ToString().Replace(",", " ") + "," + dr["IdTipoDocumento"].ToString().Replace(",", " ") + "," + dr["primer_nombre"].ToString().Replace(",", " "));


                        }

                        dr.NextResult();
                        ListarReporteClientesValidado2.Enqueue("idCliente" + "," + "FechaRegistro" + "," + "numeroDocumento" + "," + "fechaEmisionDocumento" + "," + "Nombres" + "," + "ApellidoPaterno" + "," + "ApellidoMaterno" + "," + "tipo_envio" + "," + "Stand" + "," + "Horario_entrega" + "," + "Direccion" + "," + "Departamento" + "," + "Provincia" + "," + "Distrito" + "," + "Referencia" + "," + "NumeroMovil" + "," + "Email" + "," + "reniecNombre" + "," + "reniecApellidoP" + "," + "reniecApellidoM" + "," + "reniecFechaEmision" + "," + "datoValidado" + "," + "motivos");
                        while (dr.Read())
                        {

                            ListarReporteClientesValidado2.Enqueue(dr["idCliente"].ToString().Replace(",", " ").Replace(",", " ") + "," + dr["FechaRegistro"].ToString().Replace(",", " ") + "," + dr["numeroDocumento"].ToString().Replace(",", " ") + "," + dr["fechaEmisionDocumento"].ToString().Replace(",", " ") + "," + dr["Nombres"].ToString().Replace(",", " ") + "," + dr["ApellidoPaterno"].ToString().Replace(",", " ") + "," + dr["ApellidoMaterno"].ToString().Replace(",", " ") + "," + dr["tipo_envio"].ToString().Replace(",", " ") + "," + dr["Stand"].ToString().Replace(",", " ") + "," + dr["Horario_entrega"].ToString().Replace(",", " ") + "," + dr["Direccion"].ToString().Replace(",", " ") + "," + dr["Departamento"].ToString().Replace(",", " ") + "," + dr["Provincia"].ToString().Replace(",", " ") + "," + dr["Distrito"].ToString().Replace(",", " ") + "," + dr["Referencia"].ToString().Replace(",", " ") + "," + dr["NumeroMovil"].ToString().Replace(",", " ") + "," + dr["Email"].ToString().Replace(",", " ") + "," + dr["reniecNombre"].ToString().Replace(",", " ") + "," + dr["reniecApellidoP"].ToString().Replace(",", " ") + "," + dr["reniecApellidoM"].ToString().Replace(",", " ") + "," + dr["reniecFechaEmision"].ToString().Replace(",", " ") + "," + dr["datoValidado"].ToString().Replace(",", " ") + "," + dr["motivos"].ToString().Replace(",", " "));

                        }


                    }


                    dr.Close();

                    //Create CSV File
                    using (FileStream fl = new FileStream(NombreArchivo1, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fl, Encoding.UTF8))
                        {

                            foreach (string s in ListarReporteClientesValidado1)
                            {
                                if (s != null)
                                {
                                    sw.WriteLine(s);

                                }

                            }
                            sw.Close();


                        }
                    }

                    using (FileStream fl = new FileStream(NombreArchivo2, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fl, Encoding.UTF8))
                        {
                            foreach (string s in ListarReporteClientesValidado2)
                            {
                                if (s != null)
                                {
                                    sw.WriteLine(s);

                                }

                            }
                            sw.Close();


                        }
                    }

                }

                //Enviar correo;
                string MessageMail = "Para: johel.llanos@dataimagenes.pe; daniel.hualpa@dataimagenes.pe; melody.roncal@dataimagenes.pe; UNT Malpartida Falcon George Kristhian; Canales Susanibar Junior Jaime; UNT Marcacuzco Polanco Yanina; UNT Pillihuaman Hurtado Zarmir; gustavo.sedano@dataimagenes.pe <br>Asunto: Proyecto DESA" +

"Se proceso correctamente. El archivo  se encuentra en la carpeta de salida del cliente: PAGO_EFECTIVO Data <br>" +
"Aviso de Confidencialidad <br>" +
"La información contenida en este e-mail y en sus posibles adjuntos es de propiedad de Dataimágenes S.A.C. y está sometida a obligaciones de confidencialidad y seguridad. En ese sentido, está dirigida exclusivamente a su destinatario. Por ello, cualquier revisión, difusión, reenvío, distribución o copiado de este e-mail está prohibido. Si usted no es el destinatario o una persona autorizada por éste para recibir esta comunicación y ha recibido este e-mail por error, por favor bórrelo y envíe un mensaje al remitente. A no ser que expresamente se diga lo contrario por escrito y por un funcionario autorizado para hacerlo, Dataimágenes no garantiza la veracidad ni acuciosidad de lo señalado en este email o en sus adjuntos. Finalmente, Dataimágenes no puede garantizar que esta comunicación esté libre de alteraciones o interceptaciones en su curso <br>" +
"Dataimagenes SAC";


                Mail ObjMail = new Mail();
                ObjMail._NetworkCredencialUser = ConfigurationManager.AppSettings["NetworkCredencialUser"];
                ObjMail._NetworkCredencialPass = ConfigurationManager.AppSettings["NetworkCredencialPass"];
                ObjMail._CorreoGmailOrigen = "pillihuamanhz@gmail.com";
                ObjMail._NombreGmailOrigen = "Proyecto DESA";
                ObjMail._ListaCorreos = new Queue<string>();
                ObjMail._ListaCorreos.Enqueue("zarmir.pillihuaman@orbis.com.pe");
                ObjMail._ListaCorreos.Enqueue("pillihuamanhz@gmail.com");
                ObjMail._ListaCorreos.Enqueue("universo_infinito88@hotmail.com");
                ObjMail._Subject = "Proyecto DESA";
                ObjMail._BodyHtml = "<html><body>" + MessageMail + "</body></html>";
                ObjMail.File_Path_ToAtacch = NombreArchivo1;
                ObjMail.File_Path_ToAtacch = NombreArchivo2;
                ObjMail._DisponibleHTMLBODy = true;
                ObjMail._Puerto = ConfigurationManager.AppSettings["Puerto"];
                //MailOperaciones.SenMail(ObjMail);
                /////

                Attachment Attach;
                MailMessage mailMessage = new MailMessage();
                SmtpClient SMtCliente = new SmtpClient("smtp.gmail.com");
                mailMessage.From = new MailAddress(ObjMail._CorreoGmailOrigen, ObjMail._NombreGmailOrigen);
                foreach (string s in ObjMail._ListaCorreos)
                {
                    mailMessage.To.Add(s);

                }
                mailMessage.Subject = ObjMail._Subject;
                mailMessage.IsBodyHtml = ObjMail._DisponibleHTMLBODy;
                mailMessage.Body = ObjMail._BodyHtml;
                Attach = new Attachment(NombreArchivo1);
                mailMessage.Attachments.Add(Attach);
                Attach = new Attachment(NombreArchivo2);
                mailMessage.Attachments.Add(Attach);
                SMtCliente.Port = int.Parse(ObjMail._Puerto);
                SMtCliente.Credentials = new System.Net.NetworkCredential(ObjMail._NetworkCredencialUser, ObjMail._NetworkCredencialPass);
                SMtCliente.EnableSsl = ObjMail._DisponibleHTMLBODy;

                SMtCliente.Send(mailMessage);


            }

            catch (Exception ex)
            {
                throw ex;
            }

            return "s";

        }
    }
}