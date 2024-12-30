<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="KomponenetService.User.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <div class="col-lg-6 offset-lg-3">
                <div class="mb-4">
                    <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                </div>
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">Login</h4>
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <label class="mb-1">Email</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter email"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Email is required"
                                ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                ValidationGroup="login">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label class="mb-1">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter password"
                                TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
                                ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                ValidationGroup="login">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="card-footer border-secondary bg-transparent">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-lg btn-block btn-primary font-weight-bold my-3 py-3"
                            OnClick="btnLogin_Click" ValidationGroup="login" />
                        <p class="text-center mb-0">Don't have an account? <a href="Register.aspx">Register</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
