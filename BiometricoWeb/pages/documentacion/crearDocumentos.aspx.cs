﻿using BiometricoWeb.clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BiometricoWeb.pages.documentacion
{
    public partial class crearDocumentos : System.Web.UI.Page
    {
        db vConexion = new db();
        protected void Page_Load(object sender, EventArgs e){
			try{
                if (!Page.IsPostBack){
                    if (Convert.ToBoolean(Session["AUTH"])){
                        String vEx = Request.QueryString["ex"];
                        if (vEx == "2")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Documento cargado con éxito.')", true);
                        else if(vEx == "3")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Solicitud no completada, favor comuníquese con sistemas.')", true);
                        else if (vEx == "4")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Favor ingrese el nombre.')", true);
                        else if (vEx == "5")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Favor seleccione la categoría del documento.')", true);
                        else if (vEx == "6")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Favor agrege usuarios que verán el documento.')", true);
                        else if (vEx == "7")
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Pop", "window.alert('Favor ingrese el documento.')", true);
                        DataTable vDatos = (DataTable)Session["AUTHCLASS"];
                        cargarDatos();
                    }
                }
			}catch (Exception ex){
				throw new Exception(ex.Message);
			}
        }

		private void cargarDatos() {
            try{
                String vQuery = "[RSP_Documentacion] 1";
                DataTable vDatos = vConexion.obtenerDataTable(vQuery);
                if (vDatos.Rows.Count > 0){
                    GvTipos.DataSource = vDatos;
                    GvTipos.DataBind();
                    Session["DOCUMENTOS_TIPO"] = vDatos;
                    
                    vQuery = "[RSP_Perfiles] 2," + Session["USUARIO"].ToString() + ", 5";
                    DataTable vDataInfo = vConexion.obtenerDataTable(vQuery);
                    if (vDataInfo.Rows[0][0].ToString() == "0"){
                        foreach (GridViewRow row in GvTipos.Rows){
                            var vLinkButton = row.FindControl("BtnEditar") as LinkButton;
                            vLinkButton.Visible = false;
                        }
                    }
                }
                
                vQuery = "[RSP_Documentacion] 2";
                vDatos = vConexion.obtenerDataTable(vQuery);
                if (vDatos.Rows.Count > 0){
                    DDLCategoria.Items.Clear();
                    DDLCategoria.Items.Add(new ListItem { Value = "0", Text = "Seleccione una opción" });
                    foreach (DataRow item in vDatos.Rows){
                        DDLCategoria.Items.Add(new ListItem { Value = item["idCategoria"].ToString(), Text = item["nombre"].ToString() });
                    }
                }
                
                vQuery = "[RSP_Documentacion] 6";
                vDatos = vConexion.obtenerDataTable(vQuery);
                if (vDatos.Rows.Count > 0){
                    DDLEmpleados.Items.Clear();
                    DDLEmpleados.Items.Add(new ListItem { Value = "0", Text = "Seleccione una opción" });
                    foreach (DataRow item in vDatos.Rows){
                        DDLEmpleados.Items.Add(new ListItem { Value = item["idEmpleado"].ToString(), Text = item["nombre"].ToString()  });
                    }
                }

            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        public void Mensaje(string vMensaje, WarningType type){
            ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "text", "infatlan.showNotification('top','center','" + vMensaje + "','" + type.ToString().ToLower() + "')", true);
        }

        protected void GvTipos_RowCommand(object sender, GridViewCommandEventArgs e){
            try{
                limpiarModal();

                String vId = e.CommandArgument.ToString();
                Session["DOCUMENTOS_TIPO_ID"] = vId;
                if (e.CommandName == "NuevoDoc"){
                    Session["DOCUMENTOS_CORREOS"] = null;
                    GvCorreos.DataSource = null;
                    GvCorreos.DataBind();

                    if (vId == "1")
                        LitTitulo.Text = "Boletines";
                    else if (vId == "2")
                        LitTitulo.Text = "Formatos";
                    else if (vId == "3")
                        LitTitulo.Text = "Manuales";
                    else if (vId == "4")
                        LitTitulo.Text = "Politicas";
                    else if (vId == "5")
                        LitTitulo.Text = "Procesos";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
                }else if (e.CommandName == "EntrarDoc"){
                    Response.Redirect("tipoDocumentos.aspx");
                }
            }catch (Exception Ex){
                Mensaje(Ex.Message, WarningType.Danger);
            }
        }

        protected void GvTipos_PageIndexChanging(object sender, GridViewPageEventArgs e){
            try{
                GvTipos.PageIndex = e.NewPageIndex;
                GvTipos.DataSource = (DataTable)Session["DOCUMENTOS_TIPO"];
                GvTipos.DataBind();
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void BtnCargar_Click(object sender, EventArgs e){
            try{
                validarDatos();
                String vArchivo = "", vExtension = "";
                vExtension = Path.GetExtension(FUArchivo.FileName);

                if (DDLCorreo.SelectedValue == "1"){
                    HttpPostedFile bufferDepositoT = FUArchivo.PostedFile;
                    String vNombreDepot = String.Empty;
                    byte[] vFileDeposito = null;

                    if (bufferDepositoT != null){
                        vNombreDepot = FUArchivo.FileName;
                        Stream vStream = bufferDepositoT.InputStream;
                        BinaryReader vReader = new BinaryReader(vStream);
                        vFileDeposito = vReader.ReadBytes((int)vStream.Length);
                    }
                    if (vFileDeposito != null)
                        vArchivo = Convert.ToBase64String(vFileDeposito);
                }

                String archivoLog = string.Format("{0}_{1}", Convert.ToString(Session["usuario"]), DateTime.Now.ToString("yyyyMMddHHmmss"));
                String vDireccionCarga = ConfigurationManager.AppSettings["RUTA_SERVER_DOCS"].ToString() + LitTitulo.Text.ToLower();

                String vNombreArchivo = FUArchivo.FileName;
                vDireccionCarga += "/" + archivoLog + "_" + vNombreArchivo;
                FUArchivo.SaveAs(vDireccionCarga);
                Boolean vCargado = File.Exists(vDireccionCarga) ? true : false;
                if (vCargado){
                    xml vDatos = new xml();
                    Object[] vDatosMaestro = new object[17];
                    vDatosMaestro[0] = Session["DOCUMENTOS_TIPO_ID"].ToString();
                    vDatosMaestro[1] = DDLCategoria.SelectedValue;
                    vDatosMaestro[2] = TxNombre.Text;
                    vDatosMaestro[3] = FUArchivo.FileName;
                    vDatosMaestro[4] = vExtension;
                    vDatosMaestro[5] = vArchivo;
                    vDatosMaestro[6] = vDireccionCarga;
                    vDatosMaestro[7] = DDLConfirmacion.SelectedValue;
                    vDatosMaestro[8] = DDLCorreo.SelectedValue;
                    //vDatosMaestro[9] = DDLCorreo.SelectedValue == "1" ? Convert.ToDateTime(TxFecha.Text).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00";
                    vDatosMaestro[9] = "1900-01-01 00:00:00";
                    vDatosMaestro[10] = TxFrecuencia.Text;
                    vDatosMaestro[11] = DDLFormatoFrecuencia.SelectedItem;
                    vDatosMaestro[12] = TxDurante.Text;
                    vDatosMaestro[13] = DDLDurante.SelectedItem;
                    vDatosMaestro[14] = DDLEstado.SelectedValue;
                    vDatosMaestro[15] = Session["USUARIO"].ToString();
                    vDatosMaestro[16] = CBxAdjunto.Checked;
                    String vXML = vDatos.ObtenerXMLDocumentos(vDatosMaestro);
                    vXML = vXML.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                
                    String vQuery = "[RSP_Documentacion] 4,0" +
                                    ",'" + vXML + "'";
                    int vInfo = vConexion.obtenerId(vQuery);
                    if (vInfo > 0){
                        DataTable vDTConfidenciales = (DataTable)Session["DOCUMENTOS_CORREOS"];
                        if (DDLCategoria.SelectedValue == "2"){
                            String vUsuarios = "";
                            for (int i = 0; i < vDTConfidenciales.Rows.Count; i++){
                                vQuery = "[RSP_Documentacion] 8" +
                                    "," + vDTConfidenciales.Rows[i]["idEmpleado"].ToString() +
                                    ",null," + vInfo + 
                                    ",'" + vDTConfidenciales.Rows[i]["emailEmpresa"].ToString() + "'";
                                vConexion.ejecutarSql(vQuery);

                                int vIdEmpleado = Convert.ToInt32(vDTConfidenciales.Rows[i]["idEmpleado"].ToString());
                                vUsuarios += vDTConfidenciales.Rows[i]["emailEmpresa"].ToString() + ",";
                                if (DDLCorreo.SelectedValue == "1")
                                    registrarMail(TxNombre.Text, vDTConfidenciales.Rows[i]["emailEmpresa"].ToString(), Session["DOCUMENTOS_TIPO_ID"].ToString(), vInfo,  vIdEmpleado);
                            }
                            /*
                            StringBuilder text = new StringBuilder(vUsuarios);
                            text.Replace(",","",vUsuarios.Length -1, 1);
                            vUsuarios = text.ToString();
                            if (DDLCorreo.SelectedValue == "1")
                                registrarMail(vUsuarios, TxNombre.Text, Session["DOCUMENTOS_TIPO_ID"].ToString());
                            */
                        }else{
                            if (DDLCorreo.SelectedValue == "1") {
                                String vConsulta = "[RSP_Documentacion] 6";
                                DataTable vData = vConexion.obtenerDataTable(vConsulta);
                                for (int i = 0; i < vData.Rows.Count; i++){
                                    registrarMail(TxNombre.Text, vData.Rows[i]["emailEmpresa"].ToString(), Session["DOCUMENTOS_TIPO_ID"].ToString(), vInfo, Convert.ToInt32(vData.Rows[i]["idEmpleado"].ToString()));
                                }
                            }
                            //if (DDLCorreo.SelectedValue == "1")
                            //    registrarMail("infatlan@bancatlan.hn", TxNombre.Text, Session["DOCUMENTOS_TIPO_ID"].ToString());
                        }
                        Response.Redirect("crearDocumentos.aspx?ex=2");
                    }else
                        Response.Redirect("crearDocumentos.aspx?ex=3");
                }else
                    Response.Redirect("crearDocumentos.aspx?ex=3");
                    
                limpiarModal();
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void DDLCorreo_SelectedIndexChanged(object sender, EventArgs e){
            try{
                if (DDLCorreo.SelectedValue == "1") 
                    //DivCorreos.Visible = false;
                    DivCorreos.Visible = true;
                else if (DDLCorreo.SelectedValue == "0") { 
                    DivCorreos.Visible = false;
                    DivSiempre.Visible = false;
                    DDLRecurrencia.SelectedValue = "0";
                }
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void DDLRecurrencia_SelectedIndexChanged(object sender, EventArgs e){
            try{
                if (DDLRecurrencia.SelectedValue == "0") 
                    DivSiempre.Visible = false;
                else if (DDLRecurrencia.SelectedValue == "1") 
                    DivSiempre.Visible = true;
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        private void limpiarModal() {
            TxNombre.Text = string.Empty;
            TxFecha.Text = string.Empty;
            TxFrecuencia.Text = string.Empty;
            DDLFormatoFrecuencia.SelectedValue = "0";
            DDLCategoria.SelectedValue = "0";
            DDLConfirmacion.SelectedValue = "0";
            DDLCorreo.SelectedValue = "0";
            DDLRecurrencia.SelectedValue = "0";
            DivMensaje.Visible = false;
            DivCorreos.Visible = false;
            DivSiempre.Visible = false;
            LbAdvertencia.Text = string.Empty;
            DDLEstado.SelectedValue = "1";
        }

        private void validarDatos() {
            if (TxNombre.Text == string.Empty || TxNombre.Text == "")
                Response.Redirect("crearDocumentos.aspx?ex=4");
            if (DDLCategoria.SelectedValue == "0")
                Response.Redirect("crearDocumentos.aspx?ex=5");
            if (DDLCategoria.SelectedValue == "2"){
                DataTable vDatos = (DataTable)Session["DOCUMENTOS_CORREOS"];
                if (vDatos == null || vDatos.Rows.Count < 1)
                    Response.Redirect("crearDocumentos.aspx?ex=6");
            }
            if (!FUArchivo.HasFile)
                Response.Redirect("crearDocumentos.aspx?ex=7");

            //if (DDLCorreo.SelectedValue == "1") {
            //    if (TxFecha.Text == string.Empty || TxFecha.Text == "")
            //        Response.Redirect("crearDocumentos.aspx?ex=6");
            //    throw new Exception("Favor ingrese la fecha de inicio del correo.");
            //}
        }

        protected void DDLCategoria_SelectedIndexChanged(object sender, EventArgs e){
            try{
                if (DDLCategoria.SelectedValue == "2")
                    DivEmpleados.Visible = true;
                else 
                    DivEmpleados.Visible = false;
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void LBAgregarCorreos_Click(object sender, EventArgs e){
            try{
                DivMensajeCorreo.Visible = false;
                LbMensajeCorreo.Text = "";
                DDLEmpleados.SelectedValue = "0";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModalCorreos();", true);
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void BtnAgregar_Click(object sender, EventArgs e){
            try{
                DataTable vDatosCorreos = (DataTable)Session["DOCUMENTOS_CORREOS"];
                if (vDatosCorreos == null || vDatosCorreos.Rows.Count < 1)
                    throw new Exception("Favor ingrese al menos un empleado");

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "closeModalCorreos();", true);
            }catch (Exception ex){
                DivMensajeCorreo.Visible = true;
                LbMensajeCorreo.Text = ex.Message;
            }
        }

        protected void BtnAgregarCorreo_Click(object sender, EventArgs e){
            try{
                if (DDLEmpleados.SelectedValue == "0")
                    throw new Exception("Favor seleccione el empleado.");

                DivMensajeCorreo.Visible = false;
                String vQuery = "[RSP_Documentacion] 7," + DDLEmpleados.SelectedValue;
                DataTable vDatos = vConexion.obtenerDataTable(vQuery);

                DataTable vNewDatos = new DataTable();
                DataTable vData = (DataTable)Session["DOCUMENTOS_CORREOS"];
                
                vNewDatos.Columns.Add("idEmpleado");
                vNewDatos.Columns.Add("nombre");
                vNewDatos.Columns.Add("emailEmpresa");
                vNewDatos.Columns.Add("emailPersonal");
                
                if (vData == null)
                    vData = vNewDatos.Clone();
                
                Boolean vFlag = true;
                if (vData.Rows.Count > 0){
                    for (int i = 0; i < vData.Rows.Count; i++){
                        if (vData.Rows[i]["idEmpleado"].ToString() == DDLEmpleados.SelectedValue) { 
                            vFlag = false;
                            break;
                        }
                    }
                    if (vFlag)
                        vData.Rows.Add(DDLEmpleados.SelectedValue, DDLEmpleados.SelectedItem, vDatos.Rows[0]["emailEmpresa"].ToString(), vDatos.Rows[0]["emailPersonal"].ToString());
                }else{
                    vData.Rows.Add(DDLEmpleados.SelectedValue, DDLEmpleados.SelectedItem, vDatos.Rows[0]["emailEmpresa"].ToString(), vDatos.Rows[0]["emailPersonal"].ToString());
                }
                
                if (vData.Rows.Count > 0 && vFlag){
                    Session["DOCUMENTOS_CORREOS"] = vData;
                    GvCorreos.DataSource = vData;
                    GvCorreos.DataBind();
                }
            }catch (Exception ex){
                DivMensajeCorreo.Visible = true;
                LbMensajeCorreo.Text = ex.Message;
            }
        }

        protected void GvCorreos_PageIndexChanging(object sender, GridViewPageEventArgs e){
            try{
                GvCorreos.PageIndex = e.NewPageIndex;
                GvCorreos.DataSource = (DataTable)Session["DOCUMENTOS_CORREOS"];
                GvCorreos.DataBind();
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        protected void GvCorreos_RowCommand(object sender, GridViewCommandEventArgs e){
            try{
                DataTable vDatos = (DataTable)Session["DOCUMENTOS_CORREOS"];
                if (e.CommandName == "BorrarCorreo"){
                    String vID = e.CommandArgument.ToString();
                    if (Session["DOCUMENTOS_CORREOS"] != null){
                        DataRow[] result = vDatos.Select("idEmpleado = '" + vID + "'");
                        foreach (DataRow row in result){
                            if (row["idEmpleado"].ToString().Contains(vID))
                                vDatos.Rows.Remove(row);
                        }
                    }
                }
                GvCorreos.DataSource = vDatos;
                GvCorreos.DataBind();
                Session["DOCUMENTOS_CORREOS"] = vDatos;
            }catch (Exception ex){
                Mensaje(ex.Message, WarningType.Danger);
            }
        }

        private Boolean registrarMail(String vNombre, String vPara, String vArchivo, int vIdDocumento, int vEmpleado) {
            Boolean vFlag = false;
            try{
                if (vArchivo == "1")
                    vArchivo = " Boletin";
                else if (vArchivo == "2")
                    vArchivo = " Formato";
                else if (vArchivo == "3")
                    vArchivo = " Manual";
                else if (vArchivo == "4")
                    vArchivo = "a Politica";
                else if (vArchivo == "5")
                    vArchivo = "Proceso";

             
                CryptoToken.CryptoToken vToken = new CryptoToken.CryptoToken();

                tokenClass vClassToken = new tokenClass(){
                    usuario = vEmpleado,
                    parametro1 = vIdDocumento.ToString()
                };

                String vTokenString = vToken.Encrypt(JsonConvert.SerializeObject(vClassToken), ConfigurationManager.AppSettings["TOKEN_DOC"].ToString());
                String vQuery = "[RSP_Documentacion] 12" +
                    ",'" + vNombre + "'" +
                    ",'" + vPara + "'" +
                    "Se ha creado un nuevo ducumento, favor revisarlo'" +
                    ",null," + vEmpleado +
                    "," + "<a href=http://10.128.0.149:1007/pages/documentacion/archivo.aspx?id='" + vTokenString + "'></a>";
                int vRes = vConexion.ejecutarSql(vQuery);


                /*
                SmtpService vService = new SmtpService();
                vService.EnviarMensaje(vPara,
                    typeBody.Documentos,
                    "",
                    "Se ha creado un" + vArchivo + " en el módulo de documentación. Favor revisarlo",
                    "El nombre del documento es: " + vNombre
                    );
                */
                vFlag = vRes == 1 ? true : false;
            }catch (Exception ex){
                throw;
            }
            return vFlag;
        }
    }
}