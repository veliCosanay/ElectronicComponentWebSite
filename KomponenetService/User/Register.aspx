<%@ Page Title="" Language="C#" MasterPageFile="~/User/User.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="KomponenetService.User.Register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Register Start -->
    <div class="container-fluid pt-5">
        <div class="row px-xl-5">
            <div class="col-lg-8 offset-lg-2">
                <div class="card border-secondary mb-5">
                    <div class="card-header bg-secondary border-0">
                        <h4 class="font-weight-semi-bold m-0">Register</h4>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="mb-1">Full Name</label>
                                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter full name"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required"
                                        ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationGroup="register">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="mb-1">Mobile Number</label>
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter mobile number"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ErrorMessage="Mobile number is required"
                                        ControlToValidate="txtMobile" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationGroup="register">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="mb-1">Email</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter email" TextMode="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Email is required"
                                        ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationGroup="register">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label class="mb-1">Password</label>
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter password"
                                        TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
                                        ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationGroup="register">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="mb-1">Address</label>
                            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter address"
                                TextMode="MultiLine" Rows="3"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Address is required"
                                ControlToValidate="txtAddress" ForeColor="Red" Display="Dynamic" SetFocusOnError="true" ValidationGroup="register">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <label class="mb-1">Profile Picture</label>
                            <asp:FileUpload ID="fuProfilePicture" runat="server" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="card-footer border-secondary bg-transparent">
                        <div class="mb-3">
                            <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                        </div>
                        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-lg btn-block btn-primary font-weight-bold my-3 py-3"
                            OnClick="btnRegister_Click" ValidationGroup="register" />
                        <p class="text-center mb-0">Already have an account? <a href="Login.aspx">Login</a></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Register End -->

</asp:Content> 