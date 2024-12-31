<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="KomponenetService.Admin.Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function ImagePreview(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('#<%=imgUser.ClientID%>').prop('src', e.target.result)
                        .width(200)
                        .height(200);
                };
                reader.readAsDataURL(input.files[0]);
            }
        }

        function toggleOrders(userId) {
            var orderDiv = document.getElementById('orderHistory_' + userId);
            if (orderDiv.style.display === 'none') {
                orderDiv.style.display = 'block';
            } else {
                orderDiv.style.display = 'none';
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid pt-4 px-4">
        <div class="row g-4">
            <div class="col-12">
                <div class="bg-light rounded h-100 p-4">
                    <div class="mb-4">
                        <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <h6 class="mb-4">User Details</h6>
                            <div class="form-group mb-3">
                                <label>Name</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter Name"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="Name is required"
                                    ControlToValidate="txtName" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="User">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Email</label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="Email is required"
                                    ControlToValidate="txtEmail" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="User">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Mobile</label>
                                <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" placeholder="Enter Mobile"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvMobile" runat="server" ErrorMessage="Mobile is required"
                                    ControlToValidate="txtMobile" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="User">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter Password" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ErrorMessage="Password is required"
                                    ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="User">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Address</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Enter Address" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Address is required"
                                    ControlToValidate="txtAddress" ForeColor="Red" Display="Dynamic" SetFocusOnError="true"
                                    ValidationGroup="User">
                                </asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group mb-3">
                                <label>Profile Picture</label>
                                <asp:FileUpload ID="fuUserImage" runat="server" CssClass="form-control" onchange="ImagePreview(this);" />
                            </div>
                            <div class="form-group mb-3">
                                <asp:Image ID="imgUser" runat="server" CssClass="img-thumbnail" Width="150" Height="150" />
                            </div>
                            <div class="form-group">
                                <asp:Button ID="btnAddOrUpdate" runat="server" Text="Add" CssClass="btn btn-primary"
                                    OnClick="btnAddOrUpdate_Click" ValidationGroup="User" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning"
                                    OnClick="btnClear_Click" CausesValidation="false" />
                            </div>
                        </div>
                        <div class="col-md-8">
                            <h6 class="mb-4">User List</h6>
                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <div class="input-group">
                                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by user name..."></asp:TextBox>
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" CausesValidation="false">
                                            <i class="fas fa-search"></i> Search
                                        </asp:LinkButton>
                                    </div>
                                </div>
                                <div class="col-md-6 text-end">
                                    <asp:LinkButton ID="btnDeleteAllUsers" runat="server" CssClass="btn btn-danger" 
                                        OnClick="btnDeleteAllUsers_Click" CausesValidation="false"
                                        OnClientClick="return confirm('Are you sure you want to delete all users? This action cannot be undone.');">
                                        <i class="fa fa-trash"></i> Delete All Users
                                    </asp:LinkButton>
                                </div>
                            </div>
                            <asp:Repeater ID="rUsers" runat="server" OnItemCommand="rUsers_ItemCommand">
                                <HeaderTemplate>
                                    <table class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th>#</th>
                                                <th>Name</th>
                                                <th>Email</th>
                                                <th>Mobile</th>
                                                <th>Role</th>
                                                <th>Action</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("out_name") %></td>
                                        <td>
                                            <img width="40" src='<%# (Convert.IsDBNull(Eval("out_imageurl")) ? "~/Images/No_image.png" : "~/" + Eval("out_imageurl").ToString()) %>' alt=""/>
                                        </td>
                                        <td><%# Eval("out_email") %></td>
                                        <td><%# Eval("out_mobile") %></td>
                                        <td><%# Eval("out_rolename") %></td>
                                        <td>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-info" CommandName="edit"
                                                CommandArgument='<%# Eval("out_userid") %>' CausesValidation="false">
                                                <i class="fa fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandName="delete"
                                                CommandArgument='<%# Eval("out_userid") %>' OnClientClick="return confirm('Do you want to delete this user?');"
                                                CausesValidation="false">
                                                <i class="fa fa-trash"></i>
                                            </asp:LinkButton>
                                            <button type="button" class="btn btn-sm btn-primary" onclick="toggleOrders('<%# Eval("out_userid") %>')">
                                                <i class="fa fa-history"></i> Orders
                                            </button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6">
                                            <div id="orderHistory_<%# Eval("out_userid") %>" style="display: none;">
                                                <asp:Repeater ID="rOrders" runat="server" DataSource='<%# GetOrderHistory(Convert.ToInt32(Eval("out_userid"))) %>'>
                                                    <HeaderTemplate>
                                                        <table class="table table-bordered">
                                                            <thead>
                                                                <tr>
                                                                    <th>Order Date</th>
                                                                    <th>Order Details</th>
                                                                    <th>Amount</th>
                                                                    <th>Payment Mode</th>
                                                                    <th>Status</th>
                                                                    <th>Shipping Address</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%# Convert.ToDateTime(Eval("createddate")).ToString("dd/MM/yyyy HH:mm") %></td>
                                                            <td><%# Eval("orderdetails") %></td>
                                                            <td>$<%# Eval("amount") %></td>
                                                            <td><%# Eval("paymentmode") %></td>
                                                            <td><%# Eval("paymentstatus") %></td>
                                                            <td><%# Eval("address") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                            </tbody>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
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
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
</asp:Content>
