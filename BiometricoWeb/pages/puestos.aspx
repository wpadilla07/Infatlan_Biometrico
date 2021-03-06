﻿<%@ Page Title="" Language="C#" MasterPageFile="~/main.Master" AutoEventWireup="true" CodeBehind="puestos.aspx.cs" Inherits="BiometricoWeb.pages.puestos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/css/smart_wizard.css" rel="stylesheet" type="text/css" />
    <link href="/css/smart_wizard_theme_circles.css" rel="stylesheet" type="text/css" />
    <link href="/css/smart_wizard_theme_arrows.css" rel="stylesheet" type="text/css" />
    <link href="/css/smart_wizard_theme_dots.css" rel="stylesheet" type="text/css" />
    <link href="/css/GridStyle.css" rel="stylesheet" />
    <link href="/css/pager.css" rel="stylesheet" />
    <link href="/css/breadcrumb.css" rel="stylesheet" />
    <%--<script type="text/javascript">
        var updateProgress = null;

        function postbackButtonClick() {
            updateProgress = $find("<%= UpdateProgress1.ClientID %>");
            window.setTimeout("updateProgress.set_visible(true)", updateProgress.get_displayAfter());
            return true;
        }
    </script>--%>
    <script type="text/javascript">
        function openModal() { $('#PuestosModal').modal('show'); }
        function openModalDescriptor() { $('#ModalDescriptor').modal('show'); }

        var url = document.location.toString();
        if (url.match('#')) {
            $('.nav-tabs a[href="#' + url.split('#')[1] + '"]').tab('show');
        }

        $('.nav-tabs a').on('shown.bs.tab', function (e) {
            window.location.hash = e.target.hash;
        })
    </script>

    <script type="text/javascript">
        //Guardar PDF
        function PDF(input) {

            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var ruta1 = e.target.result;
                    document.getElementById('<%=FUSubirPDF.ClientID%>').src = ruta1;
                    document.getElementById('<%=HFSubirPDF.ClientID%>').value = 'si';
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdateDivBusquedas" runat="server">
        <ContentTemplate>
            <div class="row" id="DivBusqueda" runat="server">
                <div class="col-12 grid-margin stretch-card">
                    <div class="card">
                        <div class="card-body">
                            <h4 class="card-title">Puestos o Posiciones</h4>
                            <p>Puestos de trabajo existentes.</p>
                            <div class="col-md-12">
                                <div class="form-group row">
                                    <label class="col-sm-1 col-form-label">Buscar</label>
                                    <div class="col-sm-6">
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                            <ContentTemplate>
                                                <asp:TextBox ID="TxBuscarPuesto" runat="server" placeholder="Ej. Programador - Presione afuera para proceder" class="form-control" AutoPostBack="true" OnTextChanged="TxBuscarPuesto_TextChanged"></asp:TextBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:Button ID="btnNuevo" runat="server" Text="Crear Puesto" class="btn btn-primary" OnClick="btnNuevo_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row" id="Div1" runat="server">
                <div class="col-12 grid-margin stretch-card">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="table-responsive">
                                    <asp:UpdatePanel ID="UpdateGridView" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="GVBusqueda" runat="server"
                                                CssClass="mydatagrid"
                                                PagerStyle-CssClass="pgr"
                                                HeaderStyle-CssClass="align-self-lg-start"
                                                RowStyle-CssClass="rows"
                                                GridLines="None"
                                                AllowPaging="true"
                                                PageSize="10"
                                                AutoGenerateColumns="false" OnPageIndexChanging="GVBusqueda_PageIndexChanging" OnRowCommand="GVBusqueda_RowCommand">
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-Width="60px" Visible="true">
                                                        <ItemTemplate>
                                                            <asp:Button ID="BtnPuestoModificar" runat="server" Text="Modificar" class="btn btn-inverse-primary  mr-2" CommandArgument='<%# Eval("idPuesto") %>' CommandName="PuestoModificar" />
                                                        </ItemTemplate>

                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-Width="60px" Visible="true">
                                                        <ItemTemplate>
                                                            <asp:Button ID="BtnDescriptor" Enabled="true" runat="server" Text="Descriptor" class="btn btn-inverse-success  mr-2" CommandArgument='<%# Eval("idPuesto") %>' CommandName="DescriptorPuesto" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="idPuesto" HeaderText="Id Puesto" ItemStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="nombre" HeaderText="Nombre" ItemStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" ItemStyle-HorizontalAlign="Left" />
                                                </Columns>

                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>


        </ContentTemplate>
    </asp:UpdatePanel>


    <%--MODAL DE MODIFICACION--%>
    <div class="modal fade" id="PuestosModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="ModalLabelModificacion">

                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                Gestionar Puesto
                               
                                <asp:Label ID="LbModPuesto" runat="server" Text=""></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdateModificarPuesto" runat="server">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">Id del puesto</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox ID="TxIdPuesto" ReadOnly="true" placeholder="" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group row">
                                        <label class="col-sm-3">Nombre del puesto</label>
                                        <div class="col-sm-9">
                                            <asp:TextBox ID="TxPuesto" placeholder="" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">Departamento</label>
                                        <div class="col-sm-9">
                                            <asp:DropDownList ID="DDLDepto" runat="server" class="form-control"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12" runat="server" id="DivEstado" visible="false">
                                    <div class="form-group row">
                                        <label class="col-sm-3 col-form-label">Estado</label>
                                        <div class="col-sm-9">
                                            <asp:DropDownList ID="DDLEstado" runat="server" class="form-control">
                                                <asp:ListItem Value="1" Text="Activo"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="Inactivo"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:UpdatePanel ID="UpdateModificacionBotones" runat="server">
                        <ContentTemplate>
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                            <asp:Button ID="BtnCrear" runat="server" Text="Guardar" class="btn btn-success" OnClick="BtnCrear_Click" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <%--MODAL PARA SUBIR DESCRIPTORES--%>
    <div class="modal fade" id="ModalDescriptor" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content" style="width: 600px; top: 320px; left: 50%; transform: translate(-50%, -50%);">
                <div class="modal-header">

                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                            <h4 class="modal-title" id="ModalLabelPermiso">Subir archivos - 
                               
                                <asp:Label ID="LbSubir" runat="server" Text=""></asp:Label>
                            </h4>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <div class="col-md-12">
                        <div class="form-group row">
                            <label class="col-sm-3 col-form-label">
                                <h4>Documentos</h4>
                            </label>
                            <div class="col-md-12">
                                <div class="form-group row">
                                    <div class="col-sm-12" style="text-align: justify;">
                                        Recuerde subir los documentos PDF.
                                           
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-12">
                                <asp:FileUpload ID="FUSubirPDF" runat="server" class="form-control" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                        <ContentTemplate>
                            <div class="col-md-12">
                                <div class="form-group row">
                                    <div class="col-sm-12" runat="server" id="DivAlertaDescriptor" visible="false" style="display: flex; background-color: tomato; justify-content: center">
                                        <asp:Label runat="server" CssClass="col-form-label text-white" ID="LbAlertaDescriptor"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                        <ContentTemplate>
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                            <asp:Button ID="BtnSubirDescriptor" runat="server" Text="Subir" class="btn btn-success" OnClick="BtnSubirDescriptor_Click" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="BtnSubirDescriptor" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="HFSubirPDF" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
