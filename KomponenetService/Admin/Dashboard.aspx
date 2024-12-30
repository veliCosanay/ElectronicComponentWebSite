<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="KomponenetService.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-4 px-4">
        <div class="row g-4">
            <div class="col-12">
                <div class="bg-light rounded h-100 p-4">
                    <div class="mb-4">
                        <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                    </div>
                    <h4 class="mb-4">Admin List</h4>
                    <div class="table-responsive">
                        <asp:Repeater ID="rAdmins" runat="server" OnItemCommand="rAdmins_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-hover">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Name</th>
                                            <th>Email</th>
                                            <th>Mobile</th>
                                            <th>Address</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("SrNo") %></td>
                                    <td><%# Eval("name") %></td>
                                    <td><%# Eval("email") %></td>
                                    <td><%# Eval("mobile") %></td>
                                    <td><%# Eval("address") %></td>
                                    <td>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-info" CommandName="edit"
                                            CommandArgument='<%# Eval("userid") %>'>
                                            <i class="fa fa-edit"></i>
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>

            <div class="col-12" id="adminProfile" runat="server" visible="false">
                <div class="bg-light rounded h-100 p-4">
                    <h4 class="mb-4">Admin Profile</h4>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group mb-3">
                                <label>Name</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter Name"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required"
                                    ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="Admin">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Email</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Email is required"
                                    ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="Admin">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Mobile</label>
                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter Mobile"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ErrorMessage="Mobile is required"
                                    ControlToValidate="txtMobile" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="Admin">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter Password" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
                                    ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="Admin">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Address</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter Address" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Address is required"
                                    ControlToValidate="txtAddress" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="Admin">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <asp:Button ID="btnUpdate" runat="server" Text="Update Profile" CssClass="btn btn-primary"
                                    OnClick="btnUpdate_Click" ValidationGroup="Admin" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary"
                                    OnClick="btnCancel_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
