﻿using BiometricoWeb.clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace BiometricoWeb.pages.viaticos
{
    public partial class cotizarViaje : System.Web.UI.Page
    {
        db vConexion = new db();
        db vConexion2 = new db();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Convert.ToBoolean(Session["AUTH"])){
                    cargarData();
                }else{
                    Response.Redirect("/login.aspx");
                }
            }
        }
        void cargarData()
        {
            TxFechaInicio.Text = Convert.ToString(Session["VIATICOS_FECHA_INICIO"]);
            TxFechaRegreso.Text= Convert.ToString(Session["VIATICOS_FECHA_FIN"]);
            txtEmpleado.Text= Convert.ToString(Session["VIATICOS_EMPLEADO"]);
            txtMotivoViaje.Text= Convert.ToString(Session["VIATICOS_MOTIVOVIAJE"]);
            txtTipoViaje.Text= Convert.ToString(Session["VIATICOS_TIPOVIAJE"]);
            if (txtTipoViaje.Text == "Internacional")
            {
                txtDestino.Text = Convert.ToString(Session["VIATICOS_NEWPAIS"]);
            }
            else
            {
                //CARGAR DESTINO FINAL
                String vQuery10 = "STEISP_ATM_Generales 27,'"+ Convert.ToString(Session["VIATICOS_DESTINOF"]) + "'";
                DataTable vDatos10 = vConexion2.obtenerDataTableSTEI(vQuery10);
                foreach (DataRow item in vDatos10.Rows)
                {
                    txtDestino.Text = item["nombre"].ToString();
                }
               
            }
            if (Session["VIATICOS_COMENTARIORRHH"] == null)
                LBComRRHH.Text = "";
            else
                LBComRRHH.Text = "*Comentario: " + Convert.ToString(Session["VIATICOS_COMENTARIORRHH"]);

            string codViaticos = Convert.ToString(Session["VIATICOS_CODIGO"]);
            DataTable vDatos3 = new DataTable();
            vDatos3 = vConexion.obtenerDataTable("VIATICOS_ObtenerGenerales 22,'" + codViaticos + "'");
            foreach (DataRow item in vDatos3.Rows)
            {
                txtCompañia.Text= item["empresa"].ToString();
                txtcosto.Text= item["costo"].ToString();
                txtcomentario.Text = item["comentario"].ToString();
            }

            }
        public void Mensaje(string vMensaje, WarningType type)
        {
            ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "text", "infatlan.showNotification('top','center','" + vMensaje + "','" + type.ToString().ToLower() + "')", true);
        }
        protected void BtnCrearPermiso_Click(object sender, EventArgs e)
        {
            if (txtCompañia.Text == "" || txtcosto.Text == "")
                Mensaje("No puede campos vacios",WarningType.Warning);
            else
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Pop", "openModal();", true);
        }

        protected void btnModalEnviar_Click(object sender, EventArgs e)
        {
            //string vEmpleado = Convert.ToString(Session["VIATICOS_IDEMPLEADO"]);
            string vQuery = "VIATICOS_Solicitud 5, '" + Session["VIATICOS_CODIGO"].ToString() + "','" + txtCompañia.Text + "','" + txtcosto.Text + "','" + txtcomentario.Text + "', '"+ Session["USUARIO"].ToString() + "','" + Session["VIATICOS_IDEMPLEADO"] + "'";
            Int32 vInfo = vConexion.ejecutarSql(vQuery);
            DataTable vDatosSiguiente = vConexion.obtenerDataTable(vQuery);
            if (vInfo == 1)
            {
                //SmtpService vService = new SmtpService();
                //string vQueryD = "VIATICOS_ObtenerGenerales 48," + Session["VIATICOS_CODIGO"];
                //DataTable vDatosEmpleado = vConexion.obtenerDataTable(vQueryD);

                //Boolean vFlagEnvioSupervisor = false;
                //DataTable vDatosJefatura = (DataTable)Session["AUTHCLASS"];
                //if (vDatosJefatura.Rows.Count > 0)
                //{
                //    foreach (DataRow item in vDatosJefatura.Rows)
                //    {
                //        if (!item["emailEmpresa"].ToString().Trim().Equals(""))
                //        {
                //            vService.EnviarMensaje(item["emailEmpresa"].ToString(),
                //                typeBody.Cotizar,
                //                item["nombre"].ToString(),
                //                vDatosEmpleado.Rows[0]["Nombre"].ToString()
                //                );
                //            vFlagEnvioSupervisor = true;
                //        }
                //    }
                //}

                //if (vFlagEnvioSupervisor)
                //{
                //    foreach (DataRow item in vDatosSiguiente.Rows)
                //    {
                //        if (!item["emailEmpresa"].ToString().Trim().Equals(""))
                //            vService.EnviarMensaje(item["Email"].ToString(),
                //                typeBody.Jefe,
                //               item["Nombre"].ToString(),
                //              vDatosEmpleado.Rows[0]["Nombre"].ToString()

                //            );
                //    }
                //}
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Pop", "closeModal();", true);
            Response.Redirect("cotizacion.aspx");
        }

        protected void btnModalCerrar_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Pop", "closeModal();", true);
        }
    }
}